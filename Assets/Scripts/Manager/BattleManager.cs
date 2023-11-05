using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
public class BattleManager 
{
    private BattleSystem _battleSystem;
    private UInt16? _roomID = null;
    private bool _isPlayer1 = false;
    private stBattleRoomInfo _roomInfo = new stBattleRoomInfo();

    public UInt16? RoomID { get { return _roomID; } }

    public bool IsPlayer1 { get { return _isPlayer1; } }



    public void SetBattleRoomInfo(stBattleRoomInfo info)
    {
        _roomInfo = info;
        _roomID = _roomInfo.RoomID;
        _isPlayer1 = _roomInfo.IsPlayer1;

    }

    public void MakeBattleRoom(BattleSystem battleSystem)
    {
        
        _battleSystem = battleSystem;
        _battleSystem.SetPlayerCharacters(_roomInfo.PlayerCharacters);
        _battleSystem.SetOtherPlayerCharacters(_roomInfo.OtherCharacters);
        _battleSystem.SetCharacterTurn(_roomInfo.BattleTurn);
    }


    public void ResetBattle()
    {
        _roomID = null;
        _isPlayer1 = false;
        _battleSystem = null;
        Managers.Network.ResetBattle();
    }




}
