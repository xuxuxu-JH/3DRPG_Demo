using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//>Player的移动 攻击 动画参数 攻击帧事件
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Animator animator;
    private CharacterData characterStats;
    //攻击目标游戏对象
    private GameObject attackTarget;
    //攻击冷却CD 无冷却
    private float lastAttackTime;
    private bool isDead;
    public float addRockForce;

    //得到agent的默认初始停止距离
    private float stopDistance;
    void Awake()
    {
        gameObject.SetActive(true);
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        characterStats = GetComponent<CharacterData>();

        stopDistance = agent.stoppingDistance;
        //记录玩家信息
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    private void OnEnable()
    {

    }
    private void Start()
    {
        //>OnMouseClicked event registered
        MouseManager.Instance.OnMouseClicked += MoveToTarget;

        //>OnEnemyClicked event registered
        MouseManager.Instance.OnEnemyClicked += MoveToAttackTarget;

        SaveManager.Instance.LoadPlayerData();
    }
    private void OnDisable()
    {
        //装场景 Player消失 需要取消注册订阅
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= MoveToAttackTarget;
    }

    private void Update()
    {
        //实时判断玩家是否死亡
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)
        {
            //> Dead 通知所有观察者 执行各自的EndNotify()函数里的内容
            GameManager.Instance.NotifyObservers();
        }
        SetAnimationParameters();
        lastAttackTime -= Time.deltaTime;
    }

    //绑定动画控制器中的参数
    void SetAnimationParameters()
    {
        //Plyaer 由静止->走路->跑 动画的切换 基于anget的速度 来决定
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("Death", isDead);
    }

    //>Player移动到目标位置(Ground Portal Item)
    public void MoveToTarget(Vector3 target)
    {
        //Player正在执行攻击的时候 (IEMoveToAttackTarget) 协程正在执行
        //如果在执行攻击的时候点击地面或别的操作 需要打断攻击 切换为移动(MoveToTarget) 终止所有协程 
        StopAllCoroutines();
        if (isDead)
            return;
        agent.stoppingDistance = stopDistance;
        //每次走到敌人位置后 都会停下来再攻击 当再次点击位置走到别处时 需要重置为false 继续行走
        agent.isStopped = false;
        agent.destination = target;
    }

    //>Player移动到敌人位置
    private void MoveToAttackTarget(GameObject target)
    {
        if (isDead)
            return;

        if (target != null)
        {
            attackTarget = target;

            //>在执行攻击前 先进行本次攻击的暴击判断 
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(IEMoveToAttackTarget());
        }
    }
    IEnumerator IEMoveToAttackTarget()
    {
        //再次点击 继续移动 还原agent的移动状态
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);

        if (attackTarget == null)
            yield break;

        //>攻击移动判断 在攻击范围外 保持移动
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        //>在攻击范围内 停止移动 执行攻击
        agent.isStopped = true;

        //停下来后执行一次攻击的动画 
        if (lastAttackTime < 0)
        {
            animator.SetBool("Critical", characterStats.isCritical);
            animator.SetTrigger("Attack");
            //Debug.Log("攻击");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.CDTime;
        }
    }
    //> Player 攻击动画 帧事件函数
    void Hit()
    {
        if (attackTarget.gameObject.CompareTag("Attackable"))
        {
            //>如果使用的是石头进行反击的攻击方式
            if (attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * addRockForce, ForceMode.Impulse);
            }
        }
        else
        //>普通攻击方式
        {
            var targetData = attackTarget.GetComponent<CharacterData>();
            //Enemy调用BeAttack
            targetData.BeAttack(characterStats, targetData);
        }
    }
}
