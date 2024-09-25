using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class RotateTransform : MonoBehaviour
        {
            public bool isActive = true;

            [Space]
            public Transform transf;
            public float value;

            [Space]
            public float valueFactor = 1;
            public float valueOffSet = 0;
            public float maxValue = 1, minValue = -1;

            [Space(15)]
            public bool rotateX = true;
            public float currentX = 0, factorX = 1, offSetX = 0;
            public bool clampX = true;
            public float maxX = 45, minX = -45;

            [Space(15)]
            public bool rotateY = false;
            public float currentY = 0, factorY = 1, offSetY = 0;
            public bool clampY = true;
            public float maxY = 45, minY = -45;

            [Space(15)]
            public bool rotateZ = false;
            public float currentZ = 0, factorZ = 1, offSetZ = 0;
            public bool clampZ = true;
            public float maxZ = 45, minZ = -45;


            [ContextMenu("Update Value")]
            void contextUpdateValue() { setValue(value); }

            //Update to Inicial Value
            void OnEnable()
            {
                if (!transf) transf = GetComponent<Transform>();
                if (isActive && gameObject.activeInHierarchy) setValue(value);
            }
            //

            //void Update() { setValue(value); }

            //External call to set value
            public void setValue(float newValue)
            {
                if (!isActive) return;

                //Update Value
                value = valueOffSet + valueFactor * newValue;

                //Rotate X
                if (rotateX)
                {
                    //currentX = offSetX + factorX * value;
                    currentX = offSetX + factorX * value * (maxX - minX) / (maxValue - minValue);
                    if (clampX) currentX = Mathf.Clamp(currentX, minX, maxX);

                    if (transf != null && !float.IsNaN(currentX)) transf.localRotation = Quaternion.Euler(currentX, transf.localRotation.y, transf.localRotation.z);
                }
                //

                //Rotate Y
                if (rotateY)
                {
                    currentY = offSetY + factorY * value * (maxY - minY) / (maxValue - minValue);
                    if (clampY) currentY = Mathf.Clamp(currentY, minY, maxY);

                    if (transf != null && !float.IsNaN(currentY)) transf.localRotation = Quaternion.Euler(transf.localRotation.x, currentY, transf.localRotation.z);
                }
                //

                //Rotate Y
                if (rotateZ)
                {
                    currentZ = offSetZ + factorZ * value * (maxZ - minZ) / (maxValue - minValue);
                    if (clampZ) currentZ = Mathf.Clamp(currentZ, minZ, maxZ);

                    if (transf != null && !float.IsNaN(currentZ)) transf.localRotation = Quaternion.Euler(transf.localRotation.x, transf.localRotation.y, currentZ);
                }
                //


            }
            //

        }
    }
}