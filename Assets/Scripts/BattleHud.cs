using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class BattleHud : MonoBehaviourPun
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

        photonView.RPC("SyncHudAcrossNetwork", RpcTarget.Others, titan.titanName, titan.maxHP, titan.currentHP);
    }

    public void UpdateHud(Titan titan)
    {
        hpSlider.value = titan.currentHP;
        hpText.text = titan.currentHP.ToString();

        photonView.RPC("SyncHudAcrossNetwork", RpcTarget.Others, titan.titanName, titan.maxHP, titan.currentHP);
    }

    [PunRPC]
    void SyncHudAcrossNetwork(string titanName, float maxHP, float currentHP)
    {
        nameText.text = titanName;
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        hpText.text = currentHP.ToString();
    }
}
