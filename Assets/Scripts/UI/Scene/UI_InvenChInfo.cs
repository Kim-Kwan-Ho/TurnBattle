using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_InvenChInfo : UI_Base
{
    private Character _character = new Character();
    enum GameObjects
    {
        Selected
    }
    enum Images
    {
        ChImage,
        ChTypeImage
    }

    enum Texts
    {
        ChNameText,
        ChLevelText,
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        gameObject.BindEvent(() => Managers.UI.ShowPopupUI<UI_EquipmentPopup>().SetCharacterEquip(_character));
        
        GetObject((int)GameObjects.Selected).SetActive(false);


        return true;
    }

    public void SetCharacter(Character character, bool isMainCh = false)
    {
        _character = character;
        GetObject((int)GameObjects.Selected).SetActive(isMainCh);
        GetText((int)Texts.ChLevelText).text = "Lv." + character.ChLevel.ToString();
        GetText((int)Texts.ChNameText).text = character.ChID.ToString();
    }

}
