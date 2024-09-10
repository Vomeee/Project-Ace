using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    
    [SerializeField] Transform player; // �÷��̾��� Transform

    #region aircraft moving logic's variables and referecnces

    [SerializeField] float speed = 10f; // �� ������� �ӵ�
    [SerializeField] float rotationSpeed = 2f; // ȸ�� �ӵ�

    [SerializeField] float distanceBehindPlayer = 10f; // �÷��̾��� �ڸ� ����

    #endregion
    
    [Space]

    #region reactive UI references and variables
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform lockOnUIRectTransform;
    [SerializeField] Image lockOnUIImage;

    [SerializeField] Color lockedOnColor = Color.red;
    [SerializeField] Color normalColor = Color.green;
    [SerializeField] Color transparentColor = new Color(1f,0f,0f,0f); // Ÿ���� ������ ���� ����

    [SerializeField] GameObject aircraftInfoUIobject; // ���� ���� UI ��Ʈ
    [SerializeField] Text distanceText;
    [SerializeField] Text aircraftNameText;

    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 650f;

    [SerializeField] Vector2 minSize = new Vector2(10f, 10f); // UI�� �ּ� ũ��
    [SerializeField] Vector2 maxSize = new Vector2(100f, 100f); // UI�� �ִ� ũ��

    [SerializeField] bool isFlickering = false;

    [SerializeField] Color sibal;

    #endregion

    public bool isTargeted = false; // Ÿ������ �����Ǿ����� ����
    public bool isLockedOn = false;

    void Start()
    {
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;
    }

    void Update() //��ü ���� ����.
    {
        #region Move Logics

        Vector3 targetPosition = player.position - player.forward * distanceBehindPlayer; // �÷��̾ Ÿ������ �����ʰ�, �� �ڸ� Ÿ������.
        // �÷��̾ ���� ���� ���
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        #endregion

        #region UI updates

        if(lockOnUIImage != null)
        {
            // �� ��ü�� ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

            // ���� �÷��̾� ���� �Ÿ� ���
            float distanceToTarget = Vector3.Distance(mainCamera.transform.position, transform.position);

            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                if (distanceToTarget <= maxDistance)
                {
                    lockOnUIRectTransform.gameObject.SetActive(true);

                    // �Ÿ� ������� ũ�� ����
                    float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToTarget);
                    lockOnUIRectTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, t);

                    // ȭ�� ��ǥ�� UI ĵ���� ��ǥ�� ��ȯ
                    lockOnUIRectTransform.position = screenPos;

                    if (isLockedOn)
                    {
                        // ���� ����

                        if (isFlickering)
                        {
                            StopCoroutine(FlickerEffect());
                            isFlickering = false;
                        }
                    }
                    else if (isTargeted) //�Ͽ� �ȵ�, Ÿ�ٵ�.
                    {
                        // Ÿ���� ����
                        if (!isFlickering)
                        {
                            StartCoroutine(FlickerEffect());
                        }
                    }
                    else
                    {
                        // Ÿ���õ��� ���� ����
                        
                            StopCoroutine(FlickerEffect());
                            isFlickering = false;
                        
                    }



                        distanceText.text = ((int)(distanceToTarget * 10)).ToString();
                }
                else
                {
                    lockOnUIRectTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                // �Ÿ� ���� ���̸� UI �����
                lockOnUIRectTransform.gameObject.SetActive(false);
            }

            
        }



        #endregion

        sibal = lockOnUIImage.color;
    }









    // Ÿ������ ������ �� ȣ��
    public void OnTargeted()
    {
        isTargeted = true;

        if (!aircraftInfoUIobject.activeSelf) //���� ���� Ȱ��ȭ
        {
            aircraftInfoUIobject.SetActive(true);
        }

    }

    // Ÿ�ٿ��� ��� �� ȣ��
    public void OnUntargeted()
    {
        isTargeted = false;

        lockOnUIImage.color = normalColor;

        if (aircraftInfoUIobject.activeSelf)
        {
            aircraftInfoUIobject.SetActive(false);
        }
    }

    public void OnLockedOn()
    {
        isLockedOn = true;

        lockOnUIImage.color = lockedOnColor; // ���� UI ����������.
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;

        if (!aircraftInfoUIobject.activeSelf) // ���� ���� UI Ȱ��ȭ.
        {
            aircraftInfoUIobject.SetActive(true);
            
        }
        //�Ͽ�
    }

    public void OnLockedOff()
    {
        isLockedOn = false;

        distanceText.color = normalColor;
        aircraftNameText.color = normalColor;
        //�� ����

    }

    private IEnumerator FlickerEffect() //Ÿ�������� �Ͽµ��� �ʾ��� ��, ui�� �����̴� ȿ�� ����.
    {
        isFlickering = true;

        while (isFlickering)
        {
            lockOnUIImage.color = transparentColor;
            yield return new WaitForSeconds(0.5f);
            lockOnUIImage.color = normalColor;
            yield return new WaitForSeconds(0.5f);
        }
    }





}
