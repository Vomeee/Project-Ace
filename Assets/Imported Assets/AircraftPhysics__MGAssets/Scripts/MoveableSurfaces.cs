using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class MoveableSurfaces : MonoBehaviour
        {
            public bool isActive = true;
            public FlightScript flightScript;

            [Header("Input Torque")]
            [Space]
            public Vector3 torqueSpeed = new Vector3(0.1f, 0.1f, 0.1f);
            public RotateTransform[] rotateXTorque;
            public RotateTransform[] rotateYTorque;
            public RotateTransform[] rotateZTorque;


            [Header("Input Force")]
            [Space]
            public Vector3 forceSpeed = new Vector3(0.1f, 0.1f, 0.1f);
            public RotateTransform[] rotateXForce;
            public RotateTransform[] rotateYForce;
            public RotateTransform[] rotateZForce;


            [Header("Flaps")]
            [Space]
            public float flapSpeed = 0.05f;
            public RotateTransform[] flaps;


            [Header("Propellers & Tires")]
            [Space]
            public float propellersSpeed = 1f;
            public float idleProp = 0.1f;
            public SpinTransform[] propellers;

            [Space]
            public GameObject[] engines;

            [Space]
            public float tireSpeed = 1f;
            public float tireSlowingSpeed = 0.01f;
            public SpinTransform[] tires;


            [Header("Air Brake")]
            [Space]
            public float brakeSpeed = 0.05f;
            public RotateTransform[] brakes;


            [Space(15)]
            [Header("Current Input - Read Only!")]
            public Vector3 inputTorque;
            public Vector3 inputForce;
            public float throttle;
            public float flap;
            public float tire;
            public float brake;
            public bool engine;

            [Space]
            public Vector3 currentInputTorque;
            public Vector3 currentInputForce;
            public float currentThrottle;
            public float currentFlap;
            public float currentTire;
            public float currentBrake;



            // Updates All Moveable Surfaces
            void Update()
            {
                if (!isActive) return;

                inputTorque = flightScript.getInputTorque();
                inputForce = flightScript.getInputForce();
                flap = flightScript.getFlaps();
                tire = flightScript.getIsGrounded() ? flightScript.getForwardSpeed() : 0;
                brake = flightScript.getIsBraking() ? 1 : 0;
                throttle = flightScript.getThrottle();
                engine = flightScript.getEngineOn();

                lerpCurrentInput();

                if (rotateXTorque.Length > 0) for (int i = 0; i < rotateXTorque.Length; i++) if (rotateXTorque[i] != null) rotateXTorque[i].setValue(currentInputTorque.x);
                if (rotateYTorque.Length > 0) for (int i = 0; i < rotateYTorque.Length; i++) if (rotateYTorque[i] != null) rotateYTorque[i].setValue(currentInputTorque.y);
                if (rotateZTorque.Length > 0) for (int i = 0; i < rotateZTorque.Length; i++) if (rotateZTorque[i] != null) rotateZTorque[i].setValue(currentInputTorque.z);

                if (rotateXForce.Length > 0) for (int i = 0; i < rotateXForce.Length; i++) if (rotateXForce[i] != null) rotateXForce[i].setValue(currentInputForce.x);
                if (rotateYForce.Length > 0) for (int i = 0; i < rotateYForce.Length; i++) if (rotateYForce[i] != null) rotateYForce[i].setValue(currentInputForce.y);
                if (rotateZForce.Length > 0) for (int i = 0; i < rotateZForce.Length; i++) if (rotateZForce[i] != null) rotateZForce[i].setValue(currentInputForce.z);

                if (flaps.Length > 0) for (int i = 0; i < flaps.Length; i++) if (flaps[i] != null) flaps[i].setValue(currentFlap);
                if (tires.Length > 0) for (int i = 0; i < tires.Length; i++) if (tires[i] != null) tires[i].factor = tireSpeed * currentTire;
                if (brakes.Length > 0) for (int i = 0; i < brakes.Length; i++) if (brakes[i] != null) brakes[i].setValue(currentBrake);

                if (propellers.Length > 0) for (int i = 0; i <= propellers.Length - 1; i++) if (propellers[i] != null) propellers[i].factor = propellersSpeed * currentThrottle;
                if (engines.Length > 0) for (int i = 0; i <= engines.Length - 1; i++) if (engines[i] != null) engines[i].SetActive(engine);

            }
            // Updates All Moveable Surfaces


            // Executes Lerp to smooth Current Input
            void lerpCurrentInput()
            {
                currentInputTorque = new Vector3
                    (
                    Mathf.Lerp(currentInputTorque.x, inputTorque.x, torqueSpeed.x),
                    Mathf.Lerp(currentInputTorque.y, inputTorque.y, torqueSpeed.y),
                    Mathf.Lerp(currentInputTorque.z, inputTorque.z, torqueSpeed.z)
                    );

                currentInputForce = new Vector3
                    (
                    Mathf.Lerp(currentInputForce.x, inputForce.x, forceSpeed.x),
                    Mathf.Lerp(currentInputForce.y, inputForce.y, forceSpeed.y),
                    Mathf.Lerp(currentInputForce.z, inputForce.z, forceSpeed.z)
                    );

                currentFlap = Mathf.Lerp(currentFlap, flap, flapSpeed);
                currentBrake = Mathf.Lerp(currentBrake, brake, brakeSpeed);

                if (!flightScript.getIsGrounded()) currentTire = Mathf.Lerp(currentTire, 0, tireSlowingSpeed);
                else currentTire = tire;

                if (!flightScript.getIsDamaged() && !flightScript.getIsBingoFuel()) currentThrottle = Mathf.Lerp(idleProp, 1f, Mathf.Abs(throttle)) * Mathf.Sign(throttle);
                else currentThrottle = Mathf.Lerp(0, 1f, Mathf.Abs(throttle)) * Mathf.Sign(throttle);


            }
            // Executes Lerp to smooth Current Input



        }
    }
}