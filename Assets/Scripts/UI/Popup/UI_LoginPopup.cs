using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Data;
using static Define;

public class UI_LoginPopup : UI_Popup
{
    enum Buttons
    {
        LoginButton,
        RegisterButton,
    }

    enum GameObjects
    {
        IDInput,
        PSWInput,
    }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        GetButton((int)Buttons.LoginButton).gameObject.BindEvent(() => LoginRegister(true));
        GetButton((int)Buttons.RegisterButton).gameObject.BindEvent(() => LoginRegister(false));
        Managers.Network.LoginRegisterCallBack = null;
        Managers.Network.LoginRegisterCallBack += GetLoginRegisterResult; // 로그인 or 회원가입 결과 반환
        return true;
    }
    private void LoginRegister(bool isLogin) // false => Register, true => Login
    {
        string id = GetObject((int)GameObjects.IDInput).GetComponent<TMP_InputField>().text;
        string psw = GetObject((int)GameObjects.PSWInput).GetComponent<TMP_InputField>().text;

        stLoginRegister loginRegister = CreateLoginRegisterObject(id, psw, isLogin);

        Managers.Network.TcpSendMessage<stLoginRegister>(loginRegister);
    }
    private stLoginRegister CreateLoginRegisterObject(string id, string psw, bool isLogin) // 전송할 정보 생성
    {
        return new stLoginRegister
        {
            MsgID = MessageID.LoginRegister,
            PacketSize = (ushort)Marshal.SizeOf(typeof(stLoginRegister)),
            IsLogin = isLogin,
            Succeed = false,
            ID = id,
            Password = psw
        };
    }

    private void GetLoginRegisterResult(bool succeed, string msg) // 로그인 or 회원가입 결과 반환
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (succeed) // 로그인 성공했을 경우
            {
                Managers.UI.CloseAllPopupUI();
                Managers.Scene.ChangeScene(Scene.Base, true);
            }
            else // 로그인 또는 회원가입 성공유무 반환
            {
                Managers.UI.ShowPopupUI<UI_MessagePopup>().SetText(msg); // 받은 msg 내용으로 메시지 팝업 생성
            }
        });
    }
}
