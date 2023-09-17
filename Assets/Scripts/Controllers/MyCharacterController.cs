using System;
using System.Collections;
using System.Collections.Generic;
using PlayerData;
using ServerData;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyCharacterController : BaseController
{
    private UInt16 _targetIndex = 0;

    private BattleSystem _battleSystem = null;

    protected override void Awake()
    {
        base.Awake();
        ResetCharacterBattleOrder();
    }

    protected override void Start()
    {
        _battleSystem = GetComponentInParent<BattleSystem>();
            
    }

    public void SelectCharacter()
    {
        _characterUI.ActiveButton();
        _battleSystem.OnOffPlayerOutline(false);
        SetCharacterOutline(OutlineState.CharacterSelected);
    }

    public override void DeSelectCharacter()
    {
        _battleSystem.OnOffPlayerOutline(true);
    }

    public void SelectTarget(OtherCharacterController otherCharacter)
    {
        if (_selectState == CharacterState.SetAttackTarget)
        {
            _targetIndex = otherCharacter.CharacterIndex;
            _selectState = CharacterState.Attack;
            _characterUI.SetAttackTarget(otherCharacter.CharacterId);
        }
        _battleSystem.OnOffPlayerOutline(true);
    }

    public stBattleMyCharacterOrder GetCharacterBattleOrder()
    {
        stBattleMyCharacterOrder order = new stBattleMyCharacterOrder();
        order.CharacterIndex = _characterIndex;
        order.TargetIndex = _targetIndex;
        order.State = (ushort)_selectState;
        ResetCharacterBattleOrder();
        _characterUI.ResetStateImage();
        return order;
    }

    public void ResetCharacterBattleOrder()
    {
        _targetIndex = 0;
        _selectState = CharacterState.None;
    }

}
