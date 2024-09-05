using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STDM : MonoBehaviour
{
    public Transform target; // 추적할 타겟
    public float turningForce; // 회전 속도

    public float maxSpeed; // 최대 속도
    public float accelAmount; // 가속량
    public float lifetime; // 미사일의 수명
    public float speed; // 현재 속도

    public void Launch(Transform target, float launchSpeed)
    {
        // 타겟이 존재할 때만 할당
        if (target != null)
        {
            this.target = target;
        }

        // 발사 속도를 설정
        speed = launchSpeed;
    }

    void LookAtTarget()
    {
        // 타겟이 존재할 때만 추적
        if (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
        }
    }

    void Start()
    {
        // 수명이 끝나면 미사일을 제거
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // 속도가 maxSpeed를 넘지 않도록 가속
        if (speed < maxSpeed)
        {
            speed += accelAmount * Time.deltaTime;
        }

        // 타겟이 없으면 직진만 수행
        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            // 타겟이 있으면 추적
            LookAtTarget();
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
