using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileIndicatorController : MonoBehaviour
{
    [SerializeField] GameObject missileIndicatorPrefab;
    [SerializeField] GameObject newMissileIndicator;
    void Start()
    {
        
    }

    public void AddMissileIndicator(EnemySTDM missile)
    {
        this.newMissileIndicator = Instantiate(missileIndicatorPrefab);

        // SetParent �Ŀ� localRotation�� ����
        newMissileIndicator.transform.SetParent(transform, false);

        // �θ��� ���� ȸ���� ��ӹ޵��� ������ ����
        newMissileIndicator.transform.localRotation = Quaternion.identity;

        MissileIndicator indicator = newMissileIndicator.GetComponent<MissileIndicator>();

        indicator.InitalizeReference(missile);
    }
}
