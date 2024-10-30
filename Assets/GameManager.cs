using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /////////////////////////////////game informations////////////////////////////////
    public int score;

    /////////////////////////////////player informations/////////////////////////////////
    [SerializeField] public AircraftBase playerAircraftObject;

}
