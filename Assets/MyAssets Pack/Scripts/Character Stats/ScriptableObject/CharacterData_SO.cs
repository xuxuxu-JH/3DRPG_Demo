using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//>构建人物基本角色属性信息
[CreateAssetMenu(fileName = "NewData", menuName = "Scriptable Object/Character Stats/Health and Defence Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    [Header("EnemyKill")]
    //击杀一个敌人后 获得的经验值
    public int KillPoint;

    [Header("Level")]
    public int maxLevel;
    public int currentLevel;
    //提升等级的经验值要求
    public int baseExp;
    public int currentExp;
    //等级加成
    public float levelBuff;

    //>等级加成
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    //传入敌人的经验值 加给Player
    public void UpdateExp(int KillPoint)
    {
        currentExp += KillPoint;

        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//防止当前等级超过最大值
        baseExp += (int)(baseExp * LevelMultiplier);//>等级越高 需要的基础经验值的门槛也就越高
        maxHealth = (int)(maxHealth * LevelMultiplier);//等级提升 生命值上限也提升

        //currentHealth = maxHealth;//升级后 血量回满
        Debug.Log("Level Up! " + "当前等级为: " + currentLevel + " " + "当前最大生命值为: " + maxHealth);
        float exp = baseExp - currentExp;
        Debug.Log("提升至下一级还需要的经验值为: " + exp);
    }
}
