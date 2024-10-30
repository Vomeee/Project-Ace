using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftSelectScreen : MonoBehaviour, MenuScreen
{
    [SerializeField] AircraftBase[] aircraftBaseList;
    [SerializeField] MainMenuController controller;
    public int screenMaxIndex = 4;
    public int screenStartIndex = 0;
    public float pointerTopPosY = 4.2f;

    void Start()
    {
        
    }

    public void ExecuteCurrentSelection(int index)
    {
        if (index == 0)
        {
            controller.SetPlayerAircraft(aircraftBaseList[index]);
            controller.StartMission1();
        }
        else if (index == 1)
        {
            //
        }
        else if (index == 2)
        {
            //
        }
        else if (index == 3)
        {
            //
        }
        else if (index == 4)
        {
            controller.ShowMainMenu();
        }


    }

    public int getMaxIndex()
    {
        return screenMaxIndex;
    }
    public float getPointerTopPosY()
    {
        return 4.2f;
    }
}
