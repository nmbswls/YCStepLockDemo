using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public int localPid = 0;// 0-8 本地玩家id


    public void Init(int localPid)
    {
        this.localPid = localPid;

        ActorFactory.CreateNewActor("pawn");
        ActorFactory.CreateNewActor("pawn");

        
        GameMain.GetInstance().plyCtrl.ChangePawn(GameMain.GetInstance().logicManager.GetActor(localPid));
    }
    public void Update(int dtime)
    {
        
    }

    
}
