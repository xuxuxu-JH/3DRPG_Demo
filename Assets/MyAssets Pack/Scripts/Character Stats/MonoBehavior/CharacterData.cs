using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//所有的人物均挂载此脚本 Player/Enemy
//>获取CharacterStats人物基本属性和数值,以及数值的计算(攻击伤害和生命值的计算)
public class CharacterData : MonoBehaviour
{
    //血条UI更新事件 该事件在HealthBarUI中被注册
    //每次受击后执行一次更新血条UI
    public event Action<int, int> UpdateHealthBarOnAttack;

    //ins赋值 关联获取到角色的so属性文件
    [Header("SO_Data")]
    //template避免SO的特性
    public CharacterData_SO templateData;

    [HideInInspector]
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    private AttackData_SO baseAttackData;
    //保留原始的Animator 卸下武器时切换
    //Animator控制器里放的是Runtime
    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")]
    //记录武器生成的位置 用于Instantiate
    public Transform weaponSlot;
    public Transform particlePoint;
    public GameObject ParticleObj;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if (templateData != null)
        {
            //复制一份SO 而不是直接引用 
            characterData = Instantiate(templateData);
        }
        //记录初始的攻击力
        baseAttackData = Instantiate(attackData);
        //记录原始的Animator
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region 通过属性 直接获取角色的基本属性信息
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region 攻击伤害计算
    /// <summary>
    /// >伤害计算 该函数是由受击者进行调用的 改变的是受击者的生命值
    /// </summary>
    public void BeAttack(CharacterData attacker, CharacterData defener)
    {
        //Max:如果当攻击者的攻击力小于防御者的防御力 那么造成的伤害值为0 将不产生伤害
        //>实际伤害 = 攻击力(攻击者) - 防御力(防御者)
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        //改变受击者的血量
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)//攻击者暴击
        {
            //>发生暴击 触发暴击受伤的动画
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //受击后 更新的UI信息
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //>在被攻击后 如果受击者(Enemy)死了 就将自身的KillPoint加给Player
        if (CurrentHealth <= 0)
        {
            //将自身的KilPoint 传入进去 提升Player的经验值
            GameManager.Instance.playerCharacterData.characterData.UpdateExp(characterData.KillPoint);
        }

    }
    //>Rock
    public void BeAttack(int rockDamage, CharacterData defender)
    {
        int currentDamage = rockDamage - defender.CurrentDefence;
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    //>在产生攻击伤害之前 
    //>计算当前的实际攻击力 在最大和最小攻击力之间取随机值 如果本次攻击有暴击 会进行暴击加成计算
    private int CurrentDamage()
    {
        //从最小和最大伤害中取随机值
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        //如果发生暴击 伤害要进行暴击加成
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;//Mathf的参数必须为整数
    }
    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    //>装备生成武器 并更新Player攻击信息
    public void EquipWeapon(ItemData_SO weapon)
    {
        if (weapon.weaponPrefab != null)
        {
            //>在Playe的指定的位置上生成模型
            Instantiate(weapon.weaponPrefab, weaponSlot);
            //>装备后 将Player的AttackData替换
            attackData.AppalyWeaponData(weapon.weaponAttackData);

            Instantiate(ParticleObj, particlePoint);
        }

        //切换实时动画控制器 
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;

        //装备武器后 玩家属性会发生变化 需要更新一次信息面板的显示
        InventroyManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    public void EquipWeapon2(ItemData_SO weapon)
    {
        if (weapon.weaponPrefab != null)
        {
            //>装备后 将Player的AttackData替换
            attackData.AppalyWeaponData(weapon.weaponAttackData);

            Instantiate(ParticleObj, particlePoint);
        }
        //装备武器后 玩家属性会发生变化 需要更新一次信息面板的显示
        InventroyManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }


    //卸载武器
    public void UnEquipWeapon()
    {
        //将手部的武器模型都销毁
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }

        if (particlePoint.transform.childCount != 0)
        {
            for (int i = 0; i < particlePoint.transform.childCount; i++)
            {
                Destroy(particlePoint.transform.GetChild(i).gameObject);
            }
        }

        //卸载武器 也还原初始的动画
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        //卸下武器后 攻击里还原为初始值
        attackData.AppalyWeaponData(baseAttackData);
        InventroyManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
        print("更新显示");
    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else//CurrentHealth + amount > MaxHealth
        {
            //防止加血后 超过血量上限
            CurrentHealth = MaxHealth;
        }
    }

    #endregion
}
