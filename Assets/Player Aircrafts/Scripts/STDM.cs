using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STDM : MonoBehaviour
{

    [Header("missile attributes")]
    public Transform target; // 추적할 타겟
    public float turningForce; // 회전 속도
    public float maxSpeed; // 최대 속도
    public float accelAmount; // 가속량
    public float lifetime; // 미사일의 수명
    public float speed; // 현재 속도

    public float startSpeed = 50;
    public float boresightAngle;// 미사일의 추적한계 각도.

    [Space]
    [Header("References")]

    public TagController tagController;

    [SerializeField] private GameObject enemyHitEffect; //적기 명중시 폭파효과
    [SerializeField] private GameObject groundHitEffect;

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider mslCollider;

    [SerializeField] float currentTime;

    public void Launch(Transform target, float launchSpeed, TagController tagController)
    {
        this.tagController = tagController;

        // 타겟이 존재할 때만 할당
        if (target != null)
        {
            this.target = target;
        }
        

        // 발사 속도를 설정
        speed = launchSpeed + startSpeed;
    }

    void LookAtTarget()
    {
        // 타겟이 존재할 때만 추적
        if (target == null)
            return;

        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (angle > boresightAngle)
        {
            target = null;
            tagController.ShowMissedTag();

            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
    }

    void Start()
    {
        // 수명이 끝나면 미사일을 제거
        //Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody>();
        mslCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;

        if (speed < maxSpeed)
        {
            speed += accelAmount * Time.fixedDeltaTime;
        }

        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }
        else
        {
            LookAtTarget();
            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }

        if (currentTime >= lifetime - 1)
        {
            tagController.ShowMissedTag();
            Destroy(gameObject);
        }
    }



    void OnCollisionEnter(Collision collision) //땅이든, 적이든... 파괴.
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 적기에 부딪혔을 때 효과 생성
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);

            Debug.Log("missilehittoenemy");
        }
        
        // 충돌한 오브젝트의 태그가 "Ground"일 경우
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 땅에 닿았을 때 효과 생성
            tagController.ShowMissedTag();
            Instantiate(groundHitEffect, transform.position, Quaternion.identity);
        }

        // 총알 파괴
        Destroy(gameObject);
    }
}
