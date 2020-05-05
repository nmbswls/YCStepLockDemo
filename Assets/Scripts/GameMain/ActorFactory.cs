using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFactory
{

    public static LogicActor CreateNewActor(string name)
    {
        LogicActor logicActor = new LogicActor();

        Actor actor = null;
        if (name == "pawn")
        {
            GameObject prefab = Resources.Load("Pawn") as GameObject;
            GameObject inst = GameObject.Instantiate(prefab);
            actor = inst.GetComponent<Actor>();
            actor.Init(logicActor);
            logicActor.viewActor = actor;
        }
        if (actor)
        {
            GameMain.GetInstance().viewManager.GameActors.Add(actor);
        }
        return logicActor;
    }

}
