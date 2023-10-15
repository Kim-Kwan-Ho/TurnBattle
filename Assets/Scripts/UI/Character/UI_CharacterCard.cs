using System;
using System.Collections;
using System.Collections.Generic;
using PlayerData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CharacterCard : UI_Base
{
    enum Texts
    {
        HpText,
        DamageText,
        ArmorText,
        SpeedText
    }

    enum Images
    {
        CharacterImage,
    }



    [HideInInspector] public Toggle SelectToggle = null;
    public event Action<UI_CharacterCard> OnClick = null;
    private Character _cardCharacter = new Character();

    public Character CardCharacter
    {
        get { return _cardCharacter; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        SelectToggle = GetComponent<Toggle>();
        // 카드 생성과 동시에 상위 객체에서 Onclick Action에 SelectCard 함수 추가
        SelectToggle.onValueChanged.AddListener(click => OnClick?.Invoke(this));  
        return true;
    }
    public void SetCardInfo(Character character)
    {
        _cardCharacter = character;
        GetImage((int)Images.CharacterImage).sprite =
            Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{_cardCharacter.ChID}");
        GetText((int)Texts.HpText).text = "Hp : " + _cardCharacter.ChHp.ToString();
        GetText((int)Texts.ArmorText).text = "Def : " + _cardCharacter.ChArmor.ToString();
        GetText((int)Texts.DamageText).text = "Atk : " + _cardCharacter.ChDamage.ToString();
        GetText((int)Texts.SpeedText).text = "Spd : " + _cardCharacter.ChSpd.ToString();
    }
}
