using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    public Transform player; // �÷��̾��� Transform
    public float speed = 10f; // �� ������� �ӵ�
    public float rotationSpeed = 2f; // ȸ�� �ӵ�

    public float distanceBehindPlayer = 10f; // �÷��̾��� �ڸ� ����

    void Update() //��ü ���� ����.
    {
        Vector3 targetPosition = player.position - player.forward * distanceBehindPlayer; // �÷��̾ Ÿ������ �����ʰ�, �� �ڸ� Ÿ������.
        // �÷��̾ ���� ���� ���
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public bool isTargeted = false; // Ÿ������ �����Ǿ����� ����
    public bool isLockedOn = false;
    // Ÿ������ ������ �� ȣ��
    public void OnTargeted()
    {
        isTargeted = true;
        // Ÿ������ �����Ǿ��� �� ������ �ٸ� �۾�
    }

    // Ÿ�ٿ��� ��� �� ȣ��
    public void OnUntargeted()
    {
        isTargeted = false;
        // Ÿ�ٿ��� ��� �� ������ �ٸ� �۾�
    }

    public void OnLockedOn()
    {
        isLockedOn = true;
        //�Ͽ�
    }

    public void OnLockedOff()
    {
        isLockedOn = false;
        //�� ����

    }
}
