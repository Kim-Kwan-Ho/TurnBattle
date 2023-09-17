using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ServerData;
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

        return true;
    }



    private void LoginRegister(bool isLogin) // false => Register, true => Login
    {
        string id = GetObject((int)GameObjects.IDInput).GetComponent<TMP_InputField>().text;
        string psw = GetObject((int)GameObjects.PSWInput).GetComponent<TMP_InputField>().text;

        stLoginRegister loginRegister = CreateLoginRegisterObject(id, psw, isLogin);
        Managers.Network.LoginRegisterCallBack = null;
        Managers.Network.LoginRegisterCallBack += GetResult; // 실패시 메시지 반환
        Managers.Network.TcpSendMessage<stLoginRegister>(loginRegister);
    }

    private stLoginRegister CreateLoginRegisterObject(string id, string psw, bool isLogin)
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

    private void GetResult(bool succeed, string msg = "")
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (succeed)
            {
                Managers.UI.CloseAllPopupUI();
                Managers.Scene.ChangeScene(Scene.Base, true);
            }
            else
            {
                Managers.UI.ShowPopupUI<UI_MessagePopup>().SetText(msg);
            }
        });
    }
}
