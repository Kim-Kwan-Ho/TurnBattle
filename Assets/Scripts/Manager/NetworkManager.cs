using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.IO;
using ServerData;
using PlayerData;

public class NetworkManager
{
    private Thread _tcpListenerThread = null;
    private TcpClient _socketConnection = null;
    private NetworkStream _stream = null;

    public string Ip = "127.0.0.1";
    public int Port = 9001;
    public bool Logined = false;
    public bool Matched = false;
    public bool Started = false;
    private bool _clientReady = false;
    private byte[] _buffer = new byte[ServerData.Constants.BufferSize];
    private byte[] _tempBuffer = new byte[ServerData.Constants.TempBufferSize];
    private bool _isTempByte = false;
    private int _tempByteSize = 0;

    public Action<bool, string> LoginRegisterCallBack = null;
    public Action<stBattleOrdersInfo> BattleOrdersCallBack = null;
    public Action<stBattleParticularInfo> BattleParticularCallBack = null;

    public byte[] GetObjectToByte<T>(T str) where T : struct // 구조체 => Byte
    {
        int size = Marshal.SizeOf(str); // 구조체 크기 저장
        byte[] arr = new byte[size]; // 크기만큼 배열 생성
        IntPtr ptr = Marshal.AllocHGlobal(size); // 메모리 할당 
        try
        {
            Marshal.StructureToPtr(str, ptr, true); // 구조체 => 메모리 
            Marshal.Copy(ptr, arr, 0, size); // 메모리 => 바이트 복사
        }
        finally
        {
            Marshal.FreeHGlobal(ptr); // 메모리 할당 해제
        }
        return arr; // 배열 반환
    }

    public T GetObjectFromByte<T>(byte[] arr) where T : struct // Byte => 구조체
    {
        int size = Marshal.SizeOf<T>(); // 구조체 크기 저장
        IntPtr ptr = Marshal.AllocHGlobal(size); // 메모리 할당
        try
        {
            Marshal.Copy(arr, 0, ptr, size); // 배열 => 메모리 복사
            return Marshal.PtrToStructure<T>(ptr); // 메모리 => 구조체 반환
        }
        finally
        {
            Marshal.FreeHGlobal(ptr); // 메모리 할당 해제
        }
    }

    public void Init()
    {
        ConnectToTCPServer();

    }

