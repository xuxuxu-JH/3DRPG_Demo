using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//>构建角色的攻击属性信息
[CreateAssetMenu(fileName = "NewAttack", menuName = "Scriptable Object/Character Stats/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    //远距离攻击范围
    public float skillRange;
    public float CDTime;
    public int minDamage;
    public int maxDamage;
    //暴击加成
    public float criticalMultiplier;
    //暴击率
    public float criticalChance;
    /// <summary>
    /// >装备武器后 更新人物攻击属性 直接替换
    /// </summary>
    public void AppalyWeaponData(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        CDTime = weapon.CDTime;

        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalMultiplier = weapon.criticalMultiplier;
        criticalChance = weapon.criticalChance;

    }
}
