using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    public float kickForce = 10;
    /// <summary>
    /// 动画帧事件函数 用于击退Player
    /// </summary>
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //得到击飞的方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            //在击飞前将Player停止移动
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            //击飞Player 调用velocity属性
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
