using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargettingSystem : MonoBehaviour
{
    [Header("player-enemy References")]
    public Transform playerTransform;
    public Transform currentTargetTransform; //현재 타겟의 Transform.
    public EnemyAI currentEnemy; //현재 타겟의 script.


    public float coneAngle = 25f;
    public float coneRadius = 400f;

    public Transform transformBox;

    [SerializeField]
    TextMeshProUGUI currentTargetText; //현재 타겟의 이름과 점수를 담는 좌상단 UI 컴포넌트.

    [SerializeField]
    public List<Transform> potentialTargetTransforms = new List<Transform>(); //일정 거리 안의 적들의 Transform을 담는 list.

    private void Start()
    {
        currentTargetTransform = null;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) //shift 키를 타겟 전환 키로 사용
        {
            SwitchTarget();
        }

        

        if (currentTargetTransform != null)
        {
            if (IsInCone(currentTargetTransform))
            {
                LockOnTarget(currentTargetTransform);
            }
            else
            {
                UnlockTarget(currentTargetTransform);
            }
        }
        else
        {
            currentTargetTransform = transformBox;
        }
    }

    

    public bool IsInCone(Transform target)
    {
        Vector3 directionToTarget = target.position - playerTransform.position;
        float distanceToTarget = directionToTarget.magnitude;

        // 구형 범위 내에 있는지 확인 (타겟팅 가능한 상태)
        if (distanceToTarget > coneRadius)
            return false;

        // 원뿔 범위 내에 있는지 확인
        float angleToTarget = Vector3.Angle(playerTransform.forward, directionToTarget);

        // 원뿔 범위의 각도 이내에 있는지 확인
        return angleToTarget <= coneAngle;
    }

    private void LockOnTarget(Transform target)
    {
        if (currentEnemy != null)
        {
            currentEnemy.OnLockedOn();
        }
    }

    private void UnlockTarget(Transform target)
    {
        if (currentEnemy != null)
        {
            currentEnemy.OnLockedOff();
        }

    }

    public void AddTarget(Transform target)
    {
        if (!potentialTargetTransforms.Contains(target))
        {          
            potentialTargetTransforms.Add(target);
            if (potentialTargetTransforms.Count == 1) SwitchTarget();
            //Debug.Log("Target added: " + target.name);
        }
    }

    public void RemoveTarget(Transform target)
    {
        if (potentialTargetTransforms.Contains(target))
        {
            potentialTargetTransforms.Remove(target);
            //Debug.Log("Target removed: " + target.name);

            // 타겟이 제거되었을 때 현재 타겟인 경우 처리
            if (currentTargetTransform == target)
            {
                UnlockTarget(target);
                SwitchTarget(); // 다른 타겟으로 전환
            }
        }
    }

    public void SwitchTarget()
    {
        if (potentialTargetTransforms.Count == 0)
        {
            currentTargetText.text = "";
            return;
        }
        Transform bestTarget = null;
        float bestDistance = Mathf.Infinity;
        bool isInConePriority = false;
        
        foreach (Transform target in potentialTargetTransforms)
        {
            // 현재 타겟은 스킵
            if (target == currentTargetTransform)
                continue;

            bool isInCone = IsInCone(target);
            float distanceToTarget = Vector3.Distance(playerTransform.position, target.position);

            // 원뿔 범위 내 타겟 우선 고려
            if (isInCone)
            {
                if (!isInConePriority || distanceToTarget < bestDistance)
                {
                    bestDistance = distanceToTarget;
                    bestTarget = target;
                    isInConePriority = true;
                }
            }
            else if (!isInConePriority && distanceToTarget < bestDistance)
            {
                // 원뿔 범위 외 타겟 고려 (원뿔 범위 내 타겟이 없는 경우)
                bestDistance = distanceToTarget;
                bestTarget = target;
            }
        }

        // 이전 타겟의 상태 업데이트
        if (currentTargetTransform != null)
        {
            EnemyAI previousTarget = currentTargetTransform.GetComponent<EnemyAI>();
            if (previousTarget != null)
            {
                previousTarget.OnLockedOff();
                previousTarget.OnUntargeted();
            }
        }

        // 가장 적합한 타겟으로 전환
        currentTargetTransform = bestTarget;

        // 새로운 타겟의 상태 업데이트
        if (currentTargetTransform != null)
        {
            EnemyAI newTarget = currentTargetTransform.GetComponent<EnemyAI>();
            if (newTarget != null)
            {
                newTarget.OnTargeted();
                currentEnemy = newTarget;
            }
            

            Debug.Log(newTarget.name);

            currentTargetText.text = "TARGET <mspace=30>" + newTarget.aircraftName + "</mspace><mspace=30> +" + newTarget.aircraftScore + "</mspace>";

        }
    }

}
