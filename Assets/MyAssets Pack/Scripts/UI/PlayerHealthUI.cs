using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//>更新Player经验和生命值显示 挂载到 PlayerHealth Canvas
public class PlayerHealthUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;

    private void Awake()
    {
        //得到PlayerHealth Canvas 下对应的子物体
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        //将Player的等级转换为文本显示
        levelText.text = "Level" + GameManager.Instance.playerCharacterData.characterData.currentLevel.ToString();

        UpdataHealth();
        UpdateExp();
    }

    void UpdataHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerCharacterData.CurrentHealth / GameManager.Instance.playerCharacterData.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerCharacterData.characterData.currentExp / GameManager.Instance.playerCharacterData.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}
