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
    //���˵���
    private float kickForce = 20;
    //>Animation Event
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            //�õ�һ�������˵ķ���
            Vector3 Kickdirection = attackTarget.transform.position - transform.position;
            Kickdirection.Normalize();

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//ֹͣ�ƶ�
            attackTarget.GetComponent<NavMeshAgent>().velocity = Kickdirection * kickForce;//����
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
