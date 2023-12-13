using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StoneBoss : EnemyController
{
    [Header("Skill")]
    public float kickForce;
    //得到石头预制体
    public GameObject rockPrefab;
    //石头在手部生成的位置
    public Transform handPos;

    //Attack近距离攻击
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacinTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterData>();
            //对方-自己 取相对方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            targetStats.GetComponent<NavMeshAgent>().isStopped = true;//停止移动
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//击退

            targetStats.BeAttack(enemyData, targetStats);
        }

    }
    //Skill 远距离攻击
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            //动画帧事件 某帧生成石头
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            //生成石头后 将Boss中检测到的attackTarget赋值给 target 这样石头才有了目标
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }

}
