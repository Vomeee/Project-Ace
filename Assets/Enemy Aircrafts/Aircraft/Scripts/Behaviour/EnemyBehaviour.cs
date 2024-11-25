using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    
    [SerializeField] Transform currentTarget; 
    [SerializeField] Transform player; // �÷��̾��� Transform
    [SerializeField] Transform friendTransform; //������ transform.
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
    float shootRate; //�����ȿ� ������ �� Ȯ��
    [SerializeField]
    float friendShootRate; //���Ḧ �� Ȯ��, �ſ� ����.
    [SerializeField]
    bool canShoot; // �̻��� �߻� ���� ����
    [SerializeField]
    float currentMissileCoolDown; //���� ��ٿ� ���庯��
    [SerializeField]
    float missileCoolDown; // �̻��� Ȱ��ȭ �ð�
    [SerializeField]
    float timeUntilShoot; //�̻��� �߻��� �����ؾ��ϴ� �Ͽ½ð�
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
                GameObject enemyMsl = Instantiate(enemyMissilePrefab, transform.position, transform.rotation); //�̻��� ����
                EnemySTDM missileScript = enemyMsl.GetComponent<EnemySTDM>(); //���߿� �̻��� �̸� �ٲ㼭 ���� �����.

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

            // ���ῡ�� �̻��� �߻�.
            //if (currentTarget == currentFriendTarget && canShoot && Random.Range(0, 1) < shootRate)
            //{
            //GameObject enemyMsl = Instantiate(enemyMissilePrefab, transform.position, transform.rotation); //�̻��� ����
            //EnemySTDM missileScript = enemyMsl.GetComponent<EnemySTDM>(); //���߿� �̻��� �̸� �ٲ㼭 ���� �����.

            Debug.Log("missile launch");
                //missileScript.Launch(currentTarget, maxSpeed, null);

                //canShoot = false;
                //currentMissileCoolDown = 0;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) //�÷��̾�
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
    [SerializeField] Color transparentColor = new Color(1f,0f,0f,0f); // Ÿ���� ������ ���� ����

    [SerializeField] GameObject aircraftInfoUIobject; // ���� ���� UI ��Ʈ
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
    public bool isTargeted = false; // Ÿ������ �����Ǿ����� ����
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
            // �� ��ü�� ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

            // ���� �÷��̾� ���� �Ÿ� ���
            distanceToTarget = Vector3.Distance(mainCamera.transform.position, transform.position);

            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                if (distanceToTarget <= maxDistance)
                {
                    lockOnUIRectTransform.gameObject.SetActive(true);

                    // ȭ�� ��ǥ�� UI ĵ���� ��ǥ�� ��ȯ
                    lockOnUIRectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);

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
                    distanceText.text = ((int)(distanceToTarget * 5)).ToString();
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


        CheckWaypoint();

        Rotate();
        ZAxisRotate();
        Move();

    }

    void Update() //��ü ���� ����.
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
        player = playerTransform; //�ʼ�, ���� ���� �߰��� initalize �ٿ�ȭ �ʿ�,
        this.targetingSystem = targettingSystem; //�ݵ�� �ʿ���.
        this.tagController = tagController;
        this.gameManagement = gm;
        waypointObject = waypointObj;
        enemyMissilePrefab = enemyMissile;
        this.warningController = warningController;
        this.plot = plot;
    }

    #region target, lock controls

    // Ÿ������ ������ �� ȣ��
    public void OnTargeted()
    {
        isTargeted = true;

        if (!aircraftInfoUIobject.activeSelf) //���� ���� Ȱ��ȭ
        {
            aircraftInfoUIobject.SetActive(true);
        }

        StartCoroutine(MinimapEffect());
        

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

        StopCoroutine(MinimapEffect());
        isMinimapFlickering = false;
    }

    
    public void OnLockedOn(float lockingOnTime)
    {
        float scaleFactor = lockingOnTime > maxLockingTime? maxLockingTime : lockingOnTime; //max = maybe 3.
        float normalizedScaleFactor = scaleFactor / maxLockingTime;

        // localScale�� minScale���� maxScale ���� ������ ������ ����
        float scale = Mathf.Lerp(maxScale, minScale, normalizedScaleFactor);

        // �̹����� localScale ����
        lockOnUIImage.transform.localScale = new Vector3(scale, scale, scale);

        lockOnUIImage.color = lockedOnColor; // ���� UI ����������.
        if (isLockedOn) return;
        StopCoroutine(FlickerEffect());
        isFlickering = false;

        isLockedOn = true;

        lockOnUIImage.color = lockedOnColor; // ���� UI ����������.
        distanceText.color = lockedOnColor;
        aircraftNameText.color = lockedOnColor;
        aceNameText.color = lockedOnColor;

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
        aceNameText.color = normalColor;


        if (lockOnSound.isPlaying)
        {
            lockOnSound.Stop();
        }
        //�� ����
        lockOnSquare.SetActive(false);
        lockOnUIImage.transform.localScale = new Vector3(minScale, minScale, minScale);


    }

    #endregion

    #region coroutines(minimap, targetBox)

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

    private IEnumerator MinimapEffect() //Ÿ�������� �Ͽµ��� �ʾ��� ��, ui�� �����̴� ȿ�� ����.
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
        targetingSystem.RemoveTarget(gameObject.transform); //���� Ÿ�� ����Ʈ���� ����
        tagController.ShowDestroyedTag(); //destroyed �±� ǥ��
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
