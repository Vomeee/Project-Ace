using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("enemy spawn lists")]
    [SerializeField] GameObject[] enemyAircraftPrefabsPhase1;
    [SerializeField] GameObject[] enemyAcePrefabsPhase1;
    [SerializeField] GameObject[] enemyAircraftPrefabsPhase2;
    [SerializeField] GameObject[] enemyAcePrefabsPhase2;
    [SerializeField] GameObject[] allyAircraftPrefabs;

    [SerializeField] Transform[] phase1SpawnTransforms;
    [SerializeField] Transform[] phase2SpawnTransforms;
    [SerializeField] Transform[] phase1AllySpawnTransforms;
    [SerializeField] Transform[] phase2AllySpawnTransforms;

    [SerializeField] Transform[] phase1AceSpawnTransforms;
    [SerializeField] Transform[] phase2AceSpawnTransforms;

    [Header("Subtitles")]
    [SerializeField] List<string> onPhase1StartScripts;
    [SerializeField] List<string> onPhase1EndScripts;

    [SerializeField] List<string> onPhase2StartScripts;
    [SerializeField] List<string> onPhase2EndScripts;

    [SerializeField] int phase;

    // Start is called before the first frame update
    void Start()
    {
        tagController.ShowStartMissionTag();
        phase = 1;
        currentEnemyCount = 35;

        Invoke("Phase1Start", 2.0f);
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

    void Phase2Start()
    {
        currentEnemyCount = phase2EnemyCount;
        for (int i = 0; i < phase2EnemyCount; i++)
        {
            GameObject enemy1 = Instantiate(enemyAircraftPrefabsPhase2[i], phase2SpawnTransforms[i]);
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

    [Header("optional bool")]
    bool p1_1 = false;
    bool p1_2 = false;
    bool p1_3 = false;
    bool p1_4 = false;
    bool p1_5 = false;
    bool phase1End = false;

    void Update()
    {
        
    }



    void PhaseSpawn()
    {

    }

    public void EnemyReduced()
    {
        currentEnemyCount--;
    }

    public void EventControl(int score)
    {
        if ((phase == 1))
        {
            if (score > 2000 && !p1_1)
            {
                scriptManager.AddScript("VO_1");
                p1_1 = true;
            }
            else if (score > 5000 && !p1_2)
            {
                scriptManager.AddScript("AO_1");
                p1_2 = true;
            }
            else if (score > 8000 && !p1_3)
            {
                scriptManager.AddScript("VO_2");
                p1_3 = true;
            }
            else if (score > 12000 && !p1_4)
            {
                scriptManager.AddScript("PO_1");
                p1_4 = true;
            }
            else if (score > 15000 && !p1_5)
            {
                scriptManager.AddScript("VO_3");
                p1_5 = true;
            }
            else if (score > 18000 && !phase1End)
            {
                scriptManager.AddScript("VO_2");
                phase1End = true;
                Invoke("Phase2Start", 12f);
            }
        }
    }
}
