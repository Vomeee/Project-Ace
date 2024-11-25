using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    
    [SerializeField] Transform currentTarget; 
    [SerializeField] Transform player; // 플레이어의 Transform
    [SerializeField] Transform friendTransform; //동료의 transform.
    [SerializeField] TargettingSystem targetingSystem;
    [SerializeField] WarningController warningController;
    [SerializeField] GameManagement gm;
    float distanceToTarget;

    [Header("movingAI instances")]
    #region enemy moves
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float defaultSpeed;

    float speed;

    [SerializeField] float speedLerpAmount;
    [SerializeField] float turningForce;
    [SerializeField] float turningTime;

    [SerializeField] public List<Transform> initialWaypoints;
    public Queue<Transform> waypointQueue;

    [SerializeField] Vector3 currentWaypoint;

    float prevWaypointDistance;
    float waypointDistance;
    bool isComingClose;

    float prevRotY;
    float currRotY;
    float rotateAmount;
    float zRotateValue;

    // Z Rotate Values
    [SerializeField]
    float zRotateMaxThreshold = 0.5f;
    [SerializeField]
    float zRotateAmount = 90;

    [SerializeField]
    float newWaypointDistance;
    [SerializeField]
    float waypointMinHeight;
    [SerializeField]
    float waypointMaxHeight;

    [SerializeField]
    GameObject waypointObject;

    [SerializeField]
    float playerTrackingRate;
    [SerializeField]
    float trackingStartDistance;
    [SerializeField]
    float shootRate; //범위안에 들어오면 쏠 확률
    [SerializeField]
    float friendShootRate; //동료를 쏠 확률, 매우 낮게.
    [SerializeField]
    bool canShoot; // 미사일 발사 가능 여부
    [SerializeField]
    float currentMissileCoolDown; //현재 쿨다운 저장변수
    [SerializeField]
    float missileCoolDown; // 미사일 활성화 시간
    [SerializeField]
    float timeUntilShoot; //미사일 발사전 유지해야하는 록온시간
    [SerializeField]
    int enemyState; // 0 : free flying, 1 : tracking player, 2 : evading

    [SerializeField]
    GameObject enemyMissilePrefab;
    [SerializeField]
    BoxCollider attackRangeBox;
    [SerializeField]
    float movingRangeDistance;

    //area set.
    [SerializeField]
    float minMovingRangeX;
    [SerializeField]
    float minMovingRangeZ;
    [SerializeField]
    float maxMovingRangeX;
    [SerializeField]
    float maxMovingRangeZ;

    [SerializeField] float maxLockingTime = 3f;
    [SerializeField] float minScale = 0.7f;
    [SerializeField] float maxScale = 1.5f;


    void ChangeWaypoint()
    {
        if (waypointQueue.Count == 0)
        {
            float trackingValue = UnityEngine.Random.Range(0f, 1f);
            if (trackingValue > playerTrackingRate && trackingStartDistance > distanceToTarget)
            {
                //Debug.Log("now Attacking");
                enemyState = 1;
            }
            else
            {
                enemyState = 0;
            }

            
            CreateWaypoint();            
        }
        else
        {
            currentWaypoint = waypointQueue.Dequeue().position;
        }

        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);
        prevWaypointDistance = waypointDistance;
        isComingClose = false;
    }

    void CreateWaypoint()
    {
        if (enemyState == 0) //free fly state make waypoint
        {
            float distance = UnityEngine.Random.Range(newWaypointDistance * 0.7f, newWaypointDistance);
            float height = UnityEngine.Random.Range(waypointMinHeight, waypointMaxHeight);
            float angle = UnityEngine.Random.Range(0, 360);
            Vector3 directionVector = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            Vector3 waypointPosition = transform.position + directionVector * distance;

            RaycastHit hit;
            Physics.Raycast(waypointPosition, Vector3.down, out hit);

            if (hit.distance != 0)
            {
                waypointPosition.y += height - hit.distance;
            }
            // New waypoint is below ground
            else
            {
                Physics.Raycast(waypointPosition, Vector3.up, out hit);
                waypointPosition.y += height + hit.distance;
            }
            if(waypointPosition.x < minMovingRangeX)
            {
                waypointPosition.x = minMovingRangeX;
            }
            else if (waypointPosition.x > maxMovingRangeX)
            {
                waypointPosition.x = maxMovingRangeX;
            }
            if (waypointPosition.z < minMovingRangeZ)
            {
                waypointPosition.z = minMovingRangeZ;
            }
            else if (waypointPosition.z > maxMovingRangeX)
            {
                waypointPosition.z = maxMovingRangeZ;
            }

            Instantiate(waypointObject, waypointPosition, Quaternion.identity);

            currentWaypoint = waypointPosition;

            
        }
        else if (enemyState == 1) // enemy tracking player.
        {
            if (currentTarget != null)
            {
                Vector3 targetPosition = currentTarget.position;
                Instantiate(waypointObject, targetPosition, Quaternion.identity);

                currentWaypoint = targetPosition;
            }
        }
    }

    void CheckWaypoint()
    {
        if (currentWaypoint == null) return;
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);

        if (waypointDistance >= prevWaypointDistance) // Aircraft is going farther from the waypoint
        {
            if (isComingClose == true)
            {
                ChangeWaypoint();
            }
        }
        else
        {
            isComingClose = true;
        }

        prevWaypointDistance = waypointDistance;
    }

    void Rotate()
    {
        if (currentWaypoint == null)
            return;

        Vector3 targetDir = currentWaypoint - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);

        float delta = Quaternion.Angle(transform.rotation, lookRotation);
        if (delta > 0f)
        {
            float lerpAmount = Mathf.SmoothDampAngle(delta, 0.0f, ref rotateAmount, turningTime);
            lerpAmount = 1.0f - (lerpAmount / delta);

            Vector3 eulerAngle = lookRotation.eulerAngles;
            eulerAngle.z += zRotateValue * zRotateAmount;
            lookRotation = Quaternion.Euler(eulerAngle);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lerpAmount);
        }
    }

    void ZAxisRotate()
    {
        currRotY = transform.eulerAngles.y;
        float diff = prevRotY - currRotY;

        if (diff > 180) diff -= 360;
        if (diff < -180) diff += 360;

        prevRotY = transform.eulerAngles.y;
        zRotateValue = Mathf.Lerp(zRotateValue, Mathf.Clamp(diff / zRotateMaxThreshold, -1, 1), turningForce * Time.deltaTime);
    }

    void Move()
    {
        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }

    private bool isLockingOn = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            currentTarget = player;
            if(!isLockingOn)
            {
                warningController.currentEnemyBehind += 1;
            }

            isLockingOn = true;

            StartCoroutine(LockOnWaiting());
            
            if(isLockingOn && canShoot && Random.Range(0,1) < shootRate)
            {
                GameObject enemyMsl = Instantiate(enemyMissilePrefab, transform.position, transform.rotation); //미사일 생성
                EnemySTDM missileScript = enemyMsl.GetComponent<EnemySTDM>(); //나중에 미사일 이름 바꿔서 따로 만들기.

                //Debug.Log("missile launch");
                missileScript.Launch(currentTarget, maxSpeed, warningController);

                
                canShoot = false;
                currentMissileCoolDown = 0;
            }
            
        }
        if (other.CompareTag("Friend"))
        {
            currentTarget = other.GetComponent<Transform>();
            //StartCoroutine(LockOnWaiting());

            // 동료에게 미사일 발사.
            //if (currentTarget == currentFriendTarget && canShoot && Random.Range(0, 1) < shootRate)
            //{
            //GameObject enemyMsl = Instantiate(enemyMissilePrefab, transform.position, transform.rotation); //미사일 생성
            //EnemySTDM missileScript = enemyMsl.GetComponent<EnemySTDM>(); //나중에 미사일 이름 바꿔서 따로 만들기.

            Debug.Log("missile launch");
                //missileScript.Launch(currentTarget, maxSpeed, null);

                //canShoot = false;
                //currentMissileCoolDown = 0;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) //플레이어
        {
            if(isLockingOn)
            {
                warningController.currentEnemyBehind -= 1;
            }
            
            isLockingOn = false;
        }
        else if(other.CompareTag("Friend"))
        {
            currentTarget = player;
        }
        
    }

    IEnumerator LockOnWaiting()
    {
        yield return new WaitForSeconds(timeUntilShoot);
    }



    #endregion


    [Header("EnemyInfo")]
    #region EnemyInfo
    public string aircraftName;
    public string aceName = "";
    [SerializeField] int aircraftHP = 100;
    [SerializeField] public int aircraftScore = 240;
    [SerializeField] bool isTGT;
    
    #endregion
    
    [Space]

    [Header("UI References")]
    #region reactive UI references and variables
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform lockOnUIRectTransform;
    [SerializeField] UnityEngine.UI.Image lockOnUIImage;

    [SerializeField] Color lockedOnColor = Color.red;
    [SerializeField] Color normalColor = Color.green;
    [SerializeField] Color transparentColor = new Color(1f,0f,0f,0f); // 타게팅 상태일 때의 색상

    [SerializeField] GameObject aircraftInfoUIobject; // 적기 정보 UI 세트
    [SerializeField] Text TGTText;
    [SerializeField] Text distanceText;
    [SerializeField] Text aircraftNameText;
    [SerializeField] Text aceNameText;

    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 650f;

    [SerializeField] bool isFlickering = false;
    [SerializeField] bool isMinimapFlickering = false;

    [SerializeField] GameObject lockOnSquare;

    [SerializeField] TagController tagController;
    [SerializeField] GameManagement gameManagement;

    [SerializeField] SpriteRenderer minimapSprite;

    #endregion

    [Header("enemyState")]
    #region states
    public bool isTargeted = false; // 타겟으로 지정되었는지 여부
    public bool isLockedOn = false;
    #endregion


    [Header("Sound Sources")]
    #region Audio Sources
    [SerializeField] AudioSource lockOnSound;
    #endregion

    void Start()
    { 
        mainCamera = Camera.main;

        //area set.
        minMovingRangeX = transform.position.x - movingRangeDistance;
        minMovingRangeZ = transform.position.z - movingRangeDistance;
        maxMovingRangeX = transform.position.x + movingRangeDistance;
        maxMovingRangeZ = transform.position.z + movingRangeDistance;

        waypointQueue = new Queue<Transform>();
        foreach (Transform t in initialWaypoints)
        {
            waypointQueue.Enqueue(t);
        }
        if(waypointQueue.Count == 0) //no initial
        {
            CreateWaypoint();
        }
        else
        {
            ChangeWaypoint();
        }
        

        if (!isTGT)
        {
            TGTText.text = "";
        }
        else
        {
            TGTText.text = "TGT";
        }

        //ui set.
        distanceText.color = normalColor;
        aircraftNameText.text = aircraftName;
        aircraftNameText.color = normalColor;
        aceNameText.text = aceName == null ? "" : aceName;
        aceNameText.color = normalColor;


        //moving set.
        speed = defaultSpeed;
        turningTime = 1 / turningForce;

        
        

        currentMissileCoolDown = 0;

    }

    private void FixedUpdate()
    {
        #region UI updates

        if (lockOnUIImage != null)
        {
            // 적 기체의 월드 좌표를 화면 좌표로 변환
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

            // 적과 플레이어 간의 거리 계산
            distanceToTarget = Vector3.Distance(mainCamera.transform.position, transform.position);

            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                if (distanceToTarget <= maxDistance)
                {
                    lockOnUIRectTransform.gameObject.SetActive(true);

                    // 화면 좌표를 UI 캔버스 좌표로 변환
                    lockOnUIRectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);

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
                    distanceText.text = ((int)(distanceToTarget * 5)).ToString();
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


        CheckWaypoint();

        Rotate();
        ZAxisRotate();
        Move();

    }

    void Update() //자체 비행 로직.
    {

        
        //Missile cooldown update.
        if (!canShoot)
        {
            currentMissileCoolDown += Time.deltaTime;
            
            if(currentMissileCoolDown > missileCoolDown)
            {
                canShoot = true;
            }
        }

        

    }

    public void initializeInstance(Transform playerTransform, TargettingSystem targettingSystem, 
        TagController tagController, GameManagement gm, GameObject waypointObj, GameObject enemyMissile, WarningController warningController, Plot plot)
    {
        player = playerTransform; //필수, 만약 동료 추가시 initalize 다원화 필요,
        this.targetingSystem = targettingSystem; //반드시 필요함.
        this.tagController = tagController;
        this.gameManagement = gm;
        waypointObject = waypointObj;
        enemyMissilePrefab = enemyMissile;
        this.warningController = warningController;
        this.plot = plot;
    }

    #region target, lock controls

    // 타겟으로 지정될 때 호출
    public void OnTargeted()
    {
        isTargeted = true;

        if (!aircraftInfoUIobject.activeSelf) //적기 정보 활성화
        {
            aircraftInfoUIobject.SetActive(true);
        }

        StartCoroutine(MinimapEffect());
        

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

        StopCoroutine(MinimapEffect());
        isMinimapFlickering = false;
    }

    
    public void OnLockedOn(float lockingOnTime)
    {
        float scaleFactor = lockingOnTime > maxLockingTime? maxLockingTime : lockingOnTime; //max = maybe 3.
        float normalizedScaleFactor = scaleFactor / maxLockingTime;

        // localScale을 minScale에서 maxScale 사이 값으로 역방향 보간
        float scale = Mathf.Lerp(maxScale, minScale, normalizedScaleFactor);

        // 이미지의 localScale 조정
        lockOnUIImage.transform.localScale = new Vector3(scale, scale, scale);

        lockOnUIImage.color = lockedOnColor; // 적기 UI 붉은색으로.
        if (isLockedOn) return;
        StopCoroutine(FlickerEffect());
        isFlickering = false;

        isLockedOn = true;

        lockOnUIImage.color = lockedOnColor; // 적기 UI 붉은색으로.
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;
        aceNameText.color = lockedOnColor;

        if (!aircraftInfoUIobject.activeSelf) // 적기 정보 UI 활성화.
        {
            aircraftInfoUIobject.SetActive(true);
        }

        lockOnSound.Play();
        lockOnSquare.SetActive(true);
        //록온
    }

    public void OnLockedOff()
    {
        if (!isLockedOn) return;
        isLockedOn = false;

        lockOnUIImage.color = normalColor;
        distanceText.color = normalColor;
        aircraftNameText.color = normalColor;
        aceNameText.color = normalColor;


        if (lockOnSound.isPlaying)
        {
            lockOnSound.Stop();
        }
        //록 오프
        lockOnSquare.SetActive(false);
        lockOnUIImage.transform.localScale = new Vector3(minScale, minScale, minScale);


    }

    #endregion

    #region coroutines(minimap, targetBox)

    private IEnumerator FlickerEffect() //타겟이지만 록온되지 않았을 때, ui가 깜빡이는 효과 구현.
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

    private IEnumerator MinimapEffect() //타겟이지만 록온되지 않았을 때, ui가 깜빡이는 효과 구현.
    {
        isMinimapFlickering = true;

        while (isMinimapFlickering)
        {
            minimapSprite.color = transparentColor;
            yield return new WaitForSeconds(0.25f);
            minimapSprite.color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }

    #endregion

    #region info controllers

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
        if (collision.gameObject.CompareTag("HPMM"))
        {
            AircraftDamage(102);
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

    [SerializeField] Plot plot;
    [SerializeField] GameObject explodeEffect;
    [SerializeField] bool isDestroyed = false;
    void AircraftDestroyed()
    {
        if (explodeEffect) Instantiate(explodeEffect, transform.position, Quaternion.identity, null);
        targetingSystem.RemoveTarget(gameObject.transform); //현재 타겟 리스트에서 제거
        tagController.ShowDestroyedTag(); //destroyed 태그 표출
        gameManagement.UpdateScore(aircraftScore);
        if(!isDestroyed)
        {
            if (isTGT)
            {
                plot.TGTReduced();
                isDestroyed = true;
            }
            else
            {
                plot.AircraftReduced();
                isDestroyed = true;
            }
        }
        

        Destroy(gameObject);
    }

    #endregion

}
