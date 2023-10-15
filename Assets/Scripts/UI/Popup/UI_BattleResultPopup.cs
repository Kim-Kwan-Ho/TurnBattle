using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayerData;
using ServerData;
using UnityEngine;

public class UI_BattleResultPopup : UI_Popup
{
    enum Buttons
    {
        HomeButton
    }

    enum Texts
    {
        ResultText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(GoHome);

        return true;
    }

    public void SetText(bool win)
    {
        GetText((int)Texts.ResultText).text = win ? "Win" : "Lose";
    }
    public void SetText(string text)
    {
        GetText((int)Texts.ResultText).text = text;
    }
    public void GoHome()
    {

        //Managers.Battle.ResetBattle();
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.ChangeScene(Define.Scene.Base, true);
    }

}
