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
        Managers.Battle.MakeBattleRoom();
        SceneType = Define.Scene.Battle;
        
        stBattleReady ready = new stBattleReady
        {
            MsgID = ServerData.MessageID.BattleReady,
            PacketSize = (ushort)Marshal.SizeOf(typeof(stBattleReady)),
            ID = Managers.Data.ID,
            RoomID = (ushort)Managers.Battle.RoomID,
            IsReady = true

        };
        Managers.Network.TcpSendMessage<stBattleReady>(ready);


        return true;
    }

        

}
