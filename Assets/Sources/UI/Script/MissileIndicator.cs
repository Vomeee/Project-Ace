using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissileIndicator : MonoBehaviour
{
    [SerializeField] EnemySTDM enemyMissile;
    [SerializeField] RawImage indicatorImage;
    //[SerializeField] RectTransform parentUI;

    RectTransform rectTransform;

    [SerializeField] float minimumScaleFactor; //�ּ�ũ��. 5����
    [SerializeField] float maximumScaleFactor; //�ִ�ũ��. 10����
    float scaleFactor;

    [SerializeField] float emergencyDistance;
    [SerializeField] float superEmergencyDistance;

    [SerializeField] AudioSource missileAlertNormal; //���Ÿ� �溸��
    [SerializeField] AudioSource missileAlertEmergency; //�ٰŸ� �溸��(���)
    [SerializeField] AudioSource missileAlertSuperEmergency;
    [SerializeField] bool isEmergency;
    [SerializeField] bool isSuperEmergency;

    [SerializeField] float normalInterval;
    [SerializeField] float emergencyInterval;
    [SerializeField] float superEmergencyInterval;

    Coroutine blinkCoroutine;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // ��ġ �ʱ�ȭ
        rectTransform.anchoredPosition = Vector3.zero;

        // ȸ���� �ʱ�ȭ (Quaternion.Euler�� ����Ͽ� ȸ���� 0���� ����)
        rectTransform.rotation = Quaternion.Euler(0, 0, 0);

        blinkCoroutine = StartCoroutine(BlinkIndicator(normalInterval));
    }
    public void InitalizeReference(EnemySTDM missile, 
        AudioSource missileAlertNormal, AudioSource missileAlertEmergency, AudioSource missileAlertSuperEmergency)
    {
        enemyMissile = missile;
        this.missileAlertNormal = missileAlertNormal;
        this.missileAlertEmergency = missileAlertEmergency;
        this.missileAlertSuperEmergency = missileAlertSuperEmergency;
    }

    void Update()
    {
        if (enemyMissile == null || enemyMissile.target == null)
        {
            Destroy(gameObject);            
        }

        if (enemyMissile != null)
        {
            if (Camera.main != null)
            {
                Vector3 missileLocationVector = enemyMissile.transform.position - Camera.main.transform.position;

                //////////////Angle control
                Vector3 missileDirFromCamera = missileLocationVector.normalized;
                Vector3 indicatorDirection = Camera.main.transform.InverseTransformDirection(missileDirFromCamera);

                float angle = Mathf.Atan2(indicatorDirection.x, indicatorDirection.y) * Mathf.Rad2Deg;
                indicatorImage.transform.rotation = Quaternion.Euler(50, 0, -angle);

                ///////////////Scale Control
                float currentDistance = missileLocationVector.magnitude;
                float distanceFactor = Mathf.Clamp01(currentDistance / 1000f);  // 0~1 ���� ������ ��ȯ
                scaleFactor = Mathf.Lerp(minimumScaleFactor, maximumScaleFactor, distanceFactor);  // �ּ�-�ִ� �������� ������ �� ����

                rectTransform.localScale = Vector3.one * scaleFactor;

                ///////////////Audio Control
                if (currentDistance <= superEmergencyDistance)
                {
                    // Super emergency: Closest proximity
                    if (!isSuperEmergency)
                    {
                        isSuperEmergency = true;
                        isEmergency = false;

                        missileAlertNormal.Stop();
                        missileAlertEmergency.Stop();
                        missileAlertSuperEmergency.Play();

                        StartBlinking(superEmergencyInterval);
                    }
                }
                else if (currentDistance <= emergencyDistance)
                {
                    // Emergency: Mid-range proximity
                    if (!isEmergency)
                    {
                        isSuperEmergency = false;
                        isEmergency = true;

                        missileAlertNormal.Stop();
                        missileAlertSuperEmergency.Stop();
                        missileAlertEmergency.Play();

                        StartBlinking(emergencyInterval);
                    }
                }
                else
                {
                    // Normal alert: Far distance
                    if (!missileAlertNormal.isPlaying && !isEmergency && !isSuperEmergency &&
                        !missileAlertEmergency.isPlaying && !missileAlertSuperEmergency.isPlaying)
                    {
                        missileAlertNormal.Play();
                        StartBlinking(normalInterval);
                    }
                }

                ///////////////Warning bool Control
                if (currentDistance > emergencyDistance)
                {
                    isEmergency = false;
                    isSuperEmergency = false;
                }
            }

        }
    }

    void StartBlinking(float interval)
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine); // ���� �ڷ�ƾ�� �ִٸ� �ߴ�
        }
        blinkCoroutine = StartCoroutine(BlinkIndicator(interval)); // ���ο� �ڷ�ƾ ����
    }

    // Coroutine to handle the blinking effect
    IEnumerator BlinkIndicator(float interval)
    {
        bool isVisible = true;
        while (true)
        {
            // Toggle visibility by changing the alpha value
            Color color = indicatorImage.color;
            color.a = isVisible ? 1f : 0f; // isVisible�� ���� ���İ� ����
            indicatorImage.color = color;

            isVisible = !isVisible; // ���� ���
            yield return new WaitForSeconds(interval); // ���͹���ŭ ���
        }
    }
}
