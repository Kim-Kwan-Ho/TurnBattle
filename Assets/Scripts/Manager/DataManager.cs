using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PlayerData;
using ServerData;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

public class DataManager
{
    private Player _player = new Player();
    Character[] characters = new Character[15];

    public string ID // 이걸로 서버에서 찾아서 받아오게끔
    {
        get => _player.PlayerID; 
        set => _player.PlayerID = value;
    }

    public string Name
    {
        get => _player.PlayerName;
        set => _player.PlayerName = value;
    }

    public UInt32 Gold
    {
        get => _player.PlayerGold;
        set => _player.PlayerGold = value;
    }

    public UInt32 Token
    {
        get => _player.UpgradeToken;
        set => _player.UpgradeToken = value;
    }

    public UInt32 Power
    {
        get => _player.Power; 
        set => _player.Power = value;
    }

    public Character[] MainCharacters
    {
        get => _player.PlayerMainCharacters;
        set => _player.PlayerMainCharacters = value;
    }

    public Dictionary<UInt16, Character> Characters
    {
        get => _player.PlayerCharacters;
        set => _player.PlayerCharacters = value;
    }

    public ushort RoomID
    {
        get => _player.BattleRoomID;
        set => _player.BattleRoomID = value;
    }

    public bool IsPlayer1
    {
        get => _player.IsPlayer1;
        set => _player.IsPlayer1 = value;
    }

    public void Init()
    {
    }

    public void LoadPlayerInfo(stPlayerInfo playerInfo)
    {
        SetPlayerInfo(playerInfo);
        SetCharactersInfo(playerInfo);
    }

    private void SetPlayerInfo(stPlayerInfo playerInfo)
    {
        _player.PlayerID = playerInfo.ID;
        _player.PlayerPassword = playerInfo.Password;
        _player.PlayerGold = playerInfo.Gold;
        _player.UpgradeToken = playerInfo.Token;
    }

    private void SetCharactersInfo(stPlayerInfo playerInfo)
    {
        _player.PlayerMainCharacters = ChangeToPlayerCharacter(playerInfo.MainCharacters);
        _player.PlayerCharacters =
            ChangeToPlayerCharacter(playerInfo.Characters, playerInfo.ChsCount).ToDictionary(key => key.ChID, item => item);
    }


    private Character[] ChangeToPlayerCharacter(stCharacterInfo[] chInfos, ushort chCount = 3) 
    {
        Character[] chs = new Character[chCount];

        for (int i = 0; i < chs.Length; i++)
        {
            chs[i].ChID = chInfos[i].ChID;
            chs[i].ChLevel = chInfos[i].ChLevel;
            chs[i].ChHp = chInfos[i].ChHp;
            chs[i].ChDamage = chInfos[i].ChDamage;
            chs[i].ChArmor = chInfos[i].ChArmor;
            chs[i].ChSpd = chInfos[i].ChSpd;
        }

        return chs;
    }
    private stCharacterInfo[] ChangeToServerCharacter(Character[] chInfos, int chCount = 3)
    {
        stCharacterInfo[] chs = new stCharacterInfo[chCount];

        for (int i = 0; i < chs.Length; i++)
        {
            chs[i].ChID = chInfos[i].ChID;
            chs[i].ChLevel = chInfos[i].ChLevel;
            chs[i].ChHp = chInfos[i].ChHp;
            chs[i].ChDamage = chInfos[i].ChDamage;
            chs[i].ChArmor = chInfos[i].ChArmor;
            chs[i].ChSpd = chInfos[i].ChSpd;
        }

        return chs;
    }
    public void UpdatePlayerInfoToServer()
    {
        stPlayerInfo info = new stPlayerInfo();

        info.MsgID = ServerData.MessageID.PlayerInfo;
        info.PacketSize = (ushort)Marshal.SizeOf(typeof(stPlayerInfo));
        info.ID = _player.PlayerID;
        info.Password = _player.PlayerPassword;
        info.Gold = _player.PlayerGold;
        info.Token = _player.UpgradeToken;
        info.MainCharacters = ChangeToServerCharacter(_player.PlayerMainCharacters);
        info.Characters = ChangeToServerCharacter(_player.PlayerCharacters.Values.ToArray(), _player.PlayerCharacters.Count);
        info.ChsCount = (ushort)_player.PlayerCharacters.Count;

        Managers.Network.TcpSendMessage<stPlayerInfo>(info);
    }


    public Character[] GetStarterCharacters()
    {
        Character[] chs = new Character[6];

        var sChs = Managers.Resource.GetObjectByJson<StarterCharacters>(nameof(StarterCharacters));

        for (int i = 0; i < sChs.Characters.Length; i++)
        {
            chs[i] = sChs.Characters[i];
        }


        return chs;
    }



}
