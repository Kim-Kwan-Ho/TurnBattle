using System;
using System.Collections;
using System.Collections.Generic;
using PlayerData;
using UnityEngine;
using ServerData;
using Unity.VisualScripting;
using PimDeWitte.UnityMainThreadDispatcher;

public class BaseController : MonoBehaviour
{
    [Header("Components")]
    private SpriteRenderer _spr = null;
    private Animator _anim = null;

    [Header("Character Stats")]
    protected CharacterState _selectState = CharacterState.None; // 조작용
    private stCharacterInfo _characterInfo;
    protected UInt16 _characterIndex = 0;
    private int _curHp = 0;
    private UInt16 _id = 0;

    [Header("Order")]
    private UInt16 _orderState = 1;
    public UInt16 OrderState { set { _orderState = value; } }

    [Header("Movement")]
    private Vector3 _startPos = Vector3.zero;
    private float _moveOffSetY = 2;
    private float _moveTime = 1;

    [Header("UI")]
    protected UI_CharacterBattle _characterUI = null;
    protected CharacterOutline _outline = null;

    
    public UInt16 CharacterIndex { get { return _characterIndex; } }
    public UInt16 CharacterId { get { return _id; } }
    public CharacterState SelectState
    {
        get { return _selectState; }
        set { _selectState = value; }
    }



    protected virtual void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        _startPos = transform.position;
        _characterUI = Managers.Resource.Instantiate("UI/Battle/UI_CharacterBattle", transform)
            .GetComponent<UI_CharacterBattle>();
        _outline = Managers.Resource.Instantiate("Battle/CharacterOutline", transform).GetComponent<CharacterOutline>();

    }

    protected virtual void Start()
    {

    }

    public virtual void DeSelectCharacter()
    {

    }

    public virtual void SetCharacter(stCharacterInfo info, UInt16 index)
    {
        _characterIndex = index;
        _characterInfo = info;
        _curHp = info.ChHp;
        _id = info.ChID;
        _spr.sprite = Managers.Resource.Load<Sprite>($"Sprites/Characters/Battle/{_characterInfo.ChID}");

    }

    public virtual void SetCharacterOrder(stBattleCharacterOrder order, BaseController orderTarget ,Action nextAction)
    {
        if (IsDeath())
        {
            nextAction?.Invoke();
            return;
        }
        switch (order.State)
        {

            case (ushort)CharacterState.Attack:
                if (!orderTarget.IsDeath())
                    AttackTarget(orderTarget, nextAction);
                else
                    nextAction?.Invoke();

                break;
            default:
                nextAction?.Invoke();
                break;

        }
    }

    private void AttackTarget(BaseController target, Action nexAction)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            StartCoroutine(MoveToTarget(target, nexAction));
        });
    }


    private IEnumerator MoveToTarget(BaseController target, Action nextAction)
    {
        Vector3 targetPos = target.gameObject.transform.position;
        float time = 0;
        if (targetPos.y > 0)
        {
            targetPos -= new Vector3(0, _moveOffSetY, 0);
        }
        else
        {
            targetPos += new Vector3(0, _moveOffSetY, 0);
        }

        while (time < _moveTime)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(_startPos, targetPos, time / _moveTime);
            yield return null;
        }
        target.TakeHit(_characterInfo.ChDamage);

        yield return StartCoroutine(MoveToStartPos(nextAction));
    }

    private IEnumerator MoveToStartPos(Action nextAction)
    {
        Vector3 curPos = transform.position;
        float time = 0;
        while (time < _moveTime)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(curPos, _startPos, time / _moveTime);
            yield return null;
        }

        nextAction?.Invoke();
        yield break;
    }
    public void TakeHit(UInt16 amount)
    {
        if (IsDeath())
            return;
        
        UInt16 damage = (ushort)(amount - _characterInfo.ChArmor);
        if (_orderState == (ushort)CharacterState.Defense)
        {
            damage /= 2;
        }

        CreateDamagePopup(damage);


        if (_curHp <= damage)
        {
            _curHp = 0;
            _orderState = (ushort)CharacterState.Death;
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            _curHp -= damage;
        }
        _characterUI.SetHpBar((float)_curHp / _characterInfo.ChHp);
    }

    protected bool IsDeath()
    {
        if (_curHp <= 0)
            return true;
        return false;
    }

    private void CreateDamagePopup(int damage)
    {
        GameObject go = Managers.Resource.Instantiate("Battle/DamagePopup");
        go.transform.position = transform.position + Vector3.up * 3;
        go.GetComponent<DamagePopup>().SetText(damage.ToString());
    }

    public void SetCharacterOutline(OutlineState state)
    {
        if (IsDeath())
        {
            _outline.SetOutlineColor(OutlineState.None);
        }
        else
        {
            _outline.SetOutlineColor(state);
        }
    }
}
