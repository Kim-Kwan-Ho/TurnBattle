using System.Collections;
using System.Collections.Generic;
using PlayerData;
using UnityEngine;

public class UI_EquipmentPopup : UI_Popup
{
    enum Buttons
    {
        EqHeadButton,
        EqArmorButton,
        EqWeaponButton,
        EqShoeButton,
        EqGloveButton,
        ExitButton
    }

    enum Images
    {
        HeadImage,
        ArmorImage,
        WeaponImage,
        ShoeImage,
        GloveImage
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(ClosePopupUI);
        return true;
    }

    public void SetCharacterEquip(Character character)
    {
        //GetImage((int)Images.ArmorImage).sprite =
        //    Managers.Resource.Load<Sprite>($"Sprites/Equipment/Armor/{character.ChEqArmor.ItemName}");
        /*
        GetImage((int)Images.HeadImage).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Equipment/Head/{character.ChEqHead.ItemName}");

        GetImage((int)Images.GloveImage).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Equipment/Glove/{character.ChEqGlove.ItemName}");

        GetImage((int)Images.ShoeImage).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Equipment/Shoe/{character.ChEqShoe.ItemName}");

        GetImage((int)Images.WeaponImage).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Equipment/Weapon/{character.ChEqWeapon.ItemName}");

        */
    }


}
