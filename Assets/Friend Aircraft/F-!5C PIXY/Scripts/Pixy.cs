using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixy : MonoBehaviour
{
    [SerializeField] Transform followTransformPoint; //플레이어 뒤쪽에 있는 point. 시작점.
    [SerializeField] Transform currentTargetTransform;

    [Header("movingAI instances")]
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float defaultSpeed;

    float speed;

    [SerializeField] float speedLerpAmount;
    [SerializeField] float turningForce;
    [SerializeField] float turningTime;

    [SerializeField] List<Transform> initialWaypoints;
    Queue<Transform> waypointQueue;

    Vector3 currentWaypoint;

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
    int friendState; //0 -> follow player. //1 -> attack enemy.



    void Start()
    {
        transform.position = followTransformPoint.position;
        transform.rotation = followTransformPoint.rotation;
        friendState = 0; //시작은 당연히 플레이어 따라다니기.

        speed = defaultSpeed;
        turningTime = 1 / turningForce;

        waypointQueue = new Queue<Transform>();
        foreach (Transform t in initialWaypoints)
        {
            waypointQueue.Enqueue(t);
        }
        CreateWaypoint();
    }

   
    void Update()
    {
        CreateWaypoint();
        Rotate();
        ZAxisRotate();
        Move();
    }

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
        else if (friendState == 1) // enemy tracking player.
        {
            if (waypointQueue.Count == 0)
            {
                //Instantiate(waypointObject, currentTargetTransform.transform.position, Quaternion.identity);
            }
            else
            {
                currentWaypoint = waypointQueue.Dequeue().position;
            }
        }
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
}
