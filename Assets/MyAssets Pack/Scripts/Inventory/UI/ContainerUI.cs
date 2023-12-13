using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//>Container包含当前面板中的所有格子
public class ContainerUI : MonoBehaviour
{
    //> ContainerUI -> SlotHolder -> ItemUI

    //>存储背包中的所有单元格
    public SlotHolder[] slotHoldersArray;

    //UI中30个单元格对应背包中List的30个索引,也就是List中的容量和背包中的格子的个数是一一对应的
    //背包中的单元格和List中的索引是也是一一对应的,每一个格子都记录了自身的index,已便获取到对应的数据
    //单元格通过index得到了背包List中对应的数据,刷新UI显示

    //>当背包数据发生改变 刷新所有单元格的UI
    public void RefreshHolderUI()
    {
        for (int i = 0; i < slotHoldersArray.Length; i++)
        {
            //设置好正确的序号 保持和数组的序号一致
            slotHoldersArray[i].itemUI.Index = i;
            slotHoldersArray[i].UpdateItem();
        }
    }


}
