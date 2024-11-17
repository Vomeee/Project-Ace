using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ALLY : MonoBehaviour
{
    //[SerializeField] Transform startPos;
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

    //area set.
    [SerializeField]
    float minMovingRangeX;
    [SerializeField]
    float minMovingRangeZ;
    [SerializeField]
    float maxMovingRangeX;
    [SerializeField]
    float maxMovingRangeZ;
    [SerializeField]
    float movingRangeDistance;

    #endregion
    [Space]
    [Header("UI references")]
    #region ui references
    float distanceToPlayer;
    [SerializeField] float maxUiShowingDistance;
    [SerializeField] float minUiShowingDitstance;
    [SerializeField] RectTransform lockOnUIRectTransform;
    #endregion

    void Start()
    {
        //Set Start Position

        //Set Moving range
        minMovingRangeX = transform.position.x - movingRangeDistance;
        minMovingRangeZ = transform.position.z - movingRangeDistance;
        maxMovingRangeX = transform.position.x + movingRangeDistance;
        maxMovingRangeZ = transform.position.z + movingRangeDistance;

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

        lockOnUIRectTransform.gameObject.SetActive(false);
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
        if (waypointPosition.x < minMovingRangeX)
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
        // ��ǥ �ӵ��� ���� �ε巴�� �ӵ� ����
        //float targetSpeed = (isComingClose) ? minSpeed : maxSpeed;
        //speed = Mathf.Lerp(speed, targetSpeed, speedLerpAmount * Time.deltaTime);

        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(other.tag);
    //    if (other.CompareTag("Enemy") && friendState == 0) //�� �ϳ��� ���� �� ����
    //    {
    //        Debug.Log("friendState is now 1.");

    //        currentTargetTransform = other.gameObject.GetComponent<Transform>();
    //        friendState = 1;
    //        if (currentTargetTransform == null)
    //        {
    //            Debug.Log("�� ������ ���޾ƿ�");
    //        }
    //    }
    //}

    #endregion

    void UIUpdate() //friend -> ��ġ�� �ű�� �ɵ�.
    {
        if (Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); //��ü ��ġ�� ȭ��(����ī�޶�) ��ǥ��.

            distanceToPlayer = Vector3.Distance(Camera.main.transform.position, transform.position);

            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                if (distanceToPlayer < maxUiShowingDistance) //UI�� ǥ��Ǵ� �Ÿ� �̳�.
                {
                    lockOnUIRectTransform.gameObject.SetActive(true);

                    // ȭ�� ��ǥ�� UI ĵ���� ��ǥ�� ��ȯ
                    lockOnUIRectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
                }
                else
                {
                    lockOnUIRectTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                lockOnUIRectTransform.gameObject.SetActive(false);
            }

        }

    }
}
