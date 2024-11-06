using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargettingSystem : MonoBehaviour
{
    [Header("player-enemy References")]
    public Transform playerTransform;
    public Transform currentTargetTransform; //���� Ÿ���� Transform.
    public EnemyAI currentEnemy; //���� Ÿ���� script.


    public float coneAngle = 25f;
    public float coneRadius = 400f;

    public Transform transformBox;

    [SerializeField]
    TextMeshProUGUI currentTargetText; //���� Ÿ���� �̸��� ������ ��� �»�� UI ������Ʈ.

    [SerializeField]
    public List<Transform> potentialTargetTransforms = new List<Transform>(); //���� �Ÿ� ���� ������ Transform�� ��� list.

    private void Start()
    {
        currentTargetTransform = null;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) //shift Ű�� Ÿ�� ��ȯ Ű�� ���
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

        // ���� ���� ���� �ִ��� Ȯ�� (Ÿ���� ������ ����)
        if (distanceToTarget > coneRadius)
            return false;

        // ���� ���� ���� �ִ��� Ȯ��
        float angleToTarget = Vector3.Angle(playerTransform.forward, directionToTarget);

        // ���� ������ ���� �̳��� �ִ��� Ȯ��
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

            // Ÿ���� ���ŵǾ��� �� ���� Ÿ���� ��� ó��
            if (currentTargetTransform == target)
            {
                UnlockTarget(target);
                SwitchTarget(); // �ٸ� Ÿ������ ��ȯ
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
            // ���� Ÿ���� ��ŵ
            if (target == currentTargetTransform)
                continue;

            bool isInCone = IsInCone(target);
            float distanceToTarget = Vector3.Distance(playerTransform.position, target.position);

            // ���� ���� �� Ÿ�� �켱 ���
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
                // ���� ���� �� Ÿ�� ��� (���� ���� �� Ÿ���� ���� ���)
                bestDistance = distanceToTarget;
                bestTarget = target;
            }
        }

        // ���� Ÿ���� ���� ������Ʈ
        if (currentTargetTransform != null)
        {
            EnemyAI previousTarget = currentTargetTransform.GetComponent<EnemyAI>();
            if (previousTarget != null)
            {
                previousTarget.OnLockedOff();
                previousTarget.OnUntargeted();
            }
        }

        // ���� ������ Ÿ������ ��ȯ
        currentTargetTransform = bestTarget;

        // ���ο� Ÿ���� ���� ������Ʈ
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
