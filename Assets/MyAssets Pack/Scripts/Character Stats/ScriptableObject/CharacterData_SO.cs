using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//>�������������ɫ������Ϣ
[CreateAssetMenu(fileName = "NewData", menuName = "Scriptable Object/Character Stats/Health and Defence Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    [Header("EnemyKill")]
    //��ɱһ�����˺� ��õľ���ֵ
    public int KillPoint;

    [Header("Level")]
    public int maxLevel;
    public int currentLevel;
    //�����ȼ��ľ���ֵҪ��
    public int baseExp;
    public int currentExp;
    //�ȼ��ӳ�
    public float levelBuff;

    //>�ȼ��ӳ�
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    //������˵ľ���ֵ �Ӹ�Player
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
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//��ֹ��ǰ�ȼ��������ֵ
        baseExp += (int)(baseExp * LevelMultiplier);//>�ȼ�Խ�� ��Ҫ�Ļ�������ֵ���ż�Ҳ��Խ��
        maxHealth = (int)(maxHealth * LevelMultiplier);//�ȼ����� ����ֵ����Ҳ����

        //currentHealth = maxHealth;//������ Ѫ������
        Debug.Log("Level Up! " + "��ǰ�ȼ�Ϊ: " + currentLevel + " " + "��ǰ�������ֵΪ: " + maxHealth);
        float exp = baseExp - currentExp;
        Debug.Log("��������һ������Ҫ�ľ���ֵΪ: " + exp);
    }
}
