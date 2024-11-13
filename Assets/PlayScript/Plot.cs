using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class Plot : MonoBehaviour
{
    [Header("enemies' instances")]
    #region gameplayInstances for enemies

    [SerializeField] Transform playerTransform;
    [SerializeField] TargettingSystem targettingSystem;
    [SerializeField] TagController tagController;
    [SerializeField] GameManagement gameManagement;
    [SerializeField] GameObject waypointObject;
    [SerializeField] GameObject enemyMissilePrefab;
    [SerializeField] WarningController warningController;
    #endregion

    [SerializeField]
    ScriptManager scriptManager;

    [SerializeField]
    int phase1EnemyCount;
    [SerializeField]
    int phase2EnemyCount;
    [SerializeField]
    int currentEnemyCount;
    [SerializeField] 
    int currentTGTCount; 

    [Header("enemy spawn lists")]
    [SerializeField] GameObject[] enemyAircraftPrefabsPhase1;
    [SerializeField] GameObject[] enemyAcePrefabsPhase1;
    [SerializeField] GameObject[] enemyAircraftPrefabsPhase2;
    [SerializeField] GameObject[] enemyAcePrefabsPhase2;
    [SerializeField] GameObject[] allyAircraftPrefabs;

    [SerializeField] Transform[] phase1SpawnTransforms;
    [SerializeField] Transform[] phase2SpawnTransforms;
    [SerializeField] Transform[] phase2Waypoints;
    [SerializeField] Transform[] playerFollowPoints; //플레이어 근처에 따라갈 위치. 적기마다 하나씩.
    [SerializeField] Transform[] phase1AllySpawnTransforms;
    [SerializeField] Transform[] phase2AllySpawnTransforms;

    [SerializeField] Transform[] phase1AceSpawnTransforms;
    [SerializeField] Transform[] phase2AceSpawnTransforms;

    [Header("Subtitles")]
    [SerializeField] List<string> onPhase1StartScripts;
    [SerializeField] List<string> onPhase1EndScripts;

    [SerializeField] List<string> onPhase2StartScripts;
    //[SerializeField] List<string> onPhase2EndScripts;

    [SerializeField] List<string> missionAccomplishedScripts;

    [SerializeField] int phase;

    [SerializeField] RectTransform aceDeployedUI;
    [SerializeField] TextMeshProUGUI deployedAceText;

    // Start is called before the first frame update
    void Start()
    {
        tagController.ShowStartMissionTag();
        phase = 1;
        currentEnemyCount = 20;
        currentTGTCount = 0;

        Invoke("Phase1Start", 2.0f);
    }

    public void InvokeMethod(string methodName)
    {
        Invoke(methodName, 0f); //대사에서 string 받아서 해당 함수 실행.
    }

    void Phase1Start()
    {
        ///////enemy spawn for phase 1
        currentEnemyCount = phase1EnemyCount;
        for (int i = 0; i < phase1EnemyCount; i++)
        {
            
            GameObject enemy1 = Instantiate(enemyAircraftPrefabsPhase1[i], phase1SpawnTransforms[i]);
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();

            if (enemyAI1 != null)
            {
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController);
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
        }
        //////Ally Spawn for phase 1.
        for (int i = 0; i < phase1AllySpawnTransforms.Length; i++)
        {
            GameObject ally = Instantiate(allyAircraftPrefabs[i], phase1AllySpawnTransforms[i]);
            ALLY allyScript = ally.GetComponent<ALLY>();
            if (allyScript != null)
            {
                
            }

        }

        scriptManager.AddScript(onPhase1StartScripts);
    }

    void Phase1AceSpawn()
    {
        ///////enemy spawn for phase 1
        currentEnemyCount += enemyAcePrefabsPhase1.Length;
        for (int i = 0; i < enemyAcePrefabsPhase1.Length; i++)
        {
            GameObject enemy1 = Instantiate(enemyAcePrefabsPhase1[i], phase1AceSpawnTransforms[i]);
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();

            if (enemyAI1 != null)
            {
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController);
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
        }
    }

    void Phase2Start()
    {
        gameManagement.timeLimit = 900; //시간제한 변경
        gameManagement.isPhaseEnd = false;
        tagController.ShowMissionUpdatedTag(); //임무 변경 태그. 여기서 소리 내기.
        

        currentEnemyCount += phase2EnemyCount; //2페이즈 적기 추가.
        for (int i = 0; i < phase2EnemyCount; i++)
        {
            GameObject enemy1 = Instantiate(enemyAircraftPrefabsPhase2[i], phase2SpawnTransforms[i]);
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();

            if (enemyAI1 != null)
            {
                enemyAI1.waypointQueue.Enqueue(phase2Waypoints[i]); //각자 일직선 주행 후 
                enemyAI1.waypointQueue.Enqueue(playerFollowPoints[i]); //플레이어 추적 시작.
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController);
                
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
        }
    }

    void MissionAccomplished()
    {
        tagController.ShowMissionAccomplishedTag();// 임무 완료 태그
        scriptManager.AddScript(missionAccomplishedScripts); //임무 완료 후 실행할 대사들. 이 중에 메뉴 복귀 스크립트 있음.
    }

    void ReturnToMainMenu()
    {
        //메인메뉴 복귀.
    }

    [Header("optional bool")]
    bool p1_1 = false;
    bool p1_2 = false;
    bool p1_3 = false;
    bool p1_4 = false;
    bool p1_5 = false;
    bool p1_6 = false;
    bool phase1End = false;

    void Update()
    {
        if(gameManagement.remainTime == 0)
        {
            if(!phase1End)
            {
                phase1End = true;
                EventControl(gameManagement.score, true);
            }
        }
    }



    void PhaseSpawn()
    {

    }

    public void TGTReduced()
    {
        currentEnemyCount--;
    }

    public void EventControl(int score, bool phaseEnd)
    {
        if ((phase == 1))
        {
            if (score > 3000 && !p1_1)
            {
                scriptManager.AddScript("VO_1");
                p1_1 = true;
            }
            else if (score > 5000 && !p1_2)
            {
                scriptManager.AddScript("AO_1");
                p1_2 = true;
            }
            else if (score > 7000 && !p1_3)
            {
                scriptManager.AddScript("VO_2");
                p1_3 = true;
            }
            else if (score > 9000 && !p1_4)
            {
                scriptManager.AddScript("PO_1");
                p1_4 = true;
            }
            else if (score > 13000 && !p1_5)
            {
                scriptManager.AddScript("VO_3");
                p1_5 = true;
            }
            else if (score > 1000 && !p1_6)
            {                
                p1_6 = true;

                if(gameManagement.timeLimit >= 30) //남은시간 5분 이상.
                {
                    Phase1AceSpawn();
                    aceDeployedUI.gameObject.SetActive(true);
                    deployedAceText.text = "CRIMSON";
                }
            }
            
            if (currentEnemyCount == 0) //적 전멸
            {
                phase1End = true;
                scriptManager.AddScript(onPhase1EndScripts);
            }

            if(phaseEnd) //시간 종료
            {
                if(score >= 1500)
                {
                    Debug.Log("phase 1 succsss!");
                    phase1End = true; // 한 번만 발동
                    scriptManager.AddScript(onPhase1EndScripts); //2페이즈 시작 스크립트 포함.
                }
                else
                {
                    phase1End = true;
                    //mission Failed
                }
            }
        }
    }
}
