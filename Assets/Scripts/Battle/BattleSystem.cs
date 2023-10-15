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
    private Queue<stBattleCharacterOrder> _orderQueue = new Queue<stBattleCharacterOrder>();
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
        Managers.Network.BattleOrdersCallBack -= PlayCharacterOrderByServer;
        Managers.Network.BattleOrdersCallBack += PlayCharacterOrderByServer;
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
        }
        if (_canSelect)
        {
            SelectCharacters();
        }
    }

    private void GetParticularInfo(stBattleParticularInfo info)
    {
        _infoArrived = true;
        Managers.Battle.ResetBattle();
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            DisableCharacterSelectTime();
            StopCoroutine(_selectCoroutine);
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
        for (int i = 0; i < info.Length; i++)
        {
            cTs[i].Id = info[i].CharacterID;
            cTs[i].IsPlayerCharacter = Managers.Battle.IsPlayer1 == info[i].IsPlayer1;
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
        SetCharactersOutline(_playerCharacters, OutlineState.None);
        SetCharactersOutline(_otherCharacters, OutlineState.None);

        SelectedCharacter = null;
        SelectAttackTarget = false;
    }


    private IEnumerator SetSelectTime(float time)
    {
        Managers.Network.Started = false;
        float selectTime = time;
        EnableCharacterSelectTime();
        while (selectTime > 0)
        {
            selectTime -= Time.deltaTime;
            _battleUI.SetSelectTimeText(selectTime);
            yield return null;
        }
        SetCharactersOutline(_playerCharacters, OutlineState.None);

        DisableCharacterSelectTime();
        Managers.Network.TcpSendMessage<stBattlePlayerOrder>(GetPlayerOrder());
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

    public stBattlePlayerOrder GetPlayerOrder()
    {
        stBattlePlayerOrder info = new stBattlePlayerOrder();
        info.MsgID = ServerData.MessageID.BattlePlayerOrderInfo;
        info.PacketSize = (ushort)Marshal.SizeOf(info);
        info.ID = Managers.Data.ID;
        info.RoomID = (ushort)Managers.Battle.RoomID;
        info.CharactersOrder = new stBattleMyCharacterOrder[3];
        for (int i = 0; i < _playerCharacters.Length; i++)
        {
            info.CharactersOrder[i] = _playerCharacters[i].GetCharacterBattleOrder();
            _playerCharacters[i].ResetCharacterBattleOrder();
        }
        return info;
    }

    private void PlayCharacterOrderByServer(stBattleOrdersInfo actionInfo) // 한 턴의 정보를 받아 턴 진행
    {
        for (int i = 0; i < actionInfo.Order.Length; i++)
        {
            _orderQueue.Enqueue(actionInfo.Order[i]); // 캐릭터의 행동 정보를 큐에 저장
            if (actionInfo.Order[i].IsMyCharacter)
            {
                _playerCharacters[actionInfo.Order[i].CharacterIndex].OrderState = actionInfo.Order[i].State;
            }
            else
            {
                _otherCharacters[actionInfo.Order[i].CharacterIndex].OrderState = actionInfo.Order[i].State;
            }
        }
        stBattleCharacterOrder order = _orderQueue.Dequeue(); // 첫 번째 행동 Dequeue
        SetOrder(order); // 첫 번째의 캐릭터 행동 지정
        SetNextTurn(actionInfo.GameState); // 서버에서 수신한 게임 상태에 따라 다음 턴 상태 지정
    }
    private void SetOrder(stBattleCharacterOrder order) // 캐릭터 행동 지정
    {
        if (order.IsMyCharacter)
        {
            _playerCharacters[order.CharacterIndex].SetCharacterOrder(order, _otherCharacters[order.TargetIndex], SetNextOrder);
        }
        else
        {
            _otherCharacters[order.CharacterIndex].SetCharacterOrder(order, _playerCharacters[order.TargetIndex], SetNextOrder);
        }
    }
    private void SetNextOrder() // 다음 캐릭터의 행동
    {
        if (_orderQueue.Count > 0) // 다음 행동할 캐릭터가 있을 경우 
        {
            stBattleCharacterOrder order = _orderQueue.Dequeue(); 
            SetOrder(order); // 캐릭터 행동
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _nextTurn?.Invoke(); // 더이상 행동할 캐릭터가 없을 경우 턴 마무리
            });
        }
    }
    private void SetNextTurn(int gameState) // 다음 턴 상태 지정
    {
        _nextTurn = null;
        switch (gameState)
        {
            case (int)GameState.ContinueSelect: // 게임 계속 진행
                _nextTurn += () => StartCoroutine(SetSelectTime(PlayerData.Constants.SelectTime)); 
                break;
            case (int)GameState.Player1Win: // 플레이어 1 승리 팝업 생성 및 게임 종료
                _nextTurn += () => Managers.UI.ShowPopupUI<UI_BattleResultPopup>().SetText(!Managers.Battle.IsPlayer1);
                break;
            case (int)GameState.Player2Win: // 플레이어 2 승리 팝업 생성 및 게임 종료
                _nextTurn += () => Managers.UI.ShowPopupUI<UI_BattleResultPopup>().SetText(Managers.Battle.IsPlayer1);
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
