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

        // 3�� �̻� ����� ���
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
            





            elapsedTime = 0.0f; // �ٽ� �ʱ�ȭ�Ͽ� �ݺ� �����ϰ�
        }
    }
}
