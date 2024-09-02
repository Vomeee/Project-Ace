using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    public Transform player; // 플레이어의 Transform
    public float speed = 10f; // 적 비행기의 속도
    public float rotationSpeed = 2f; // 회전 속도

    public float distanceBehindPlayer = 10f; // 플레이어의 뒤를 설정

    void Update() //자체 비행 로직.
    {
        Vector3 targetPosition = player.position - player.forward * distanceBehindPlayer; // 플레이어를 타겟으로 하지않고, 그 뒤를 타겟으로.
        // 플레이어를 향한 방향 계산
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public bool isTargeted = false; // 타겟으로 지정되었는지 여부
    public bool isLockedOn = false;
    // 타겟으로 지정될 때 호출
    public void OnTargeted()
    {
        isTargeted = true;
        // 타겟으로 지정되었을 때 수행할 다른 작업
    }

    // 타겟에서 벗어날 때 호출
    public void OnUntargeted()
    {
        isTargeted = false;
        // 타겟에서 벗어날 때 수행할 다른 작업
    }

    public void OnLockedOn()
    {
        isLockedOn = true;
        //록온
    }

    public void OnLockedOff()
    {
        isLockedOn = false;
        //록 오프

    }
}
