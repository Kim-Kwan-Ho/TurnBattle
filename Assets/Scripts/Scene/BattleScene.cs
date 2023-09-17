using ServerData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BattleScene : BaseScene
{
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        Managers.Battle.SetBattleObjects();
        SceneType = Define.Scene.Battle;
        
        stBattleReadyInfo ready = new stBattleReadyInfo
        {
            MsgID = ServerData.MessageID.BattleReadyInfo,
            PacketSize = (ushort)Marshal.SizeOf(typeof(stBattleReadyInfo)),
            ID = Managers.Data.ID,
            RoomID = Managers.Data.RoomID,
            IsReady = true

        };
        Managers.Network.TcpSendMessage<stBattleReadyInfo>(ready);


        return true;
    }



}
