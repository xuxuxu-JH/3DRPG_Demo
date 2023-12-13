using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    //>敌人的初始状态
    GUARD,//守卫 站着不动
    PATROL,//巡逻 在巡逻的范围内随机移动
    //>敌人的状态(初始状态会切换到以下两个状态)
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterData))]
//>敌人的AI逻辑:状态切换,追击/攻击Player,动画切换
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    protected CharacterData enemyData;
    private Collider enemyCollider;

    [Header("Basic Settings")]
    public float sightRadius;//视野范围
    public bool isGuard;//>Ins勾选 是否是守卫类型的敌人
    private float speed;//>记录初始速度 用于追击和巡逻状态速度的切换
    protected GameObject attackTarget;//>记录攻击对象Player
    //攻击CD 每次攻击结束后会重置 0.5s
    private float lastAttackTime;

    [Header("Patrol State")]
    //巡逻状态相关变量
    public float patrolRange;////巡逻移动范围
    private Vector3 guardOriginalPos;//记录初始(当前)位置的点
    private Quaternion guardRotation;//记录初始角度
    private Vector3 wayPoint;//随机目标点

    [Header("Animation Parameters")]
    //绑定动画参数变量
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardOriginalPos = transform.position;//获得初始位置
        guardRotation = transform.rotation;//获得初始角度
        enemyData = GetComponent<CharacterData>();
        enemyCollider = GetComponent<Collider>();
    }
    private void Start()
    {
        //确定enemy的默认初始状态
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            //如果是巡逻状态的敌人 在游戏初始就要给一个随机的点 防止wayPoint不是默认值0,0,0
            GetNewWayPoint();
        }
        //游戏开始 将敌人加入观察者列表当中
        GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        //如果敌人有这个脚本 且 当前状态是死亡的时候 就掉落
        if (GetComponent<DropItems>() && isDead)
        {
            GetComponent<DropItems>().Spawnloot();
        }

        //敌人死亡 任务工作 更新任务进度
        if (QuestDataManager.IsInitialized && isDead)
        {
            QuestDataManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    }

    private void Update()
    {
        //实时进行死亡判断
        if (enemyData.CurrentHealth == 0)
        {
            isDead = true;
        }

        if (!playerDead)
        {
            SwitchStates();
            SetAnimationParameters();
            lastAttackTime -= Time.deltaTime;
        }

    }

    void SetAnimationParameters()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", enemyData.isCritical);
        anim.SetBool("Death", isDead);
    }

    //>敌人状态的切换
    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
        {
            agent.stoppingDistance = enemyData.attackData.attackRange;
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            //>守卫 和 巡逻 是两个初始状态
            //>追击 和 死亡 是两个触发状态

            #region GUARD 守卫状态
            case EnemyStates.GUARD:
                isChase = false;

                //脱战后走回到初始位置
                if (transform.position != guardOriginalPos)
                {
                    isWalk = true;
                    //往回走的时候 速度为原来的一半
                    agent.speed = speed * 0.5f;
                    agent.isStopped = false;

                    agent.destination = guardOriginalPos;
                    //距离检测 是否以及回到了初始的位置
                    if (Vector3.Distance(guardOriginalPos, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        //缓慢转向 初始角度 
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            #endregion

            #region PATROL 巡逻状态
            case EnemyStates.PATROL:
                isChase = false;//脱战
                agent.speed = speed * 0.3f;//巡逻状态速度是原来的 1/3

                //如果 当前位置和随机点的距离 小于等于stoppingDistance 说明走到了随机点的位置
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    GetNewWayPoint();
                }
                else //>Vector3.Distance(wayPoint, transform.position) >= agent.stoppingDistance
                //没走到随机点 继续走
                {
                    isWalk = true;
                    //如果wayPoint在Start中没有被初始化 Enemy就会走到 0,0,0 这个初始值位置
                    agent.destination = wayPoint;
                }
                break;
            #endregion

            #region CHASE 追击状态
            case EnemyStates.CHASE:
                #region 追击
                //走变跑
                isWalk = false;
                isChase = true;
                agent.speed = speed;//当追击的时候 速度增加 变为正常速度

                //>当玩家在视野范围内 就追击玩家
                if (FoundPlayer())
                {
                    isFollow = true;
                    agent.isStopped = false;
                    transform.LookAt(attackTarget.transform);
                    agent.destination = attackTarget.transform.position;
                }
                //>脱战 如果在追击的时候玩家离开了范围
                else if (!FoundPlayer())
                {
                    isFollow = false;
                    //脱战后 敌人先停留在当前位置
                    agent.destination = transform.position;

                    //脱战 切换回初始默认状态
                    if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else//Patrol
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                #endregion

                #region 攻击
                //>当player进入视野范围内 先追击; 进入攻击范围后 攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    //如果在攻击范围内 动画和位置都要停止
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)//lastAttakTime In Update -=
                    {
                        //重置CD冷却时间 0.5s
                        lastAttackTime = enemyData.attackData.CDTime;
                        //>在攻击前 根据暴击率进行暴击判断 暴击率在0~data数据中去随机值 如果在暴击数值范围内 返回true
                        enemyData.isCritical = Random.value < enemyData.attackData.criticalChance;
                        Attack();
                    }
                }
                #endregion
                break;
            #endregion

            #region DEAD 死亡
            case EnemyStates.DEAD:
                enemyCollider.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
            #endregion

            default:
                break;
        }
    }
    //>根据Player的距离 执行远距离攻击还是近身攻击
    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            //Attack = ture
            //Attack = ture && isCritical = true 会执行Attack02
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //只适用于Boss 
            anim.SetTrigger("Skill");
        }
    }

    //>Enemy 实时检测Player是否进入可视范围内
    //>获取到attackTarget
    bool FoundPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    #region 攻击范围检测
    //Player在近距离攻击范围内
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyData.attackData.attackRange;
        else
            return false;
    }

    //Player在远距离攻击范围内
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyData.attackData.skillRange;
        else
            return false;
    }

    #endregion
    //>基于巡逻范围 生成随机点wayPoint   
    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //基于初始位置 进行计算 从而得到新的随机位置(Y不变)
        //>初始位置 + 随机值 = 新的位置 Vector3 
        Vector3 randomPoint = new Vector3(guardOriginalPos.x + randomX, transform.position.y, guardOriginalPos.z + randomZ);

        //>在用产生的随机点之前 先进行判断 这个点在是否在Navmesh的可行走区域内
        //>如果是 返回为true hit.position就是在范围内的点 
        //>如果不是 wayPoint = transfrom.position 会回到Distance 再次执行GetNewWayPoint() 直到满足
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    //画出敌人的视野&巡逻范围
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

    //>攻击动画 帧事件函数
    void Hit()
    {
        if (attackTarget != null && transform.IsFacinTarget(attackTarget.transform))
        {
            var targetData = attackTarget.GetComponent<CharacterData>();
            targetData.BeAttack(enemyData, targetData);
        }
    }

    public void EndNotify()
    {
        //停止移动
        //停止所有动画 只播放胜利动画
        //停止Agent
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;

    }
}
