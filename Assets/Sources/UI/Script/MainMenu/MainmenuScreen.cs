using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainmenuScreen : MonoBehaviour, MenuScreen
{
    [SerializeField] MainMenuController controller;
    public int screenMaxIndex = 2;
    public int screenStartIndex = 0;
    public float pointerTopPosY = 3.35f;
    
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteCurrentSelection(int index)
    {
        if(index == 0)
        {
            controller.ShowMissionSelectMenu(); //�̼� ȭ������ ��ȯ
        }
        else if(index == 1)
        {
            controller.ShowControlScreen();
        }
        else if(index == 2)
        {
            controller.Quit(); //���� ����
        }
    }

    public int getMaxIndex()
    {
        return screenMaxIndex;
    }
    public float getPointerTopPosY()
    {
        return 3.35f;
    }
}
