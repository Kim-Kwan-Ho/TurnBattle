using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_CharacterBattle : UI_Base
{
    enum Images
    {
        HpImage,
        TargetImage
    }

    enum GameObjects
    {
        Buttons,
        Attack,
        Defense
    }
    enum Buttons
    {
        DefenseButton,
        AttackButton,
        CancelButton
    }

    private BaseCharacterController _chController = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _chController = GetComponentInParent<BaseCharacterController>();
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        if (_chController is not MyCharacterController)
        {
            DeActiveButton();
        }
        else
        {
            _chController = GetComponentInParent<MyCharacterController>();
        }
        
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(CancelButton);
        GetButton((int)Buttons.AttackButton).gameObject.BindEvent(() => SetCharacterState(CharacterState.SetAttackTarget));
        GetButton((int)Buttons.DefenseButton).gameObject.BindEvent(() => SetCharacterState(CharacterState.Defense));
        GetObject((int)GameObjects.Attack).SetActive(false);
        GetObject((int)GameObjects.Defense).SetActive(false);
        GetObject((int)GameObjects.Buttons).SetActive(false);

        return true;
    }


    private void SetCharacterState(CharacterState state)
    {
        _chController.SelectState = state;
        switch (state)
        {
            case CharacterState.SetAttackTarget:
                BattleSystem.SelectAttackTarget = true;
                GetObject((int)GameObjects.Buttons).SetActive(false);
                ResetStateImage();
                break;
            case CharacterState.Defense:
                BattleSystem.SelectedCharacter = null;
                GetObject((int)GameObjects.Attack).SetActive(false);
                GetObject((int)GameObjects.Defense).SetActive(true);
                DeActiveButton();
                break;
        }

    }

    public void SetHpBar(float ratio)
    {
        GetImage((int)Images.HpImage).fillAmount = ratio;
    }

    public void ActiveButton()
    {
        GetObject((int)GameObjects.Buttons).SetActive(true);
    }

    public void CancelButton()
    {
        BattleSystem.SelectedCharacter = null;
        DeActiveButton();
    }
    public void DeActiveButton()
    {
        _chController.DeSelectCharacter();
        GetObject((int)GameObjects.Buttons).SetActive(false);
    }

    public void SetAttackTarget(ushort id)
    {
        GetObject((int)GameObjects.Attack).SetActive(true);
        GetImage((int)Images.TargetImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/Characters/Battle/{id}");
    }

    public void ResetStateImage()
    {
        GetObject((int)GameObjects.Defense).SetActive(false);
        GetObject((int)GameObjects.Attack).SetActive(false);
        GetObject((int)GameObjects.Buttons).SetActive(false);
    }

}
