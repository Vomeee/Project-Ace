using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 400f;
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider bulletCollider;

    public GameObject enemyHitEffect;
    public ParticleSystem ps;
    //public GameObject groundHitEffect;





    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * bulletSpeed;
        
    }

    void OnCollisionEnter(Collision collision) //땅이든, 적이든... 파괴.
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 적기에 부딪혔을 때 효과 생성
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            
            Debug.Log("222");
        }
        // 충돌한 오브젝트의 태그가 "Ground"일 경우
        //else if (collision.gameObject.CompareTag("Ground"))
        {
            // 땅에 닿았을 때 효과 생성
            //Instantiate(groundHitEffect, transform.position, Quaternion.identity);
        }

        // 총알 파괴
        Destroy(gameObject);
    }
}
