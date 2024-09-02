using UnityEngine;

namespace MGAssets
{
    public class FlightScript : MonoBehaviour
    {
        public static FlightScript current;

        public delegate void GearDelegate();
        public GearDelegate gearDelegate;

        public delegate void BrakeDelegate();
        public BrakeDelegate brakeDelegate;

        public delegate void FlapsDelegate();
        public FlapsDelegate flapsDelegate;

        public delegate void RecoverDelegate();
        public RecoverDelegate recoverDelegate;


        public enum ControlType { Manual = 0, FlyByWire = 1, FlyByWireNavigation = 2, FlyByWireTargeting = 3 };
        public virtual ControlType getMode() { return 0; }
        

        public virtual int getCamIndex() { return 0; }
        public virtual bool getIsDamaged() { return false; }


        public virtual void updateInputs() { return; }
        public virtual void setInputTorque(Vector3 inputValue) { return; }
        public virtual void setInputForce(Vector3 inputValue) { return; }

        public virtual Vector3 getInputTorque() { return Vector3.zero; }
        public virtual Vector3 getInputForce() { return Vector3.zero; }

        public virtual float getThrottle() { return 0; }
        public virtual bool getEngineOn() { return true; }

        public virtual float getFlapsAngle() { return 0; }
        public virtual float getFlaps() { return 0; }
        public virtual float getForwardSpeed() { return 0; }
        public virtual Vector3 getSpeed() { return Vector3.zero; }
        public virtual Vector3 getAngularSpeed() { return Vector3.zero; }

        public virtual float getAltitude() { return 0; }

        public virtual float getFuel() { return 0; }
        public virtual float getFuelFlow() { return 0; }
        public virtual void setFuel(float value) { return; }
        public virtual void addFuel(float value) { return; }


        public virtual bool getIsGrounded() { return false; }
        public virtual bool getIsBraking() { return false; }

        public virtual bool getIsGearDown() { return true; }
        public virtual bool getIsBingoFuel() { return true; }


        public virtual void toogleArcade() { return; }
        public virtual void recoverAttitude() { return; }
        public virtual void changeCamera() { return; }
        public virtual void flapsDown() { return; }
        public virtual void flapsUp() { return; }
        public virtual void toogleGear() { return; }
        public virtual void toogleBrake() { return; }

    }
}