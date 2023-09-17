using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using ServerData;
using PlayerData;
using PimDeWitte.UnityMainThreadDispatcher;

public class BattleSystem : MonoBehaviour
{
    [Header("Characters")]
    private MyCharacterController[] _playerCharacters = new MyCharacterController[3];
    private OtherCharacterController[] _otherCharacters = new OtherCharacterController[3];

    [Header("Select Order")]
    private bool _canSelect = false;
    private Coroutine _selectCoroutine = null;
    public static MyCharacterController SelectedCharacter = null;
    public static bool SelectAttackTarget = false;

    [Header("ServerOrder")]
    private Queue<stBattleOrder> _orderQueue = new Queue<stBattleOrder>();
    private Action _nextTurn = null;

    [Header("UI")]
    private UI_BattlePopup _battleUI = null;

    [Header("Particular Info")] 
    private bool _infoArrived = false;
    private void Awake()
    {
        _playerCharacters = GetComponentsInChildren<MyCharacterController>();
        _otherCharacters = GetComponentsInChildren<OtherCharacterController>();
        _battleUI = Managers.UI.ShowPopupUI<UI_BattlePopup>();
    }

    private void Start()
    {
        Managers.Network.BattleCallBack -= PlayCharacterOrderByServer;
        Managers.Network.BattleCallBack += PlayCharacterOrderByServer;
        Managers.Network.BattleParticularCallBack -= GetParticularInfo;
        Managers.Network.BattleParticularCallBack += GetParticularInfo;

    }

    private void Update()
    {
        if (_infoArrived)
            return;
        
        if (StartSelectOrder())
        {
            _selectCoroutine = StartCoroutine(SetSelectTime(PlayerData.Constants.SelectTime));
            Managers.Network.Started = false;
        }
        if (_canSelect)
        {
            SelectCharacters();
        }
    }

