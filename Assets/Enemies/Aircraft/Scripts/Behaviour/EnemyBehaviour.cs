using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    
    [SerializeField] Transform player; // 플레이어의 Transform

    #region aircraft moving logic's variables and referecnces

    [SerializeField] float speed = 10f; // 적 비행기의 속도
    [SerializeField] float rotationSpeed = 2f; // 회전 속도

    [SerializeField] float distanceBehindPlayer = 10f; // 플레이어의 뒤를 설정

    #endregion
    
    [Space]

    #region reactive UI references and variables
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform lockOnUIRectTransform;
    [SerializeField] Image lockOnUIImage;

    [SerializeField] Color lockedOnColor = Color.red;
    [SerializeField] Color normalColor = Color.green;
    [SerializeField] Color transparentColor = new Color(1f,0f,0f,0f); // 타게팅 상태일 때의 색상

    [SerializeField] GameObject aircraftInfoUIobject; // 적기 정보 UI 세트
    [SerializeField] Text distanceText;
    [SerializeField] Text aircraftNameText;

    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 650f;

    [SerializeField] Vector2 minSize = new Vector2(10f, 10f); // UI의 최소 크기
    [SerializeField] Vector2 maxSize = new Vector2(100f, 100f); // UI의 최대 크기

    [SerializeField] bool isFlickering = false;

    [SerializeField] Color sibal;

    #endregion

    public bool isTargeted = false; // 타겟으로 지정되었는지 여부
    public bool isLockedOn = false;

    void Start()
    {
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;
    }

    void Update() //자체 비행 로직.
    {
        #region Move Logics

        Vector3 targetPosition = player.position - player.forward * distanceBehindPlayer; // 플레이어를 타겟으로 하지않고, 그 뒤를 타겟으로.
        // 플레이어를 향한 방향 계산
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        #endregion

        #region UI updates

        if(lockOnUIImage != null)
        {
            // 적 기체의 월드 좌표를 화면 좌표로 변환
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

            // 적과 플레이어 간의 거리 계산
            float distanceToTarget = Vector3.Distance(mainCamera.transform.position, transform.position);

            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                if (distanceToTarget <= maxDistance)
                {
                    lockOnUIRectTransform.gameObject.SetActive(true);

                    // 거리 기반으로 크기 조정
                    float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToTarget);
                    lockOnUIRectTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, t);

                    // 화면 좌표를 UI 캔버스 좌표로 변환
                    lockOnUIRectTransform.position = screenPos;

                    if (isLockedOn)
                    {
                        // 락온 상태

                        if (isFlickering)
                        {
                            StopCoroutine(FlickerEffect());
                            isFlickering = false;
                        }
                    }
                    else if (isTargeted) //록온 안됨, 타겟됨.
                    {
                        // 타게팅 상태
                        if (!isFlickering)
                        {
                            StartCoroutine(FlickerEffect());
                        }
                    }
                    else
                    {
                        // 타겟팅되지 않은 상태
                        
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
                // 거리 범위 밖이면 UI 숨기기
                lockOnUIRectTransform.gameObject.SetActive(false);
            }

            
        }



        #endregion

        sibal = lockOnUIImage.color;
    }









    // 타겟으로 지정될 때 호출
    public void OnTargeted()
    {
        isTargeted = true;

        if (!aircraftInfoUIobject.activeSelf) //적기 정보 활성화
        {
            aircraftInfoUIobject.SetActive(true);
        }

    }

    // 타겟에서 벗어날 때 호출
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

        lockOnUIImage.color = lockedOnColor; // 적기 UI 붉은색으로.
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;

        if (!aircraftInfoUIobject.activeSelf) // 적기 정보 UI 활성화.
        {
            aircraftInfoUIobject.SetActive(true);
            
        }
        //록온
    }

    public void OnLockedOff()
    {
        isLockedOn = false;

        distanceText.color = normalColor;
        aircraftNameText.color = normalColor;
        //록 오프

    }

    private IEnumerator FlickerEffect() //타겟이지만 록온되지 않았을 때, ui가 깜빡이는 효과 구현.
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
