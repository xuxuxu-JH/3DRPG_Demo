using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// MonsterPlant 
/// </summary>
public class MonsterPlant : EnemyController
{
    [Header("Skill")]
    //击退的力
    private float kickForce = 20;
    //>Animation Event
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            //得到一个被击退的方向
            Vector3 Kickdirection = attackTarget.transform.position - transform.position;
            Kickdirection.Normalize();

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//停止移动
            attackTarget.GetComponent<NavMeshAgent>().velocity = Kickdirection * kickForce;//击退
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
