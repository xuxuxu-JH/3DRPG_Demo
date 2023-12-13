using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//挂在在每一个快捷栏的格子中
public class ActionButton : MonoBehaviour
{
    //Inspector窗口中绑定了1-6的数字键
    public KeyCode actionKey;
    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItemData())
        {
            //currentSlotHolder.UseItem();
        }
    }
}
