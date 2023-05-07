using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHud : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpText;



    public void SetHud(Titan titan)
    {
        nameText.text = titan.titanName;
        hpSlider.maxValue = titan.maxHP;
        hpSlider.value = titan.currentHP;
        hpText.text = titan.currentHP.ToString();
    }
    

}