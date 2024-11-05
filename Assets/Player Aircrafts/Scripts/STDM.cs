using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STDM : MonoBehaviour
{

    [Header("missile attributes")]
    public Transform target; // ������ Ÿ��
    public float turningForce; // ȸ�� �ӵ�
    public float maxSpeed; // �ִ� �ӵ�
    public float accelAmount; // ���ӷ�
    public float lifetime; // �̻����� ����
    public float speed; // ���� �ӵ�

    public float startSpeed = 50;
    public float boresightAngle;// �̻����� �����Ѱ� ����.

    [Space]
    [Header("References")]

    public TagController tagController;

    [SerializeField] private GameObject enemyHitEffect; //���� ���߽� ����ȿ��
    [SerializeField] private GameObject groundHitEffect;

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider mslCollider;

    [SerializeField] float currentTime;

    public void Launch(Transform target, float launchSpeed, TagController tagController)
    {
        this.tagController = tagController;

        // Ÿ���� ������ ���� �Ҵ�
        if (target != null)
        {
            this.target = target;
        }
        

        // �߻� �ӵ��� ����
        speed = launchSpeed + startSpeed;
    }

    void LookAtTarget()
    {
        // Ÿ���� ������ ���� ����
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
        // ������ ������ �̻����� ����
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
            tagController.ShowMissedTag();
            Instantiate(groundHitEffect, transform.position, Quaternion.identity);
        }

        // �Ѿ� �ı�
        Destroy(gameObject);
    }
}
