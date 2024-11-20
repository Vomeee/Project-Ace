using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    [SerializeField]
    Plot plot;

    [Space]
    [Header("update system info instance")]
    [SerializeField]
    public float phase1TimeLimit;
    public float phase2TimeLimit;
    public float remainTime;
    public int score;
    [SerializeField] int objectiveScore = 12000;
    [SerializeField] TextMeshProUGUI systemTimeText; //시스템 시간 컴포넌트.
    [SerializeField] TextMeshProUGUI scoreText;

    public bool isPhaseEnd = false;


    void Start()
    {
        remainTime = phase1TimeLimit;
    }
    
    public void UpdateScore(int addedScore)
    {
        score += addedScore;
        scoreText.text = "SCORE <mspace=30>" + score.ToString("D6") + "</mspace>" + "/" + objectiveScore.ToString("D6");
        plot.EventControl(score, false);
    }

    void SetSystemTime()
    {
        if (remainTime <= 0)
        {

            remainTime = 0;
            return;
        }

        remainTime -= Time.deltaTime;
        int seconds = (int)remainTime;

        int min = seconds / 60;
        int sec = seconds % 60;
        int millisec = (int)((remainTime - seconds) * 100);
        string text = string.Format("TIME <mspace=30>{0:00}</mspace>:<mspace=30>{1:00}</mspace>:<mspace=30>{2:00}</mspace>", min, sec, millisec);
        systemTimeText.text = text;
    }

    void Update()
    {
        if(phase1TimeLimit == 0 && !isPhaseEnd)
        {
            isPhaseEnd = true;
            plot.EventControl(score, true);
        }
        SetSystemTime();
    }
}
