using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public LogicActor bindLogicActor = null;
    //public Animator animator;
    // Start is called before the first frame update
    public bool isLocal = false;

    public Vector2 targetWorldPos;
    public void Start()
    {
        //animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (isLocal)
        {
            //只有 特效 不是local
            return;
        }

        if(bindLogicActor == null)
        {
            //Debug.Log("爆炸了，异常");
            return;
        }

        //float curTickRate = 1/Time.deltaTime;
        //Debug.Log("帧率" + curTickRate);
        //if (bindLogicActor.isNoMove())
        //{
        //    return;
        //}
        //float dt = 100;
        //v = 20/s
        float rv = bindLogicActor.speed * Time.deltaTime;
        targetWorldPos = new Vector2(bindLogicActor.Pos.x * 0.001f, bindLogicActor.Pos.y * 0.001f);
        //Debug.Log("target:" + bindLogicActor.Pos);
        Vector2 diff = (targetWorldPos - (Vector2)transform.position);
        //transform.position = targetWorldPos;
        //if(diff.magnitude < 1e-4f)
        //{
        //    animator.SetBool("moving",false);
        //    //Debug.Log("budong");
        //    return;
        //}

        //animator.SetBool("moving", true);

        if (bindLogicActor.GetLogicDTime() != 0)
        {
            //计算 应该移多远 才能在下一逻辑帧抵达目标位置
            float rate = Time.deltaTime * 1000f / bindLogicActor.GetLogicDTime();
            float shouldMoveLen = bindLogicActor.LogicDiff.magnitude * 0.001f * rate;
            float actualMoveLen = rv;
            //如果 移动太慢了 需要加速 
            if (shouldMoveLen > actualMoveLen * 1.5f)
            {
                rv *= 1.5f;
            }
        }


        if (diff.magnitude < rv)
        {
            //Debug.Log("shezhi");
            transform.position = targetWorldPos;
        }
        else
        {
            //Debug.Log("移动");
            Vector3 mvDir = diff.normalized * rv;
            transform.position += mvDir;
        }


        //线性插值
        //float svrTickInterval = GameMain.GetInstance().logicManager.LogicCurTickTime - GameMain.GetInstance().logicManager.LogicLastTickTime;
        //float rate = svrTickInterval / (1.0f / GameMain.GetInstance().logicManager.LogicTickRate);
        //Vector2 lastWorldPos = new Vector2(bindLogicActor.LastPos.x * 0.001f, bindLogicActor.LastPos.y * 0.001f);

        //targetWorldPos = new Vector2(bindLogicActor.Pos.x * 0.001f, bindLogicActor.Pos.y * 0.001f);
        //transform.position = Vector3.Lerp(transform.position,targetWorldPos,0.3f);
        //Vector2 moveDir = (targetWorldPos - lastWorldPos) / 6;
        //float diff = (targetWorldPos - new Vector2(transform.position.x, transform.position.y)).magnitude;
        //transform.position += moveDir;
        //bindLogicActor.Pos;
        //bindLogicActor.Rotate;
        //transform.position = Vector3.Lerp(transform.position, targetWorldPos, 0.3f);


    }


    
   



    public List<Actor> contactActors = new List<Actor>();
    public void Init(LogicActor logicActor)
    {
        this.bindLogicActor = logicActor;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Actor other = collision.GetComponent<Actor>();
        if(other == null)
        {
            return;
        }
        contactActors.Add(other);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Actor other = collision.GetComponent<Actor>();
        if (other == null)
        {
            return;
        }
        if (contactActors.Contains(other))
        {
            contactActors.Remove(other);
        }
    }

    public void OnStateChange()
    {
        //当逻辑改变 底层还在走时 怎么切？
        //if (bindLogicActor.state == LogicActor.eState.IDLE)
        //{
        //    animator.SetTrigger("idle");
        //}
        //else if (bindLogicActor.state == LogicActor.eState.MOVE)
        //{
        //    animator.SetTrigger("move");
        //}
    }

}
