using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//物品掉落
public class DropItems : MonoBehaviour
{

    [System.Serializable]
    public class LootItem
    {
        //掉落物品
        public GameObject item;

        [Range(0, 1)]
        //掉落的随机权重
        public float weight;
    }
    public LootItem[] lootItems;

    //敌人死亡的时候调用
    public void Spawnloot()
    {
        //取随机数 和权重想比较 如果小于 就生成掉落这个物品
        float currentValue = Random.value;

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 3;
                break;
            }
        }
    }
}
