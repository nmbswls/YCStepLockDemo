using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public LogicActor bindLogicActor = null;
    public Animator animator;
    // Start is called before the first frame update
    public bool isLocal = false;

    //public Vector2 targetWorldPos;


    public Vector2 targetVec = Vector2.zero;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }


    public enum eMoveState{
        IDLE,
        MOVE,
    }

    public eMoveState LastState = eMoveState.IDLE;
    public eMoveState NowState = eMoveState.IDLE;
    public float guanxingTimer = 0;

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

        
        

        Vector2 LogicWorldPos = new Vector2(bindLogicActor.Pos.x * 0.001f, bindLogicActor.Pos.y * 0.001f);
        Vector2 LogicDiff = (LogicWorldPos - (Vector2)transform.position);

        

        //如果偏移太远了
        if (LogicDiff.magnitude > 0.15f)
        {

        }
        else
        {

            if (bindLogicActor.Pos == bindLogicActor.LastPos)
            {

            }
            else
            {
                float rv = bindLogicActor.speed * Time.deltaTime;
                Vector3 mvDir = targetVec * rv;
                transform.position += mvDir;
            }


            
        }




        return;
        
        

        ////if (diff.magnitude < 1e-1f)
        ////{
        ////    newState = eMoveState.IDLE;
        ////}

        ////animator.SetBool("moving", newMoving);

        //if (newState != NowState)
        //{
        //    if (guanxingTimer > 0f)
        //    {
        //        LastState = NowState;
        //        NowState = newState;
        //        guanxingTimer = 0;
        //    }
            
        //}
        //guanxingTimer += Time.deltaTime;

        //if (NowState == eMoveState.IDLE)
        //{
        //    return;
        //}

        //if (diff.magnitude < 0.4f)
        //{
        //    rv = rv * 0.5f;
        //}

        //if (bindLogicActor.GetLogicDTime() != 0)
        //{
        //    //计算 应该移多远 才能在下一逻辑帧抵达目标位置
        //    float rate = Time.deltaTime * 1000f / bindLogicActor.GetLogicDTime();
        //    float shouldMoveLen = bindLogicActor.LogicDiff.magnitude * 0.001f * rate;
        //    float actualMoveLen = rv;
        //    //如果 移动太慢了 需要加速 
        //    if (shouldMoveLen > actualMoveLen * 1.5f)
        //    {
        //        rv *= 1.5f;
        //    }
        //}



        //if (diff.magnitude < rv)
        //{
        //    //Debug.Log("shezhi");
        //    transform.position = targetWorldPos;
        //}
        //else
        //{
        //    //Debug.Log("移动");
        //    Vector3 mvDir = diff.normalized * rv;
        //    transform.position += mvDir;
        //}



    }



    public void UpdateVec()
    {
        Vector2 NowWorldPos = new Vector2(bindLogicActor.LastPos.x * 0.001f, bindLogicActor.LastPos.y * 0.001f);
        Vector2 NowDiff = (NowWorldPos - (Vector2)transform.position);
        
        //计算 目前的差距 看要不要强行调整
        

        Vector2 TargetWorldPos = new Vector2(bindLogicActor.Pos.x * 0.001f, bindLogicActor.Pos.y * 0.001f);
        Vector2 VelocityDir = (TargetWorldPos - (Vector2)transform.position);
        targetVec = VelocityDir.normalized;


        
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
