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

    void OnCollisionEnter(Collision collision) //���̵�, ���̵�... �ı�.
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ���⿡ �ε����� �� ȿ�� ����
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            
            Debug.Log("222");
        }
        // �浹�� ������Ʈ�� �±װ� "Ground"�� ���
        //else if (collision.gameObject.CompareTag("Ground"))
        {
            // ���� ����� �� ȿ�� ����
            //Instantiate(groundHitEffect, transform.position, Quaternion.identity);
        }

        // �Ѿ� �ı�
        Destroy(gameObject);
    }
}
