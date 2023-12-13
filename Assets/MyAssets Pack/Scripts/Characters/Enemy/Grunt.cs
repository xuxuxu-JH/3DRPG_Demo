using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    public float kickForce = 10;
    /// <summary>
    /// ����֡�¼����� ���ڻ���Player
    /// </summary>
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //�õ����ɵķ���
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            //�ڻ���ǰ��Playerֹͣ�ƶ�
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            //����Player ����velocity����
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
