using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public GameObject playerAircraft;



    void Start()
    {
        Vector3 startPosition = new Vector3(0, 300, 0);
        Quaternion startRotation = new Quaternion(0f, 0f, 0f, 0f);
        GameObject stdm = Instantiate(playerAircraft, startPosition, startRotation);
    }
    
}
