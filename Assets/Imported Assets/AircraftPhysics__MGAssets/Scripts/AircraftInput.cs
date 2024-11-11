using UnityEngine;
using UnityEngine.UI;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class AircraftInput : MonoBehaviour
        {
            public static AircraftInput current;


            [Space]
            public bool isActive = true;
            [Tooltip("Aircraft that will receive Inputs from this Script")] public FlightScript flightScript;
            //public FlightScript flightScript; //AircraftScript
            public Vector2 throttleClamp = new Vector2(0, 1);

            [Space]
            public bool cursorStartLocked = false;
            bool clampInput = true;

            [Space]
            [Header("Input Settings")]
            [Space(15)]
            public bool useKeyboard = true;
            public KeyCode toogleCursorKey = KeyCode.Tab;

            [Space]
            public KeyCode recoverKey = KeyCode.Space;
            public KeyCode modeKey = KeyCode.Backspace, cameraKey = KeyCode.C;

            [Space]
            public KeyCode gearKey = KeyCode.G;
            public KeyCode brakeKey = KeyCode.B;
            public KeyCode flapsUpKey = KeyCode.PageUp;
            public KeyCode flapsDownKey = KeyCode.PageDown;


            [Space]
            public float throttleKeyFactor = 1f;
            public KeyCode throttleUp = KeyCode.R, throttleDown = KeyCode.F;
            public float throttleKeyStep = 0.25f;


            [Space]
            public float yawKeyFactor = 1f;
            public KeyCode yawLeft = KeyCode.Q, yawRight = KeyCode.E;

            [Space]
            public float pitchKeyFactor = 1f;
            public KeyCode pitchDown = KeyCode.W, pitchUp = KeyCode.S;

            [Space]
            public float rollKeyFactor = 1f;
            public KeyCode rollLeft = KeyCode.A, rollRight = KeyCode.D;






            [Space(30)]
            public bool useMouse = true;
            [Tooltip("0 = Left Button; 1 = Right Button; 2 = Middle Button")] public int brakeMouseButton = 2;

            [Space]
            public float pitchMouseFactor = 1f;
            public string pitchMouse = "Mouse Y";

            public float rollMouseFactor = 1f;
            public string rollMouse = "Mouse X";

            [Space]
            public float yawMouseFactor = 1f;
            public string yawMouse = "";    //"Mouse X"

            [Space]
            public float throttleMouseFactor = 1f;
            public string throttleMouse = ""; //"Mouse ScrollWheel"
            public float throttleMouseStep = 0.25f;





            [Space(30)]
            public bool useMobile = true;
            [Tooltip("Show Mobile controls even in other platforms")] public bool forceMobile = false;
            public float pitchMobileFactor = 1f, rollMobileFactor = 1f, yawMobileFactor = 1f;

            [Space]
            public float throttleMobileFactor = 1f;




            [Space(30)]
            public bool useJoystick = false;
            public KeyCode recoverJoystick = KeyCode.Joystick1Button1, modeJoystick = KeyCode.Joystick1Button2, cameraJoystick = KeyCode.Joystick1Button3;

            [Space]
            public KeyCode gearJoystick = KeyCode.Joystick1Button4;
            public KeyCode brakeJoystick = KeyCode.Joystick1Button5;
            public KeyCode flapsUpJoystick = KeyCode.Joystick1Button6;
            public KeyCode flapsDownJoystick = KeyCode.Joystick1Button7;


            [Space]
            public float pitchAxisFactor = 1f;
            public string pitchAxis = "Vertical";

            public float rollAxisFactor = 1f;
            public string rollAxis = "Horizontal";

            public float yawAxisFactor = 1f;
            public string yawAxis = ""; //"Yaw"

            [Space]
            public float throttleAxisFactor = 1f;
            public string throttleAxis = ""; //"Throttle"




            [Space(30)]
            [Header("GUI References")]
            [Space]
            public RectTransform mobileInputCanvas;
            public Image gearButton;
            public Image brakeButton;
            public FlashImg recoverFlashImgBut;
            [Tooltip("GUI Elements visible only by FP Camera that are disabled on External Cameras")] public RectTransform[] disableExtGuis;
            [HideInInspector] public InputMobileFlight[] inputsMobile;

            [SerializeField]
            RectTransform smallMinimap;
            [SerializeField]
            RectTransform bigMinimap;
            [SerializeField]
            int currentMinimapState;

            [Space(30)]
            [Header("Current Input - Read Only!")]
            public Vector3 inputForce;
            public Vector3 inputTorque;
            public float keyThrottle = 0;

            float lastInputZ;

            bool isPaused = false;


            //////////////////////////////////////// Initialization
            void Awake()
            {
                current = this;

                if (mobileInputCanvas != null && inputsMobile.Length == 0) inputsMobile = mobileInputCanvas.gameObject.GetComponentsInChildren<InputMobileFlight>(true);
                if (flightScript != null) UpdateMobile((int)flightScript.getMode());
            }
            void Start()
            {
                if (cursorStartLocked) Cursor.lockState = CursorLockMode.Locked; else Cursor.lockState = CursorLockMode.None;

                recoverAttitude();

                currentMinimapState = 0;

                smallMinimap.gameObject.SetActive(true);
                bigMinimap.gameObject.SetActive(false);
            }
            void OnEnable()
            {
                current = this;
                if (flightScript) flightScript.updateInputs();
                detectControls();

                flightScript.gearDelegate += gearUpdate;
                flightScript.brakeDelegate += brakeUpdate;
                flightScript.recoverDelegate += recoverUpdate;
                //flightScript.flapsDelegate += flapsUpdate;
            }
            void OnDisable()
            {
                if (current == this) current = null;

                flightScript.gearDelegate -= gearUpdate;
                flightScript.brakeDelegate -= brakeUpdate;
                flightScript.recoverDelegate -= recoverUpdate;
                //flightScript.flapsDelegate -= flapsUpdate;
            }
            //
            public void detectControls()
            {
                if (mobileInputCanvas != null)
                {
                    if (useMobile)
                    {
                        if (forceMobile || SystemInfo.deviceType == DeviceType.Handheld)
                        {
                            mobileInputCanvas.gameObject.SetActive(true);
                        }
                        else { useMobile = false; mobileInputCanvas.gameObject.SetActive(false); }
                    }
                    else mobileInputCanvas.gameObject.SetActive(false);
                }
                else useMobile = false;
            }
            //
            //////////////////////////////////////// Initialization



            ////////////////////////////////////// Aircraft Input Control
            void Update()
            {
                // Return if control is not activated
                if (!isActive || flightScript == null) return;

                if (isPaused == false)
                {

                    // Cursor lock-unlock with Tab key
                    if (Input.GetKeyDown(toogleCursorKey))
                    {
                        if (Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
                        else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

                        AircraftSnd.PlayClick();
                    }
                    //

                    // Recover Aircraft Attitude
                    if (useKeyboard && Input.GetKeyDown(recoverKey)) recoverAttitude();
                    if (useJoystick && Input.GetKeyDown(recoverJoystick)) recoverAttitude();
                    //

                    // Switch from Manual/FlyByWire Mode
                    if (useKeyboard && Input.GetKeyDown(modeKey)) toogleArcade();
                    if (useJoystick && Input.GetKeyDown(modeJoystick)) toogleArcade();
                    //

                    // Switch from FPV Camera and External
                    if (useKeyboard && Input.GetKeyDown(cameraKey)) changeCamera();
                    if (useJoystick && Input.GetKeyDown(cameraJoystick)) changeCamera();
                    //

                    // Flaps, Gear and Brake
                    if (useKeyboard && Input.GetKeyDown(gearKey)) toogleGear();
                    if (useKeyboard && Input.GetKeyDown(brakeKey)) toogleBrake();
                    if (useKeyboard && Input.GetKeyDown(flapsUpKey)) flapsUp();
                    if (useKeyboard && Input.GetKeyDown(flapsDownKey)) flapsDown();

                    if (useMouse && Input.GetMouseButtonDown(brakeMouseButton)) toogleBrake();

                    if (useJoystick && Input.GetKeyDown(gearJoystick)) toogleGear();
                    if (useJoystick && Input.GetKeyDown(brakeJoystick)) toogleBrake();
                    if (useJoystick && Input.GetKeyDown(flapsUpJoystick)) flapsUp();
                    if (useJoystick && Input.GetKeyDown(flapsDownKey)) flapsDown();
                    //


                    // GUI Recover Flashing Image
                    if (recoverFlashImgBut != null)
                    {
                        if (flightScript.getIsDamaged() != recoverFlashImgBut.isFlashing) recoverFlashImgBut.flash(flightScript.getIsDamaged());
                    }
                    //if (recoverFlashImgBut != null && flightScript.getIsDamaged() && !recoverFlashImgBut.isFlashing) recoverFlashImgBut.flash();
                    //


                    //////////////////////// Read all INPUTs
                    //
                    inputTorque = new Vector3
                    (
                        (useKeyboard ? pitchKeyFactor * ((Input.GetKey(pitchDown) ? 1 : 0) - (Input.GetKey(pitchUp) ? 1 : 0)) : 0) +
                        (useJoystick && pitchAxis != string.Empty ? pitchAxisFactor * Input.GetAxis(pitchAxis) : 0) +
                        ((useMouse && Cursor.lockState == CursorLockMode.Locked && pitchMouse != string.Empty) ? pitchMouseFactor * Input.GetAxis(pitchMouse) : 0) +
                        (useMobile ? pitchMobileFactor * InputMobileFlight.pitchInput : 0)
                        ,
                        (useKeyboard ? yawKeyFactor * ((Input.GetKey(yawRight) ? 1 : 0) - (Input.GetKey(yawLeft) ? 1 : 0)) : 0) +
                        (useJoystick && yawAxis != string.Empty ? yawAxisFactor * Input.GetAxis(yawAxis) : 0) +
                        ((useMouse && Cursor.lockState == CursorLockMode.Locked && yawMouse != string.Empty) ? yawMouseFactor * Input.GetAxis(yawMouse) : 0) +
                        (useMobile ? yawMobileFactor * InputMobileFlight.yawInput : 0)
                        ,
                        (useKeyboard ? rollKeyFactor * ((Input.GetKey(rollLeft) ? 1 : 0) - (Input.GetKey(rollRight) ? 1 : 0)) : 0) +
                        (useJoystick && rollAxis != string.Empty ? rollAxisFactor * -Input.GetAxis(rollAxis) : 0) +
                        ((useMouse && Cursor.lockState == CursorLockMode.Locked && rollMouse != string.Empty) ? rollMouseFactor * -Input.GetAxis(rollMouse) : 0) +
                        (useMobile ? rollMobileFactor * -InputMobileFlight.rollInput : 0)
                    );
                    //
                    inputForce = new Vector3
                        (
                        0
                        ,
                        0
                        ,
                        (useJoystick && throttleAxis != string.Empty ? throttleAxisFactor * Input.GetAxis(throttleAxis) : 0) +
                        (useMobile ? throttleMobileFactor * InputMobileFlight.throttleInput : 0)
                        );


                    // Throttle Step - Keyboard and Mouse
                    if (useKeyboard)
                    {
                        if (Input.GetKeyDown(throttleUp)) keyThrottle += throttleKeyStep;
                        else if (Input.GetKeyDown(throttleDown)) keyThrottle -= throttleKeyStep;
                    }

                    if (useMouse && throttleMouse != string.Empty)
                    {
                        if (Input.GetAxis(throttleMouse) > 0) keyThrottle += throttleMouseStep;
                        else if (Input.GetAxis(throttleMouse) < 0) keyThrottle -= throttleMouseStep;
                    }

                    if (lastInputZ != inputForce.z)
                    {
                        lastInputZ = inputForce.z;
                        keyThrottle = 0;
                    }
                    else keyThrottle = Mathf.Clamp(keyThrottle, throttleClamp.x - Mathf.Abs(inputForce.z), throttleClamp.y - Mathf.Abs(inputForce.z));
                    //else keyThrottle = Mathf.Clamp(keyThrottle , -1f - Mathf.Abs(inputForce.z), 1f + Mathf.Abs(inputForce.z) );

                    //////////////////////// Read all INPUTs


                    //////// Clamp Input to -1x1 at each direction and Throttle
                    if (clampInput)
                    {
                        flightScript.setInputTorque(new Vector3(Mathf.Clamp(inputTorque.x, -1f, 1f), Mathf.Clamp(inputTorque.y, -1f, 1f), Mathf.Clamp(inputTorque.z, -1f, 1f)));
                        flightScript.setInputForce(new Vector3(Mathf.Clamp(inputForce.x, -1f, 1f), Mathf.Clamp(inputForce.y, -1f, 1f), Mathf.Clamp(inputForce.z + keyThrottle, throttleClamp.x, throttleClamp.y)));
                        //flightScript.setInputForce(new Vector3(Mathf.Clamp(inputForce.x, -1f, 1f), Mathf.Clamp(inputForce.y, -1f, 1f), Mathf.Clamp(inputForce.z + keyThrottle, -1f, 1f)));
                    }
                    else
                    {
                        flightScript.setInputTorque(inputTorque);
                        flightScript.setInputForce(inputForce);
                    }
                    ////////
                    /// change minimap
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        ChangeMinimap();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGame();
                    }

                }
                else //ispaused = 1
                {
                    if(Input.GetKeyDown(KeyCode.Escape)) 
                    {
                        PauseEnd();
                    }
                    if(Input.GetKeyDown(KeyCode.S))
                    {
                        UpdatePauseMenuIndex(1);
                    }
                    if(Input.GetKeyDown(KeyCode.W)) 
                    {
                        UpdatePauseMenuIndex(-1);
                    }
                    if(Input.GetKeyDown(KeyCode.Return)) 
                    {
                        ExecutePauseMenuSelection();
                    }
                }
                
                


                //print("keyThrottle = " + keyThrottle + " // inputFinal = " + (Mathf.Clamp(inputForce.z + keyThrottle, throttleClamp.x, throttleClamp.y)));
            }
            //

            void ChangeMinimap()
            {
                if (currentMinimapState == 0)
                {
                    currentMinimapState = 1; //to big state.

                    smallMinimap.gameObject.SetActive(true);
                    bigMinimap.gameObject.SetActive(false);
                }
                else if (currentMinimapState == 1)
                {
                    currentMinimapState = 0;

                    smallMinimap.gameObject.SetActive(false);
                    bigMinimap.gameObject.SetActive(true);

                }
            }

            [SerializeField] RectTransform pauseMenu; //일시정지 ui세트.
            [SerializeField] Text pauseMenuPointer; // 일시정지 메뉴 포인터.
            [SerializeField] Text pauseMenuDescription; // 일시정지 메뉴 선택지 설명.
            [SerializeField] int pauseMenuCurrentIndex; //일시정지 현재 선택 인덱스.
            [SerializeField] int pauseMenuMaxIndex = 3;
            [SerializeField] float[] pauseMenuPointerYPos = {-140f, -180f, -220f, -260f }; //일시정지 메뉴 포인터 위치 배열.
            [SerializeField] string[] pauseMenuDescriptions;
            
            void PauseGame()
            {
                Time.timeScale = 0; //time stop.
                isPaused = true;

                UpdatePauseMenuIndex(0);
                AudioListener.pause = isPaused;
                pauseMenu.gameObject.SetActive(true);
            }

            void PauseEnd()
            {
                Time.timeScale = 1;
                isPaused = false;

               

                pauseMenuCurrentIndex = 0;
                AudioListener.pause = isPaused;
                pauseMenu.gameObject.SetActive(false);


            }

            void UpdatePauseMenuIndex(int change)
            {
                pauseMenuCurrentIndex += change;

                if(pauseMenuCurrentIndex < 0) 
                {
                    pauseMenuCurrentIndex = 3; //underflow.
                }
                else if(pauseMenuCurrentIndex > pauseMenuMaxIndex) 
                {
                    pauseMenuCurrentIndex = 0; //overflow.
                }

                pauseMenuPointer.rectTransform.anchoredPosition = new Vector3(120f, pauseMenuPointerYPos[pauseMenuCurrentIndex], 0f);

                pauseMenuDescription.text = pauseMenuDescriptions[pauseMenuCurrentIndex];
            }

            void ExecutePauseMenuSelection()
            {
                if (isPaused == false) return;
                
                if(pauseMenuCurrentIndex == 0)
                {
                    PauseEnd();
                }
                else if(pauseMenuCurrentIndex == 1) 
                {
                    //Checkpoint restart
                }
                else if(pauseMenuCurrentIndex == 2)
                {
                    //Mission restart.
                }
                else if(pauseMenuCurrentIndex == 3)
                {
                    //Quit.
                }
            }


            ////////////////////////////////////////////////// Update Mobile Inputs
            public static void UpdateMobile(int mode)
            {
                if (current == null || !current.useMobile || current.inputsMobile.Length == 0) return;
                for (int i = 0; i < current.inputsMobile.Length; i++) current.inputsMobile[i].checkMode(mode);
            }
            ////////////////////////////////////////////////// Update Mobile Inputs





            //////////////////////////////////////////////////////////// Functions
            public void toogleArcade()
            {
                if (flightScript == null || !isActive) return;

                flightScript.toogleArcade();

                AircraftSnd.PlayClick();
                HudMsg.Show((flightScript.getMode()).ToString() + " Mode", 5f);
            }
            public void recoverAttitude()
            {
                if (flightScript == null || !isActive) return;

                flightScript.recoverAttitude();
                if (recoverFlashImgBut != null) recoverFlashImgBut.stopFlash();
                AircraftSnd.PlayClick();
            }
            public void changeCamera()
            {
                if (flightScript == null || !isActive) return;

                flightScript.changeCamera();

                if (disableExtGuis.Length > 0)
                {
                    for (int i = 0; i < disableExtGuis.Length; i++) if (disableExtGuis[i] != null) disableExtGuis[i].gameObject.SetActive((flightScript.getCamIndex() == 0));
                }

                AircraftSnd.PlayClick();
            }
            public void flapsDown() { if (flightScript == null || !isActive) return; flightScript.flapsDown(); }
            public void flapsUp() { if (flightScript == null || !isActive) return; flightScript.flapsUp(); }
            public void toogleGear() { if (flightScript == null || !isActive) return; flightScript.toogleGear(); }
            public void toogleBrake() { if (flightScript == null || !isActive) return; flightScript.toogleBrake(); }


            // Delegates
            void gearUpdate()
            {
                if (gearButton) gearButton.transform.localScale = new Vector3(gearButton.transform.localScale.x, (flightScript.getIsGearDown() ? 1 : -1), gearButton.transform.localScale.z);
            }
            void brakeUpdate()
            {
                if (brakeButton) brakeButton.color = new Color(brakeButton.color.r, brakeButton.color.g, brakeButton.color.b, (flightScript.getIsBraking() ? 1 : 0.5f));
            }
            void recoverUpdate()
            {
                keyThrottle = flightScript.getThrottle() - inputForce.z;
            }
            //void flapsUpdate() { print("Delegate Input: flapsUpdate()"); }
            // Delegates


            //////////////////////////////////////////////////////////// Functions


        }
    }
}