using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    #region gameplayInstances

    [SerializeField] Transform playerTransform;
    [SerializeField] TargettingSystem targettingSystem;


    #endregion 





    [SerializeField] GameObject fa26Prefab;
    private float elapsedTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 3초 이상 경과한 경우
        if (elapsedTime > 4.0f)
        {
            GameObject enemy1 = Instantiate(fa26Prefab);
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();
            if (enemyAI1 != null ) 
            {
                enemyAI1.initializeInstance(playerTransform, targettingSystem);
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
            





            elapsedTime = 0.0f; // 다시 초기화하여 반복 가능하게
        }
    }
}