    private void GetParticularInfo(stBattleParticularInfo info)
    {
        _infoArrived = true;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            DisableCharacterSelectTime();
            _nextTurn = null;
            string text = "";
            if (info.ParticularInfo == (ushort)ParticularInfo.Surrender)
            {
                text = "Other Player Surrendered";
            }
            else
            {
                text = "Other Player has left the game";
            }
            Managers.UI.ShowPopupUI<UI_BattleResultPopup>().SetText(text);
        });

    }

    public void SetPlayerCharacters(stCharacterInfo[] characters)
    {
        for (int i = 0; i < _playerCharacters.Length; i++)
        {
            _playerCharacters[i].SetCharacter(characters[i], (ushort)i);
        }
        SetCharactersOutline(_playerCharacters, OutlineState.None);
    }

    public void SetOtherPlayerCharacters(stCharacterInfo[] characters)
    {
        for (int i = 0; i < _otherCharacters.Length; i++)
        {
            _otherCharacters[i].SetCharacter(characters[i], (ushort)i);
        }
        SetCharactersOutline(_otherCharacters, OutlineState.None);
    }

    public void SetCharacterTurn(stBattleTurnInfo[] info)
    {
        CharacterTurn[] cTs = new CharacterTurn[6];
        for (int i = 0; i < info.Length ; i++)
        {
            cTs[i].Id = info[i].Id;
            cTs[i].IsPlayerCharacter = Managers.Data.IsPlayer1 == info[i].IsPlayer1;
        }

        _battleUI.SetBattleOrderImages(cTs);
    }

    private void EnableCharacterSelectTime()
    {
        _canSelect = true;
        SetCharactersOutline(_playerCharacters, OutlineState.SelectCharacter);
        SetCharactersOutline(_otherCharacters, OutlineState.None);
    }

    private void DisableCharacterSelectTime()
    {
        _canSelect = false;
        StopCoroutine(_selectCoroutine);
        SetCharactersOutline(_playerCharacters, OutlineState.None);
        SetCharactersOutline(_otherCharacters, OutlineState.None);

        SelectedCharacter = null;
        SelectAttackTarget = false;
    }


    private IEnumerator SetSelectTime(float time)
    {
        float selectTime = time;
        EnableCharacterSelectTime();
        while (selectTime > 0)
        {
            selectTime -= Time.deltaTime;
            _battleUI.SetSelectTimeText(selectTime);
            yield return null;
        }

        DisableCharacterSelectTime();
        Managers.Network.TcpSendMessage<stBattleMyOrder>(GetPlayerOrder());
        selectTime = 0;
    }
    private void SelectCharacters()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!SelectAttackTarget)
            {
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 5, 1 << 7);
                if (hit.collider != null)
                {
                    SelectedCharacter = hit.collider.GetComponent<MyCharacterController>();
                    SelectedCharacter.SelectCharacter();
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 5, 1 << 8);
                if (hit.collider != null)
                {
                    SelectedCharacter.SelectTarget(hit.collider.GetComponent<OtherCharacterController>());
                }
                else
                {
                    SelectedCharacter.SelectState = CharacterState.None;
                    SelectedCharacter.DeSelectCharacter();
                    SelectedCharacter = null;
                }
                SelectAttackTarget = false;
                SetCharactersOutline(_otherCharacters, OutlineState.None);
            }
        }
    }

    public stBattleMyOrder GetPlayerOrder()
    {
        stBattleMyOrder info = new stBattleMyOrder();
        info.MsgID = ServerData.MessageID.BattleMyOrder;
        info.PacketSize = (ushort)Marshal.SizeOf(info);
        info.ID = Managers.Data.ID;
        info.RoomID = Managers.Data.RoomID;
        info.CharactersOrder = new stBattleMyCharacterOrder[3];
        for (int i = 0; i < _playerCharacters.Length; i++)
        {
            info.CharactersOrder[i] = _playerCharacters[i].GetCharacterBattleOrder();
            _playerCharacters[i].ResetCharacterBattleOrder();
        }
        return info;
    }

    private void PlayCharacterOrderByServer(stBattleInfo battleInfo)
    {
        for (int i = 0; i < battleInfo.Order.Length; i++)
        {
            _orderQueue.Enqueue(battleInfo.Order[i]);
            if (battleInfo.Order[i].IsMyCharacter)
            {
                _playerCharacters[battleInfo.Order[i].CharacterIndex].OrderState = battleInfo.Order[i].State;
            }
            else
            {
                _otherCharacters[battleInfo.Order[i].CharacterIndex].OrderState = battleInfo.Order[i].State;
            }
        }
        stBattleOrder order = _orderQueue.Dequeue();
        SetOrder(order);
        SetNextTurn(battleInfo.GameState);
    }

   
    private void SetNextOrder()
    {
        if (_orderQueue.Count > 0)
        {
            stBattleOrder order = _orderQueue.Dequeue();
            SetOrder(order);
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _nextTurn?.Invoke();
            });
        }
    }
    private void SetOrder(stBattleOrder order)
    {
        if (order.IsMyCharacter)
        {
            _playerCharacters[order.CharacterIndex].SetCharacterOrder(order, _otherCharacters[order.TargetIndex] ,SetNextOrder);
        }
        else
        {
            _otherCharacters[order.CharacterIndex].SetCharacterOrder(order, _playerCharacters[order.TargetIndex] ,SetNextOrder);
        }
    }
    private void SetNextTurn(int gameState)
    {
        _nextTurn = null;
        switch (gameState)
        {
            case (int)GameState.ContinueSelect:
                _nextTurn += () => StartCoroutine(SetSelectTime(PlayerData.Constants.SelectTime));
                break;
            case (int)GameState.Player1Win:
                _nextTurn += () => Managers.UI.ShowPopupUI<UI_BattleResultPopup>().SetText(!Managers.Data.IsPlayer1);
                break;
            case (int)GameState.Player2Win:
                _nextTurn += () => Managers.UI.ShowPopupUI<UI_BattleResultPopup>().SetText(Managers.Data.IsPlayer1);
                break;
        }
    }

    private void SetCharactersOutline(BaseController[] characters, OutlineState state)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetCharacterOutline(state);
        }
    }

   

    public void OnOffPlayerOutline(bool on)
    {
        if (on)
        {
            SetCharactersOutline(_playerCharacters, OutlineState.SelectCharacter);
        }
        else
        {
            SetCharactersOutline(_playerCharacters, OutlineState.None);
        }
    }
    private bool StartSelectOrder()
    {
        return Managers.Network.Started && !_canSelect;
    }
}
