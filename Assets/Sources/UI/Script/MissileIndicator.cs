using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileIndicator : MonoBehaviour
{
    [SerializeField] EnemySTDM enemyMissile;

    [SerializeField] RawImage indicatorImage;
    //[SerializeField] RectTransform parentUI;

    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // ��ġ �ʱ�ȭ
        rectTransform.anchoredPosition = Vector3.zero;

        // ȸ���� �ʱ�ȭ (Quaternion.Euler�� ����Ͽ� ȸ���� 0���� ����)
        rectTransform.rotation = Quaternion.Euler(0, 0, 0);


    }
    public void InitalizeReference(EnemySTDM missile)
    {
        enemyMissile = missile;
        Destroy(gameObject, enemyMissile.lifetime - 0.1f);
    }

    void Update()
    {
        if (enemyMissile == null || enemyMissile.target == null)
            Destroy(gameObject);
        //Vector3 missilePos = Camera.main.WorldToViewportPoint(enemyMissile.transform.position);

        //no bound
        //no shown bool

        Vector3 missileDirFromCamera = (enemyMissile.transform.position - Camera.main.transform.position).normalized;

        Vector3 indicatorDirection = Camera.main.transform.InverseTransformDirection(missileDirFromCamera);

        float angle = Mathf.Atan2(indicatorDirection.x, indicatorDirection.y) * Mathf.Rad2Deg;
        indicatorImage.transform.rotation = Quaternion.Euler(50, 0, -angle);

    }
}
