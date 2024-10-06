using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MslIndicatorObj : MonoBehaviour
{
    [SerializeField] GameObject parentUI;
    
    void Start()
    {
        gameObject.transform.SetParent(parentUI.transform);
    }

    void Update()
    {
        
    }
}
