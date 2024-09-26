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

    #endregion



    [SerializeField] GameObject fa26Prefab;
    private float elapsedTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        tagController.ShowStartMissionTag();
    }


    Vector3 centerPosition = new Vector3(0, 200, 0); // 기준 위치
    float spawnRadius = 1000f; // 구의 반경

    //Vector3 examplePostition = new Vector3(400f, 300f, 400f);
    Vector3 exampleRotation = new Vector3(0, 0, 0);
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 3초 이상 경과한 경우
        if (elapsedTime > 10.0f)
        {

            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y); // Y축은 양수로 유지 (바닥 아래로 생기지 않도록)

            Vector3 examplePosition = centerPosition + randomOffset; // 기준 위치에 랜덤 오프셋 추가
            Vector3 exampleRotation = new Vector3(0, 0, 0);

            GameObject enemy1 = Instantiate(fa26Prefab, examplePosition, Quaternion.Euler(exampleRotation));
            EnemyAI enemyAI1 = enemy1.GetComponent<EnemyAI>();

            if (enemyAI1 != null ) 
            {
                enemyAI1.initializeInstance(playerTransform, targettingSystem, tagController, gameManagement, waypointObject);
            }
            else
            {
                Debug.Log("enemyAi null!");
            }
            





            elapsedTime = 0.0f; // 다시 초기화하여 반복 가능하게
        }
    }
}
