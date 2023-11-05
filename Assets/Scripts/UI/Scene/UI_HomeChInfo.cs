using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_HomeChInfo : UI_Base
{
    enum Images
    {
        ChImage,
    }

    enum Texts
    {
        HpText,
        AtkText,
        DfText,
        SpdText,
        LevelText
    }


    enum GameObjects
    {
        Selected,
        Using,
    }

    private bool _selected;
    private Character _character;
    public Character GetCharacter()
    { return _character; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));

        SetMainCharacter();
        gameObject.BindEvent(SelectCharacter);
        gameObject.BindEvent(() => GetComponentInParent<UI_CHSelect>().ChangeSelect(this));

        return true;
    }

    public void SetCharacter(Character character, bool selected = false, bool mainCh = false) // 게임 시작하면서 불릴 것
    {
        _character = character;
        GetText((int)Texts.AtkText).text = "ATK : " + character.ChDamage.ToString();
        GetText((int)Texts.HpText).text = "HP : " + character.ChHp.ToString();
        GetText((int)Texts.DfText).text = "DF : " + character.ChArmor.ToString();
        GetText((int)Texts.SpdText).text = "SPD : " + character.ChSpd.ToString();
        GetText((int)Texts.LevelText).text = "+" + character.ChLevel.ToString();
        _selected = selected;
        GetObject((int)GameObjects.Selected).gameObject.SetActive(_selected);
        GetImage((int)Images.ChImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Characters/Main/{character.ChID}");
        // 전투력 계산은 나중에
    }

    public void SelectCharacter()
    {
        _selected = !_selected;
        GetObject((int)GameObjects.Selected).gameObject.SetActive(_selected);
    }

    public void SetMainCharacter(bool main = false)
    {
        GetObject((int)GameObjects.Using).SetActive(main);
    }

}
