using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//物品类型
public enum ItemType
{
    //消耗品
    Useable,
    //武器
    Weapon,
    //防具
    Armor,
}
//>单个物品的数据 
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    //物品类型
    public ItemType itemType;

    public string itemName;
    //>物品显示在背包上的图标
    public Sprite itemIcon;
    //>物品数量 
    public int itemAmount;

    [TextArea]//物品描述
    public string description = "";
    //>该物品是否可以堆叠 默认武器是不可堆叠的
    public bool stackable;

    [Header("Useable Item")]
    public UseableItemData_SO useableData;

    [Header("Weapon")]
    //>生成在玩家身上的物体模型 
    public GameObject weaponPrefab;
    //>当前物品的攻击属性信息
    public AttackData_SO weaponAttackData;
    //>装备武器后的另一套Animator
    public AnimatorOverrideController weaponAnimator;
}
