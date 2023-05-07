using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skillButtonScript : MonoBehaviour
{
    Button thisButton;
    BattleSystem battleSystem;

    private void Start()
    {
        battleSystem = BattleSystem.instance;
    }

    private void OnEnable()
    {
        thisButton = gameObject.GetComponent<Button>();
        //thisButton.onClick.AddListener(buttonCallBack);
    }



    void OnDisable()
    {
        //Un-Register Button Events
        thisButton.onClick.RemoveAllListeners();
    }

}
