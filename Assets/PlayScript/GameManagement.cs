using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    [Header("spawn player instance")]
    public GameObject playerAircraft;


    [Space]
    [Header("update system info instance")]
    public int score;
    [SerializeField] TextMeshProUGUI scoreText;


    void Start()
    {
        Vector3 startPosition = new Vector3(0, 300, 0);
        Quaternion startRotation = new Quaternion(0f, 0f, 0f, 0f);
        GameObject player = Instantiate(playerAircraft, startPosition, startRotation);
    }
    
    public void UpdateScore(int addedScore)
    {
        score += addedScore;
        scoreText.text = "SCORE <mspace=30>" + score.ToString("D6") + "</mspace>";
    }

}
