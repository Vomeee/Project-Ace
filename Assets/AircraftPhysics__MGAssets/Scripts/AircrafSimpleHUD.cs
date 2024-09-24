using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class AircrafSimpleHUD : MonoBehaviour
        {
            public static AircrafSimpleHUD current;


            [Header("Config References")]
            public bool isActive = true;
            public FlightScript flightScript;


            [Space(5)]
            [Header("Heading")]
            public bool useHeading = true;
            public float headingAmplitude = 1, headingOffSet = 0;
            [Range(0, 1)] public float headingFilterFactor = 0.1f;
            public Text headingTxt;


            [Space(5)]
            [Header("Altitude")]
            public bool useAltitude = true;
            public float altitudeAmplitude = 10, altitudeOffSet = 0;
            [Range(0, 1)] public float altitudeFilterFactor = 0.5f;
            public TextMeshProUGUI altitudeTxt;

            [Space(5)]
            [Header("AirSpeed")]
            public bool useSpeed = true;
            public float speedAmplitude = 10, speedOffSet = 0;
            [Range(0, 1)] public float speedFilterFactor = 0.25f;
            public TextMeshProUGUI speedTxt;



            [Space(5)]
            [Header("GlidePath")]
            [Space]
            public bool useGlidePath = true;
            [Range(0, 1)] public float glidePathFilterFactor = 0.1f;
            public float glideXDeltaClamp = 600f, glideYDeltaClamp = 700f;
            public RectTransform glidePath;

            [Space]
            public float alphaAmplitude = 1, alphaOffSet = 0;
            [Range(0, 1)] public float alphaFilterFactor = 0.25f;

            [Space]
            public float betaAmplitude = 1;
            public float betaOffSet = 0;
            [Range(0, 1)] public float betaFilterFactor = 0.25f;



            [Space(5)]
            [Header("Engine and Fuel")]
            public bool useEngine = true;
            public float engineAmplitude = 100;
            public float engineOffSet = 0;
            [Range(0, 1)] public float engineFilterFactor = 0.05f;
            public Slider engineSliderUI;
            public Image engineFillUI;
            public Text engineTxt;

            [Space]
            public bool useFuel = true;
            public float fuelAmplitude = 100;
            [Range(0, 1)] public float fuelFilterFactor = 0.0125f;
            public Slider fuelSliderUI;
            public Image fuelFillUI;
            public Text fuelTxt;

            [Space]
            public float fuelFlowAmplitude = 1;
            public Image fuelFlowFillUI;
            public Text fuelFlowTxt;



            [Space(5)]
            [Header("Flaps & Gear")]
            public bool useFlaps = true;
            [Range(0, 1)] public float flapsFilterFactor = 0.05f;
            public Slider flapsSliderUI;
            public Image flapsFillUI;
            public Text flapsTxt;

            [Space]
            public bool useGear = true;
            public bool gearDown = true;
            public string gearDownTxt = "DOWN";
            public string gearUpTxt = "UP";
            float gear;

            [Space]
            public Text gearTxt;
            public GameObject[] gearLights;
            public FlashImg[] gearFlashLights;

            [Space]
            public bool useBrake = true;
            public Text brakeTxt;

            [Space]
            public TextMeshProUGUI systemTimeText; //시스템 시간 컴포넌트.
            public TextMeshProUGUI systemScoreText; //현재 점수 컴포넌트.

            [Space]
            public UVController speedUV;
            public UVController altitudeUV;







            //All Flight Variables
            [Space(10)]
            [Header("Flight Variables - ReadOnly!")]
            [Space]
            public float flaps;
            public float speed;
            public float altitude, heading, alpha, beta, engine, fuel;
            public bool brake;
            //
            
            
            [SerializeField] float remainTime;
            [SerializeField] int score;

            void setSystemTime()
            {
                if (remainTime <= 0)
                {
                    remainTime = 0;
                    return;
                }

                remainTime -= Time.deltaTime;
                int seconds = (int)remainTime;

                int min = seconds / 60;
                int sec = seconds % 60;
                int millisec = (int)((remainTime - seconds) * 100);
                string text = string.Format("TIME <mspace=30>{0:00}</mspace>:<mspace=30>{1:00}</mspace>:<mspace=30>{2:00}</mspace>", min, sec, millisec);
                systemTimeText.text = text;
            }

            void Update()
            {
                setSystemTime();
            }

            void UpdateScore(int aircraftScore)
            {
                int newScore = score + aircraftScore;
                string text = string.Format("{0:D6}", newScore);
                systemScoreText.text = text;
            }


            #region aircraft codes


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////// Inicialization
            void Awake()
            {
                if (current == null) current = this;
            }
            //
            void OnEnable()
            {
                if (current != this) current = this;

                if (useGear) gear = 0.5f;
            }
            //
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////// Inicialization








            /////////////////////////////////////////////////////// Update All Instruments
            void FixedUpdate() //Update()
            {
                if (useAltitude) updateAltitude();
                if (useSpeed) updateSpeed();

                // Return if not active
                //if (!isActive) return;

                //// Call each Instrument
                //if (useHeading) updateHeading();
                //if (useGlidePath) updateGlide();

                //if (useAltitude) updateAltitude();
                //if (useSpeed) updateSpeed();

                //if (useEngine) updateEngine();
                //if (useFuel) updateFuel();

                //if (useFlaps) updateFlaps();
                //if (useGear) updateGear();
                //if (useBrake) updateBrake();
                //

            }
            /////////////////////////////////////////////////////// Update All Instruments











            /////////////////////////////////////////////////////// Calculations
            void updateHeading()
            {
                heading = Mathf.LerpAngle(heading, headingAmplitude * flightScript.transform.rotation.eulerAngles.y + headingOffSet, headingFilterFactor) % 360f;

                //Send values to Gui and Instruments
                if (headingTxt != null) { if (heading < 0) headingTxt.text = (heading + 360f).ToString("000"); else headingTxt.text = heading.ToString("000"); }
            }
            //
            void updateGlide()
            {
                //Calculate both Angles
                alpha = Mathf.Lerp(alpha, alphaOffSet + alphaAmplitude * Vector2.SignedAngle(new Vector2(flightScript.getSpeed().z, flightScript.getSpeed().y), Vector2.right), alphaFilterFactor);
                beta = Mathf.Lerp(beta, betaOffSet + betaAmplitude * Vector2.SignedAngle(new Vector2(flightScript.getSpeed().x, flightScript.getSpeed().z), Vector2.up), betaFilterFactor);

                //Apply angle values to the glidePath UI element
                if (useGlidePath && glidePath != null) glidePath.localPosition = Vector3.Lerp(glidePath.localPosition, new Vector3(Mathf.Clamp(-beta * -8.4f, -glideXDeltaClamp, glideXDeltaClamp), Mathf.Clamp(alpha * -8.4f, -glideYDeltaClamp, glideYDeltaClamp), 0), glidePathFilterFactor);
            }
            //
            void updateAltitude()
            {
                altitude = Mathf.Lerp(altitude, altitudeOffSet + altitudeAmplitude * flightScript.getAltitude(), speedFilterFactor);

                //Send values to Gui and Instruments
                if (altitudeTxt != null) altitudeTxt.text = altitude.ToString("0").PadLeft(5);

                altitudeUV.SetUV(altitude);
            }
            //
            void updateSpeed()
            {
                speed = Mathf.Lerp(speed, speedOffSet + speedAmplitude * flightScript.getForwardSpeed(), speedFilterFactor);

                //Send values to Gui and Instruments
                if (speedTxt != null) speedTxt.text = speed.ToString("0").PadLeft(5);//.ToString("##0");

                speedUV.SetUV(speed);
            }
            public float getSpeed()
            {
                return speed;
            }
            //
            void updateEngine()
            {
                engine = Mathf.Lerp(engine, flightScript.getThrottle() * engineAmplitude + (flightScript.getEngineOn() ? engineOffSet * Mathf.Sign(flightScript.getThrottle()) : 0), engineFilterFactor);

                //Send values to Gui
                if (engineSliderUI != null) engineSliderUI.value = (engine / engineAmplitude);
                if (engineFillUI != null) engineFillUI.fillAmount = Mathf.Abs(engine / (engineAmplitude + engineOffSet * Mathf.Sign(flightScript.getThrottle())));
                if (engineTxt != null) engineTxt.text = engine.ToString("##0");
            }
            //
            void updateFuel()
            {
                fuel = Mathf.Lerp(fuel, fuelAmplitude * flightScript.getFuel(), fuelFilterFactor);

                //Send values to Gui
                if (fuelSliderUI != null) fuelSliderUI.value = (fuel / fuelAmplitude);
                if (fuelFillUI != null) fuelFillUI.fillAmount = (fuel / fuelAmplitude);
                if (fuelTxt != null) fuelTxt.text = fuel.ToString("##0");
            }
            //
            void updateGear()
            {
                gearDown = flightScript.getIsGearDown();

                // Verify Gear Changes
                if (gearDown && gear < 1)
                {
                    if (gear != 0.5f) foreach (FlashImg light in gearFlashLights) light.flash();
                    foreach (GameObject light in gearLights) light.gameObject.SetActive(true);

                    //if (gear != 0.5f) AircraftSnd.Play(3);
                    gear = 1;


                    if (gearTxt != null) gearTxt.text = gearDownTxt;
                }
                else if (!gearDown && gear > 0)
                {

                    if (gear != 0.5f) foreach (FlashImg light in gearFlashLights) light.flash();
                    foreach (GameObject light in gearLights) light.gameObject.SetActive(false);

                    //if (gear != 0.5f) AircraftSnd.Play(3);
                    gear = 0;

                    if (gearTxt != null) gearTxt.text = gearUpTxt;
                }
            }
            //
            void updateFlaps()
            {
                //Updates current Flap value
                flaps = Mathf.Lerp(flaps, flightScript.getFlaps(), flapsFilterFactor);


                //Send values to Gui and Instruments
                if (flapsSliderUI != null) flapsSliderUI.value = flaps;
                if (flapsFillUI != null) flapsFillUI.fillAmount = flaps;
                if (flapsTxt != null) flapsTxt.text = flightScript.getFlapsAngle().ToString();
            }
            //
            void updateBrake()
            {
                brake = flightScript.getIsBraking();
                if (brakeTxt != null) brakeTxt.gameObject.SetActive(brake);
            }
            //
            /////////////////////////////////////////////////////// Calculations

#endregion
        }
    }
}
