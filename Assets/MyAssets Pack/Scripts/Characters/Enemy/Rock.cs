using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    //Rock三种状态
    //石头人生成石头 攻击Player
    //Player通过石头 反击石头人
    //石头落地 Player接触不产生伤害
    public enum RockStates
    {
        HitPlayer,
        HitEnemy,
        HitNothing
    }
    public RockStates rockStates;
    private Rigidbody rb;
    public GameObject breakPrefab;

    [Header("Basic Settings")]
    //石头向前冲击力
    public float force;
    //石头飞向的目标 实例化后由StoneBoss赋值
    public GameObject target;
    //石头飞向的方向
    Vector3 direction;
    //石头造成的伤害
    public int rockDamage;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //>石头生成是 默认状态是攻击Player的
        rockStates = RockStates.HitPlayer;
        rb.velocity = Vector3.one;//给rock一个初始的速度 避免初始速度为0 影响速度判断
        //实例化直接飞向目标
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //>判断刚体的速度 当石头在地面上速度缓慢静止时 也切换为HitNothing状态
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
            Invoke("InstantiateBreak", 4.99999999f);
            Destroy(gameObject, 5f);
        }
    }

    //>飞向目标
    public void FlyToTarget()
    {
        //即时在生成石头的时候丢失了Player目标 依然得到Player飞向它
        if (target == null)
            target = FindAnyObjectByType<PlayerController>().gameObject;
        //石头飞向的方向
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
        //玩家方向向量减去当前的坐标 然后再标准化取反 就得到这个飞出去的方向 + v3.up 添加偏移量 使得石头能够在空中停滞飞行一会

    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterData>().BeAttack(rockDamage, other.gameObject.GetComponent<CharacterData>());
                    //>砸到Player切换状态为HitNothing
                    rockStates = RockStates.HitNothing;

                }
                break;

            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<StoneBoss>())
                {
                    var BossData = other.gameObject.GetComponent<CharacterData>();
                    BossData.BeAttack(rockDamage, BossData);
                    InstantiateBreak();
                    Debug.Log("Boss受到石头攻击");
                    Debug.Log(BossData.CurrentHealth);

                    Destroy(gameObject);
                }
                break;
        }
    }

    public void InstantiateBreak()
    {
        Instantiate(breakPrefab, transform.position, Quaternion.identity);
    }
}
