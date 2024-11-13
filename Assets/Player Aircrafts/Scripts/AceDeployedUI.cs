using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AceDeployedUI : MonoBehaviour
{
    [SerializeField] float uiShowingTime;

    void Awake()
    {
        Invoke("DisableUI", uiShowingTime);
    }

    void DisableUI()
    {
       gameObject.SetActive(false);
    }
 
}
