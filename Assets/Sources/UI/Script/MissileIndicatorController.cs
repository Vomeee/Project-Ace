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

        // SetParent 후에 localRotation을 설정
        newMissileIndicator.transform.SetParent(transform, false);

        // 부모의 로컬 회전을 상속받도록 강제로 설정
        newMissileIndicator.transform.localRotation = Quaternion.identity;

        MissileIndicator indicator = newMissileIndicator.GetComponent<MissileIndicator>();

        indicator.InitalizeReference(missile);
    }
}
