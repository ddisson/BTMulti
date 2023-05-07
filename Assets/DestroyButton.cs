using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyButton : MonoBehaviour
{
    Button thisIcon;

    private void OnEnable()
    {
        thisIcon = gameObject.GetComponent<Button>();
        thisIcon.onClick.AddListener(DestroyOnClick);
    }

    private void OnDIsable()
    {
        thisIcon.onClick.RemoveAllListeners();
    }

    private void DestroyOnClick()
    {
        if (thisIcon != null)
        {
            Destroy(gameObject);
        }




    }

}
