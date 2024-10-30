using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectScreen : MonoBehaviour, MenuScreen
{
    [SerializeField] MainMenuController controller;
    public int screenMaxIndex = 2;
    public int screenStartIndex = 0;
    public float pointerTopPosY = 4.2f;
    // Start is called before the first frame update
    void Start()
    {
        //controller.maxIndex = screenMaxIndex;
        //controller.currentIndex = screenStartIndex;
        ////controller.pointerTopPosY = 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteCurrentSelection(int index)
    {
        if (index == 0)
        {
            controller.ShowAircraftSelectMenu();
        }
        else if (index == 1)
        {
            //
        }
        else if(index == 2)
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

