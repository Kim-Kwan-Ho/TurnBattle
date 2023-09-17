using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerData;
using PlayerData;
public class BattleManager 
{
    public BattleSystem BattleSystem;
    public stBattleRoomCharactersInfo RoomCharactersInfo = new stBattleRoomCharactersInfo();

    public void SetBattleObjects()
    {
        GameObject go = Managers.Resource.Instantiate(("Battle/BattleSystem"));
        Managers.Data.RoomID = RoomCharactersInfo.RoomID;
        Managers.Data.IsPlayer1 = RoomCharactersInfo.IsPlayer1;
        BattleSystem = go.GetComponent<BattleSystem>();
        BattleSystem.SetPlayerCharacters(RoomCharactersInfo.PlayerCharacters);
        BattleSystem.SetOtherPlayerCharacters(RoomCharactersInfo.OtherCharacters);
        BattleSystem.SetCharacterTurn(RoomCharactersInfo.BattleTurn);

    }


    public void ResetBattle()
    {
        Managers.Data.RoomID = 0;
        Managers.Data.IsPlayer1 = false;
        Managers.Network.Matched = false;
        Managers.Network.Started = false;
        Managers.Network.BattleCallBack = null;
        BattleSystem = null;
    }




}
