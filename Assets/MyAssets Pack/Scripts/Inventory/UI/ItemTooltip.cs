using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//物品信息显示
public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;

    public Text itemInfoText;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        //鼠标实时位置跟随
        UpdatePosition();
    }
    //将物品的数据 和 描述文本相匹配
    //显示 物品名字 和 物品描述
    public void SetupTooltip(ItemData_SO itemData)
    {
        itemNameText.text = itemData.itemName;
        itemInfoText.text = itemData.description;
    }

    void Update()
    {
        UpdatePosition();
    }
    //将描述信息栏 实时 跟随鼠标位置
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y < height)
        {
            rectTransform.position = mousePos + Vector3.up * height * 0.6f;
        }
        else if (Screen.width - mousePos.x > width)
        {
            rectTransform.position = mousePos + Vector3.right * width * 0.6f;
        }
        else
        {
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
        }
    }
}
