using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STDM : MonoBehaviour
{
    public Transform target;
    public float turningForce;

    public float maxSpeed;
    public float accelAmount;
    public float lifetime;
    public float speed;

    public void Launch(Transform target, float launchSpeed)
    {
        this.target = target;
        speed = launchSpeed;
    }

    void LookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (speed < maxSpeed)
        {
            speed += accelAmount * Time.deltaTime;
        }

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        LookAtTarget();
    }
}
