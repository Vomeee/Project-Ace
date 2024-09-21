using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    
    [SerializeField] Transform player; // �÷��̾��� Transform

    [Header("EnemyInfo")]
    #region EnemyInfo
    public string aircraftName;
    [SerializeField] int aircraftHP = 100;
    [SerializeField] public int aircraftScore = 240;
    #endregion

    [Header("Moving logic instances")]
    #region aircraft moving logic's variables and referecnces

    [SerializeField] float speed = 10f; // �� ������� �ӵ�
    [SerializeField] float rotationSpeed = 2f; // ȸ�� �ӵ�

    [SerializeField] float distanceBehindPlayer = 10f; // �÷��̾��� �ڸ� ����

    #endregion
    
    [Space]

    [Header("UI References")]
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

    [SerializeField] GameObject lockOnSquare;
    [SerializeField] Color sibal;

    [SerializeField] TagController tagController;
    [SerializeField] GameManagement gameManagement;

    #endregion

    [Header("enemyState")]
    public bool isTargeted = false; // Ÿ������ �����Ǿ����� ����
    public bool isLockedOn = false;

    [SerializeField] TargettingSystem targetingSystem;

    [Header("Sound Sources")]
    [SerializeField] AudioSource lockOnSound;


    void Start()
    { 
        mainCamera = Camera.main;   

        distanceText.color = normalColor;
        aircraftNameText.text = aircraftName;
        aircraftNameText.color = normalColor;
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

    public void initializeInstance(Transform playerTransform, TargettingSystem targettingSystem, TagController tagController, GameManagement gm)
    {
        player = playerTransform;
        this.targetingSystem = targettingSystem;
        this.tagController = tagController;
        this.gameManagement = gm;
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
        lockOnUIImage.color = lockedOnColor; // ���� UI ����������.
        if (isLockedOn) return;
        StopCoroutine(FlickerEffect());
        isFlickering = false;

        isLockedOn = true;

        lockOnUIImage.color = lockedOnColor; // ���� UI ����������.
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;

        if (!aircraftInfoUIobject.activeSelf) // ���� ���� UI Ȱ��ȭ.
        {
            aircraftInfoUIobject.SetActive(true);
        }

        lockOnSound.Play();
        lockOnSquare.SetActive(true);
        //�Ͽ�
    }

    public void OnLockedOff()
    {
        if (!isLockedOn) return;
        isLockedOn = false;

        lockOnUIImage.color = normalColor;
        distanceText.color = normalColor;
        aircraftNameText.color = normalColor;


        if (lockOnSound.isPlaying)
        {
            lockOnSound.Stop();
        }
        //�� ����
        lockOnSquare.SetActive(false);

        
    }

    private IEnumerator FlickerEffect() //Ÿ�������� �Ͽµ��� �ʾ��� ��, ui�� �����̴� ȿ�� ����.
    {
        isFlickering = true;

        while (isFlickering)
        {
            lockOnUIImage.color = transparentColor;
            yield return new WaitForSeconds(0.25f);
            lockOnUIImage.color = normalColor;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet"))
        {
            AircraftDamage(15);
            if(aircraftHP > 0)
            {
                tagController.ShowHitTag();
            }
            
            Debug.Log("gun hit(aircraft detect)");

        }
        if (collision.gameObject.CompareTag("stdm"))
        {
            AircraftDamage(70);
            if (aircraftHP > 0)
            {
                tagController.ShowHitTag();
            }
            Debug.Log("msl hit(aircraft detect)");

        }
    }

    void AircraftDamage(int damage)
    {
        aircraftHP -= damage;

        if (aircraftHP <= 0)
        {
            AircraftDestroyed();
        }
    }

    [SerializeField] GameObject explodeEffect;
    void AircraftDestroyed()
    {
        if (explodeEffect) Instantiate(explodeEffect, transform.position, Quaternion.identity, null);
        targetingSystem.RemoveTarget(gameObject.transform); //���� Ÿ�� ����Ʈ���� ����
        tagController.ShowDestroyedTag(); //destroyed �±� ǥ��
        gameManagement.UpdateScore(aircraftScore);
        Destroy(gameObject);
    }



}
