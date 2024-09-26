using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargettingSystem : MonoBehaviour
{
    public Transform playerTransform;
    public Transform currentTarget;
    public float coneAngle = 20f;
    public float coneRadius = 400f;

    public Transform transformBox;

    [SerializeField]
    TextMeshProUGUI currentTargetText;

    [SerializeField]
    private List<Transform> potentialTargetTransforms = new List<Transform>();

  

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // 예를 들어 Tab 키를 타겟 전환 키로 사용
        {
            SwitchTarget();
        }

        if (currentTarget != null)
        {
            if (IsInCone(currentTarget))
            {
                LockOnTarget(currentTarget);
            }
            else
            {
                UnlockTarget(currentTarget);
            }
        }
        else
        {
            currentTarget = transformBox;
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
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.OnLockedOn();
        }
    }

    private void UnlockTarget(Transform target)
    {
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.OnLockedOff();
        }

    }

    public void AddTarget(Transform target)
    {
        

        if (!potentialTargetTransforms.Contains(target))
        {          
            potentialTargetTransforms.Add(target);
            if (potentialTargetTransforms.Count == 1) SwitchTarget();
            Debug.Log("Target added: " + target.name);
        }
    }

    public void RemoveTarget(Transform target)
    {
        if (potentialTargetTransforms.Contains(target))
        {
            potentialTargetTransforms.Remove(target);
            Debug.Log("Target removed: " + target.name);

            // 타겟이 제거되었을 때 현재 타겟인 경우 처리
            if (currentTarget == target)
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
            if (target == currentTarget)
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
        if (currentTarget != null)
        {
            EnemyAI previousTarget = currentTarget.GetComponent<EnemyAI>();
            if (previousTarget != null)
            {
                previousTarget.OnLockedOff();
                previousTarget.OnUntargeted();
            }
        }

        // 가장 적합한 타겟으로 전환
        currentTarget = bestTarget;

        // 새로운 타겟의 상태 업데이트
        if (currentTarget != null)
        {
            EnemyAI newTarget = currentTarget.GetComponent<EnemyAI>();
            if (newTarget != null)
            {
                newTarget.OnTargeted();
            }
            

            Debug.Log(newTarget.name);

            currentTargetText.text = "TARGET <mspace=30>" + newTarget.aircraftName + "</mspace> <mspace=30> +" + newTarget.aircraftScore + "</mspace>";

        }
    }

}
