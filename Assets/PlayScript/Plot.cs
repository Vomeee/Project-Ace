using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
    [SerializeField] GameObject cutSceneCamera; //�ƽ��� ����� ī�޶� ������Ʈ, enable true�� �ִϸ��̼ǰ� �԰� ����.
    [SerializeField] Camera mainCamera; //�÷��� ī�޶�.

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
    [SerializeField] Transform[] playerFollowPoints; //�÷��̾� ��ó�� ���� ��ġ. ���⸶�� �ϳ���.
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
    [SerializeField] int phaseStartScore;

    [SerializeField] RectTransform aceDeployedUI;
    [SerializeField] TextMeshProUGUI deployedAceText;

    [SerializeField] RectTransform CutSceneUI;
    [SerializeField] Canvas cutSceneUICanvas;
    [SerializeField] AudioSource OstPlayer;
    [SerializeField] AudioClip phase1Ost; //showdown
    [SerializeField] AudioClip phase2Ost; //kings

    [SerializeField] WeaponSystem weaponSystem;
     
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        tagController.ShowStartMissionTag();
        phase = 1;
        currentEnemyCount = 20;
        currentTGTCount = 0;
        

        Invoke("Phase1Start", 2.0f);
    }

    public void InvokeMethod(string methodName)
    {
        Invoke(methodName, 0f); //��翡�� string �޾Ƽ� �ش� �Լ� ����.
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
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController, this);
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

        //////////missile script block.
        weaponSystem.scriptCoolDown = 60f;
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
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController, this);
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
        }
    }

    void Phase1End() // 1������ ������ �� �ƽ� ��� ����.
    {
        //timescale = 0; //�ð� ����...�� �ȵǴµ�.
        //�뷡 ���߱�
        OstPlayer.Stop();
        OstPlayer.clip = phase2Ost; //kings.
        //���� ���� ��� ��Ȱ��ȭ?

        //�ƽ� ����.
        if(cutSceneCamera != null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            // �� Canvas�� enabled �Ӽ� ��Ȱ��ȭ
            foreach (Canvas canvas in canvases)
            {
                canvas.enabled = false;
            }

            cutSceneUICanvas.enabled = true;

            Time.timeScale = 0; //�ð� ����.
            cutSceneCamera.SetActive(true); //�� �� ī�޶� Ȱ��ȭ.
            Camera.main.gameObject.SetActive(false); //�⺻ ī�޶� ��Ȱ��ȭ.
        }
    }

    public void Phase2Start()
    {
        Debug.Log("Phase 2 start ");
     

        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = true;
        }
        cutSceneUICanvas.enabled = false;
        mainCamera.gameObject.SetActive(true);
        cutSceneCamera.SetActive(false); //�� �� ī�޶� ��Ȱ��ȭ.
        OstPlayer.Play(); //���� ����.
        gameManagement.remainTime = gameManagement.phase2TimeLimit;
        gameManagement.isPhaseEnd = false;
        scriptManager.ClearScriptQueue();
        scriptManager.AddScript(onPhase2StartScripts);
        phase = 2;

        ////missile script block.
        weaponSystem.scriptCoolDown = 60f;
        ////

        Time.timeScale = 1; //���� ��� ����.
        

        
        tagController.ShowMissionUpdatedTag(); //�ӹ� ���� �±�. ���⼭ �Ҹ� ����.
        

        currentEnemyCount += phase2EnemyCount; //2������ ���� �߰�.

        currentTGTCount = phase2EnemyCount;

        for (int i = 0; i < phase2EnemyCount; i++)
        {
            GameObject enemy1 = Instantiate(enemyAircraftPrefabsPhase2[i], playerFollowPoints[i].position, playerFollowPoints[i].rotation);
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();

            if (enemyAI1 != null)
            {
                
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject, enemyMissilePrefab, warningController, this);
                //enemyAI1.waypointQueue.Enqueue(phase2Waypoints[i]); //���� ������ ���� �� 
                //enemyAI1.waypointQueue.Enqueue(playerFollowPoints[i]); //�÷��̾� ���� ����.
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
        }
    }

    void MissionAccomplished()
    {
        tagController.ShowMissionAccomplishedTag();// �ӹ� �Ϸ� �±�
        scriptManager.ClearScriptQueue();
        scriptManager.AddScript(missionAccomplishedScripts); //�ӹ� �Ϸ� �� ������ ����. �� �߿� �޴� ���� ��ũ��Ʈ ����.
    }

    void missionFailed()
    {
        //ȭ����� 
        
        //�ٽ� �Ѽ�

        //ui����.
    }

    public void ReturnToMainMenu()
    {
        //���θ޴� ����.
        //fade out?

        //Scene ����
        SceneManager.LoadScene("HomeScene");
    }

    void ReTurnToMissionAccomplishedMenu()
    {
        //�ӹ� �Ϸ� �� 
    }

    [Header("optional bool")]
    bool p1_1 = false;
    bool p1_2 = false;
    bool p1_3 = false;
    bool p1_4 = false;
    bool p1_5 = false;
    bool p1_6 = false;
    bool phase1End = false;

    bool p2_1 = false;
    bool p2_2 = false;
    bool p2_3 = false;
    bool p2_4 = false;
    bool p2_5 = false;
    bool p2_6 = false;
    bool phase2End = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
            {
            Phase1End();
        }
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
        currentTGTCount--;

        if (currentTGTCount == 0)
        {
            EventControl(gameManagement.score, false);
        }
    }

    public void EventControl(int score, bool phaseEnd)
    {
        #region Phase1 Event

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

                if(gameManagement.phase1TimeLimit >= 30) //�����ð� 5�� �̻�.
                {
                    Phase1AceSpawn();
                    aceDeployedUI.gameObject.SetActive(true);
                    deployedAceText.text = "CRIMSON";
                }
            }
            
            if (currentEnemyCount == 0) //�� ����
            {
                phase1End = true;
                scriptManager.ClearScriptQueue();
                phaseStartScore = score;
                scriptManager.AddScript(onPhase1EndScripts);

            }

            if(phaseEnd) //�ð� ����
            {
                if(score >= 1500)
                {
                    Debug.Log("phase 1 succsss!");
                    phase1End = true; // �� ���� �ߵ�
                    phaseStartScore = score;
                    scriptManager.AddScript(onPhase1EndScripts); //2������ ���� ��ũ��Ʈ ����.
                }
                else
                {
                    phase1End = true;
                    //mission Failed
                }
            }
        }
        #endregion

        #region Phase 2 Event.
        else if (phase == 2)
        {
            if (currentTGTCount == 7 && !p2_1)
            {
                scriptManager.AddScript("SO_1");
                p2_1 = true;
            }
            else if (currentTGTCount == 6 && !p2_2)
            {
                scriptManager.AddScript("SO_2");
                p2_2 = true;
            }
            else if (currentTGTCount == 4 && !p2_3)
            {
                scriptManager.AddScript("AO_3");
                p2_3 = true;
            }
            else if (currentTGTCount == 3 && !p2_4)
            {
                scriptManager.AddScript("SO_3");
                p2_4 = true;
            }
            else if (currentTGTCount == 2 && !p2_5)
            {
                scriptManager.AddScript("SO_4");
                p2_5 = true;
            }
            //ace for phase 2?
            //else if (score > 1000 && !p1_6)
            //{
            //    p1_6 = true;

            //    if (gameManagement.timeLimit >= 30) //�����ð� 5�� �̻�.
            //    {
            //        Phase1AceSpawn();
            //        aceDeployedUI.gameObject.SetActive(true);
            //        deployedAceText.text = "CRIMSON";
            //    }
            //}

            else if (currentTGTCount == 0 && !phase2End) //�� ���� -> ����.
            {
                phase2End = true;
                //scriptManager.ClearScriptQueue();
                MissionAccomplished(); //���� �������� ���⿡.
                

            }

            if (phaseEnd) //�ð� ����
            {
                //�ӹ� ����.
            }
        }

    #endregion
    }
}
