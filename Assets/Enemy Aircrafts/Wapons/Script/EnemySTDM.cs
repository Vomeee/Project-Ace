using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySTDM : MonoBehaviour
{
    public WarningController wc;
    public TagController tagController;
    public Transform target; // ������ Ÿ��

    [Header("Attributes")]
    public float turningForce; // ȸ�� �ӵ�
    public float maxSpeed; // �ִ� �ӵ�
    public float accelAmount; // ���ӷ�
    public float lifetime; // �̻����� ����
    public float speed; // ���� �ӵ�
    public int damage; //�̻����� �����

    public float boresightAngle; //�Ѱ� ���� ����. 90�� base


    [Space]
    [Header("Effect and sounds")]
    [SerializeField] private GameObject enemyHitEffect; //���� ���߽� ����ȿ��
    [SerializeField] private GameObject groundHitEffect;

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider mslCollider;


    public void Launch(Transform target, float launchSpeed, WarningController warningController)
    {
        wc = warningController;

        // Ÿ���� ������ ���� �Ҵ�
        if (target != null)
        {
            this.target = target;

            wc.TrackingMissileCount(1);
        }

        Debug.Log("Missile instantiated");
        // �߻� �ӵ��� ����
        speed = launchSpeed;
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
            Debug.Log("evaded");
            target = null;

            //�溸 ����
            wc.TrackingMissileCount(-1);
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
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
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            wc.TrackingMissileCount(-1);
            // ���⿡ �ε����� �� ȿ�� ����
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            
            Aircraft playerAircraft = collision.gameObject.GetComponent<Aircraft>();
            if (playerAircraft != null)
            {
                playerAircraft.playerHP -= damage;
            }


            Debug.Log("missilehittoplayer");

            Destroy(gameObject);
        }

        // �浹�� ������Ʈ�� �±װ� "Ground"�� ���
        if (collision.gameObject.CompareTag("Ground"))
        {
            // ���� ����� �� ȿ�� ����
            Instantiate(groundHitEffect, transform.position, Quaternion.identity);

            
        }

        // �Ѿ� �ı�
        
    }
}
