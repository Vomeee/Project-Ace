using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionEndScreen : MonoBehaviour, MenuScreen
{
    [SerializeField] MainMenuController controller;
    [SerializeField] Text scoreText;
    [SerializeField] Text timeBonusText;
    [SerializeField] Text finalRankText;


    public int screenMaxIndex = 0;
    public int screenStartIndex = 0;
    public float pointerTopPosY = 4.2f;
    // Start is called before the first frame update
    void Awake()
    {
        int score = PlayerPrefs.GetInt("Score");

        scoreText.text = "Score: " + score;

        //time bonus
        int finalTime = PlayerPrefs.GetInt("Final Time");

        int timeBonus = finalTime >= 500 ? 20000 : 20000 - (40 * (500 - finalTime));

        timeBonusText.text = "Time Bonus: " + timeBonus.ToString();

        int finalScore = timeBonus + score;

        if(finalScore >= 45000)
        {
            finalRankText.text = "Final Rank: S";
        }
        else if (finalScore >= 40000)
        {
            finalRankText.text = "Final Rank: A";
        }
        else if (finalScore >= 30000)
        {
            finalRankText.text = "Final Rank: B";
        }
        else if (finalScore >= 25000)
        {
            finalRankText.text = "Final Rank: C";
        }
        else
        {
            finalRankText.text = "Final Rank: D";
        }

        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Mission Number", 0);
        PlayerPrefs.SetInt("Final Time", 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteCurrentSelection(int index)
    {
        if (index == 0)
        {
            controller.ShowMainMenu();
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
        return pointerTopPosY;
    }
}

