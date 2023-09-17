using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MessagePopup : UI_Popup
{
    enum Texts
    {
        MessageText
    }

    enum Buttons
    {
        ConfirmButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(ClosePopupUI);
        return true;
    }

    public void SetText(string text)
    {
        GetText((int)Texts.MessageText).text = text;
    }



}
