using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//拖拽背包面板
public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    RectTransform rectTransform;
    public Canvas InventoryCanvas;
    public Button closeButton;
    RectTransform closeButtonRt;
    private void Awake()
    {
        //得到面板的Rt
        rectTransform = GetComponent<RectTransform>();
        if (transform.gameObject.name == "Inventroy Bag")
            closeButtonRt = closeButton.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //拖拽的时候不是面板中心点跟随鼠标移动
        //eventData.delta 会加上每一帧的鼠标偏移量
        rectTransform.anchoredPosition += eventData.delta / InventoryCanvas.scaleFactor;

        if (transform.gameObject.name == "Inventroy Bag")
            closeButtonRt.anchoredPosition += eventData.delta / InventoryCanvas.scaleFactor;
    }
    //当鼠标按下准备拖拽面板的时候
    //改变当前面板Index层级 让它显示在上层
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);
    }
}
