using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Pixy : MonoBehaviour
{
    [SerializeField] Transform followTransformPoint; //플레이어 뒤쪽에 있는 point. 시작점.
    [SerializeField] Transform currentTargetTransform;

    [Header("movingAI instances")]
    #region move references
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float defaultSpeed;

    float speed;

    [SerializeField] float speedLerpAmount;
    [SerializeField] float turningForce;
    [SerializeField] float turningTime;

    [SerializeField] List<Transform> initialWaypoints;
    Queue<Transform> waypointQueue;

    [SerializeField] Vector3 currentWaypoint;

    [SerializeField] float prevWaypointDistance;
    [SerializeField] float waypointDistance;
    [SerializeField] bool isComingClose;

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

    #endregion
    [Space]
    [Header("UI references")]
    #region ui references
    float distanceToPlayer;
    [SerializeField] float maxUiShowingDistance;
    [SerializeField] float minUiShowingDitstance;
    [SerializeField] RectTransform lockOnUIRectTransform;
    [SerializeField] string tacName;
    [SerializeField] string callSign;
    [SerializeField] Text tacNameText;
    [SerializeField] Text callSignText;
    #endregion

    [SerializeField]
    int friendState; //0 -> follow player. //1 -> attack enemy.



    void Start()
    {
        //Set Start Position
        Vector3 startPosition = followTransformPoint.position;
        startPosition.z -= 10f;
        transform.position = startPosition;
        transform.rotation = followTransformPoint.rotation;

        //Set start state that follows player.
        friendState = 0; //시작은 당연히 플레이어 따라다니기.

        //Set moveset initial state.
        isComingClose = true;
        speed = defaultSpeed;
        turningTime = 1 / turningForce;

        waypointQueue = new Queue<Transform>();
        foreach (Transform t in initialWaypoints)
        {
            waypointQueue.Enqueue(t);
        }
        CreateWaypoint();

        //UI initial state.
        tacNameText.text = tacName;
        callSignText.text = callSign;
    }



    void Update()
    {
        //UI update.
        UIUpdate();


        //Aircraft moves.
        CheckWaypoint();
        Rotate();
        ZAxisRotate();
        Move();
    }

    #region move methods.

    void CreateWaypoint()
    {
        if (friendState == 0) //following player.
        {
            if(waypointQueue.Count == 0)
            {
                currentWaypoint = followTransformPoint.transform.position;
            }
            else
            {
                currentWaypoint = waypointQueue.Dequeue().position;
            }

        }
        else if (friendState == 1) // tracking enemy.
        {
            if (waypointQueue.Count == 0)
            {
                currentWaypoint = currentTargetTransform.transform.position;
            }
            else
            {
                currentWaypoint = waypointQueue.Dequeue().position;
            }
        }

        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);
        prevWaypointDistance = waypointDistance;
        isComingClose = false;
    }
    void CheckWaypoint()
    {
        if (currentWaypoint == null) return;
        waypointDistance = Vector3.Distance(transform.position, currentWaypoint);

        if (waypointDistance >= prevWaypointDistance) // Aircraft is going farther from the waypoint
        {
            if (isComingClose == true)
            {
                CreateWaypoint();
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
        // 목표 속도를 향해 부드럽게 속도 조절
        //float targetSpeed = (isComingClose) ? minSpeed : maxSpeed;
        //speed = Mathf.Lerp(speed, targetSpeed, speedLerpAmount * Time.deltaTime);

        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Enemy") && friendState == 0) //딱 하나만 받을 수 있음
        {
            Debug.Log("friendState is now 1.");
            friendState = 1;
            currentTargetTransform = other.gameObject.GetComponent<Transform>();

            if (currentTargetTransform == null)
            {
                Debug.Log("적 정보를 못받아요");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentTargetTransform = null;
            friendState = 0;
        }
    }

    #endregion

    void UIUpdate() //friend -> 위치만 옮기면 될듯.
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); //기체 위치를 화면(메인카메라) 좌표로.

        distanceToPlayer = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            if (distanceToPlayer < maxUiShowingDistance) //UI가 표출되는 거리 이내.
            {
                lockOnUIRectTransform.gameObject.SetActive(true);

                // 화면 좌표를 UI 캔버스 좌표로 변환
                lockOnUIRectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
            }
        }
        else
        {
            lockOnUIRectTransform.gameObject.SetActive(false);
        }
           
    }

    
}