    private void ConnectToTCPServer()
    {
        _tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequest));
        _tcpListenerThread.IsBackground = true;
        _tcpListenerThread.Start();

    }
    private void ListenForIncomingRequest()
    {
        try
        {
            _socketConnection = new TcpClient(Ip, Port);
            _stream = _socketConnection.GetStream();
            _clientReady = true;

            while (true)
            {
                if (!IsConnected(_socketConnection))
                {
                    DisConnect();
                    break;
                }
                if (_clientReady)
                {
                    if (_stream.DataAvailable)
                    {
                        Array.Clear(_buffer, 0, _buffer.Length);
                        int messageLength = _stream.Read(_buffer, 0, _buffer.Length);
                        byte[] processBuffer = new byte[messageLength + _tempByteSize];
                        if (_isTempByte)
                        {
                            Array.Copy(_tempBuffer, 0, processBuffer, 0, _tempByteSize);
                            Array.Copy(_buffer, 0, processBuffer, _tempByteSize, messageLength);
                        }
                        else
                        {
                            Array.Copy(_buffer, 0, processBuffer, 0, messageLength);
                        }

                        if (_tempByteSize + messageLength > 0)
                        {
                            OnIncomingData(processBuffer);
                        }
                    }
                    else if (_tempByteSize > 0)
                    {
                        byte[] processBuffer = new byte[_tempByteSize];
                        Array.Copy(_tempBuffer, 0, processBuffer, 0, _tempByteSize);
                        OnIncomingData(processBuffer);
                    }
                }
                else
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log(socketException.ToString());
            _clientReady = false;
        }
    }
    private bool IsConnected(TcpClient client)
    {
        if (client?.Client == null || !client.Client.Connected)
            return false;

        try
        {
            return !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
        }
        catch
        {
            return false;
        }
    }
    private void OnIncomingData(byte[] data)
    {
        if (data.Length < ServerData.Constants.HeaderSize)
        {
            Array.Copy(data, 0, _tempBuffer, _tempByteSize, data.Length);
            _isTempByte = true;
            _tempByteSize += data.Length;
            return;
        }

        byte[] headerDataByte = new byte[ServerData.Constants.HeaderSize];
        Array.Copy(data, 0, headerDataByte, 0, headerDataByte.Length);
        stHeader headerData = GetObjectFromByte<stHeader>(headerDataByte);
        if (headerData.PacketSize > data.Length)
        {
            Array.Copy(data, 0, _tempBuffer, _tempByteSize, data.Length);
            _isTempByte = true;
            _tempByteSize += data.Length;
            return;
        }

        byte[] msgData = new byte[headerData.PacketSize];
        Array.Copy(data, 0, msgData, 0, headerData.PacketSize);
        
        IncomingDataProcess(headerData.MsgID, msgData);

        if (data.Length == msgData.Length)
        {
            _isTempByte = false;
            _tempByteSize = 0;
        }
        else
        {
            Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            Array.Copy(data, msgData.Length, _tempBuffer, 0, data.Length - (msgData.Length));
            _isTempByte = true;
            _tempByteSize = data.Length - (msgData.Length);
        }
    }

    public void DisConnect()
    {
        if (_socketConnection == null)
            return;

        if (Managers.Battle.RoomID != null)
        {
            stBattleParticularInfo info = new stBattleParticularInfo();
            info.MsgID = ServerData.MessageID.BattleParticularInfo;
            info.PacketSize = (ushort)Marshal.SizeOf(info);
            info.ID = Managers.Data.ID;
            info.RoomID = (ushort)Managers.Battle.RoomID;
            info.ParticularInfo = (ushort)ParticularInfo.LogOut;
            TcpSendMessage<stBattleParticularInfo>(info);
        }

        _clientReady = false;
        _stream.Close();

        _socketConnection.Close();
        _socketConnection = null;

        _tcpListenerThread.Abort();
        _tcpListenerThread = null;

    }


    public void TcpSendMessage<T>(T sendObject) where T : struct
    {
        if (_socketConnection.Connected)
        {
            Byte[] sendBytes = GetObjectToByte<T>(sendObject);
            _socketConnection.GetStream().Write(sendBytes, 0, sendBytes.Length);
        }
    }


    private void IncomingDataProcess(ushort msgId, byte[] msgData)
    {
        switch (msgId)
        {
            case ServerData.MessageID.LoginRegister: // 로그인 실패 or 회원가입 성공여부 반환
            {
                stLoginRegister info = GetObjectFromByte<stLoginRegister>(msgData);
                if (info.IsLogin) // 로그인 정보는 실패만 반환
                {
                    LoginRegisterCallBack?.Invoke(false, "Login Failed");
                }
                else
                {
                    if (info.Succeed)
                    {
                        LoginRegisterCallBack?.Invoke(false, "Register Success");
                    }
                    else
                    {
                        LoginRegisterCallBack?.Invoke(false,"Register Failed");
                    }
                }
                break;
            }
            case ServerData.MessageID.PlayerInfo: // 로그인 성공 시 플레이어 정보 수신
            {
                stPlayerInfo info = GetObjectFromByte<stPlayerInfo>(msgData);
                Managers.Data.LoadPlayerInfo(info); // 수신한 정보 저장
                LoginRegisterCallBack?.Invoke(true, string.Empty); // 로그인 성공 반환
                break;
            }
            case ServerData.MessageID.BattleRoomInfo: // 매치 성공 시 게임 룸 정보 수신
            {
                Matched = true;  // 매치 성공
                stBattleRoomInfo info = GetObjectFromByte<stBattleRoomInfo>(msgData);
                Managers.Battle.SetBattleRoomInfo(info); // 게임 룸 정보 설정
                break;
            }
            case ServerData.MessageID.BattleStart:
            {
                stBattleStart info = GetObjectFromByte<stBattleStart>(msgData);
                Started = true;
                break;
            }
            case ServerData.MessageID.BattleOrdersInfo:
            {
                stBattleOrdersInfo info = GetObjectFromByte<stBattleOrdersInfo>(msgData);
                BattleOrdersCallBack?.Invoke(info);
                break;
            }
            case ServerData.MessageID.BattleParticularInfo:
            {
                stBattleParticularInfo info = GetObjectFromByte<stBattleParticularInfo>(msgData);
                BattleParticularCallBack?.Invoke(info);
                break;
            }
        }
    }

}
