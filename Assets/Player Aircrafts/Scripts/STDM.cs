using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STDM : MonoBehaviour
{
    public Transform target; // ������ Ÿ��
    public float turningForce; // ȸ�� �ӵ�

    public float maxSpeed; // �ִ� �ӵ�
    public float accelAmount; // ���ӷ�
    public float lifetime; // �̻����� ����
    public float speed; // ���� �ӵ�

    [SerializeField] private GameObject enemyHitEffect; //���� ���߽� ����ȿ��
    [SerializeField] private GameObject groundHitEffect;

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider mslCollider;

    public void Launch(Transform target, float launchSpeed)
    {
        // Ÿ���� ������ ���� �Ҵ�
        if (target != null)
        {
            this.target = target;
        }

        // �߻� �ӵ��� ����
        speed = launchSpeed;
    }

    void LookAtTarget()
    {
        // Ÿ���� ������ ���� ����
        if (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
        }
    }

    void Start()
    {
        // ������ ������ �̻����� ����
        Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody>();
        mslCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // �ӵ��� maxSpeed�� ���� �ʵ��� ����
        if (speed < maxSpeed)
        {
            speed += accelAmount * Time.deltaTime;
        }

        // Ÿ���� ������ ������ ����
        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            // Ÿ���� ������ ����
            LookAtTarget();
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision) //���̵�, ���̵�... �ı�.
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ���⿡ �ε����� �� ȿ�� ����
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);

            Debug.Log("missilehittoenemy");
        }
        
        // �浹�� ������Ʈ�� �±װ� "Ground"�� ���
        if (collision.gameObject.CompareTag("Ground"))
        {
            // ���� ����� �� ȿ�� ����
            Instantiate(groundHitEffect, transform.position, Quaternion.identity);
        }

        // �Ѿ� �ı�
        Destroy(gameObject);
    }
}
