using UnityEngine;
using UnityEngine.SceneManagement;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class DemoSceneControl : MonoBehaviour
        {
            public static DemoSceneControl current;

            public bool isActive = true, singleton = false, useTabCursor = false, useF7Restart = true;
            public int awakeFrameRate = 60;
            public int screenShotSuperSize = 2;


            [Space]
            [Header("Demo Controls")]
            [Space]
            public GameObject instructionsGUI;
            public KeyCode closeGuiKey = KeyCode.Return;

            [Space]
            public GameObject aircraft1;
            public KeyCode key1 = KeyCode.Alpha1;

            [Space]
            public GameObject aircraft2;
            public KeyCode key2 = KeyCode.Alpha2;

            [Space]
            public GameObject aircraft3;
            public KeyCode key3 = KeyCode.Alpha3;

            [Space]
            public GameObject aircraft4;
            public KeyCode key4 = KeyCode.Alpha4;

            [Space]
            public GameObject aircraft5;
            public KeyCode key5 = KeyCode.Alpha5;
            //



            void Awake()
            {
                if (isActive)
                {
                    // Singleton - Single Instance
                    if (singleton)
                    {
                        if (current != null) { DestroyImmediate(gameObject); return; }
                        DontDestroyOnLoad(gameObject);
                        current = this;
                    }
                    //

                    Application.targetFrameRate = awakeFrameRate;
                }
            }
            void Update()
            {
                //Disable component if is not active
                if (!isActive) { this.enabled = false; return; }
                //

                //Quit Playmode by pressing F1
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit();
                    #endif
                }
                //

                //Restart Game
                if (useF7Restart && Input.GetKeyDown(KeyCode.F7)) SceneManager.LoadScene(0);
                //

                //Cursor lock-unlock with Tab key
                if (useTabCursor && Input.GetKeyDown(KeyCode.Tab))
                {
                    if (Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
                    else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

                    HudMsg.ShowQuick("Cursor = " + Cursor.lockState);
                    print("Cursor = " + Cursor.lockState);
                }
                //


                //Changes FrameRate
                if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.Delete))
                {
                    if (Application.targetFrameRate == 60) { Application.targetFrameRate = 120; }
                    else if (Application.targetFrameRate == 120) { Application.targetFrameRate = -1; }
                    else if (Application.targetFrameRate == -1) { Application.targetFrameRate = 15; }
                    else if (Application.targetFrameRate == 15) { Application.targetFrameRate = 30; }
                    else if (Application.targetFrameRate == 30) { Application.targetFrameRate = 60; }

                    HudMsg.Show("FPS = " + Application.targetFrameRate, 5);
                    print("FPS = " + Application.targetFrameRate);
                }
                //


                //Show current FrameRate on console
                if (Input.GetKeyDown(KeyCode.N) && Input.GetKey(KeyCode.Delete))
                {
                    HudMsg.Show("Current FPS = " + (1 / Time.deltaTime));
                    print("Current FPS = " + 1 / Time.deltaTime);
                }
                //


                /////////// Screenshots
                if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.Delete))
                {

                    //YYYYmmddHHMMSSfff -> Mode with Date+Hour+Minutes+Seconds+Miliseconds
                    ScreenCapture.CaptureScreenshot("Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd-hhmmss-fff") + ".png", screenShotSuperSize);

                    //HudMsg.ShowQuick("Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd-hhmmss-MS-fff") + ".png");
                    print("Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd-hhmmss-MS-fff") + ".png");
                }
                ///////////


                // Clear Console
                if (Input.GetKey(KeyCode.Delete) && Input.GetKeyDown(KeyCode.Insert)) ClearConsole();
                //



                /////////// Demo Functions
                if (Input.GetKeyDown(closeGuiKey))
                {
                    if (instructionsGUI) instructionsGUI.SetActive(!instructionsGUI.activeSelf);
                }

                if (Input.GetKeyDown(key1))
                {
                    if (aircraft1) aircraft1.SetActive(true);
                    if (aircraft2) aircraft2.SetActive(false);
                    if (aircraft3) aircraft3.SetActive(false);
                    if (aircraft4) aircraft4.SetActive(false);
                    if (aircraft5) aircraft5.SetActive(false);

                }

                if (Input.GetKeyDown(key2))
                {
                    if (aircraft1) aircraft1.SetActive(false);
                    if (aircraft2) aircraft2.SetActive(true);
                    if (aircraft3) aircraft3.SetActive(false);
                    if (aircraft4) aircraft4.SetActive(false);
                    if (aircraft5) aircraft5.SetActive(false);
                }

                if (Input.GetKeyDown(key3))
                {
                    if (aircraft1) aircraft1.SetActive(false);
                    if (aircraft2) aircraft2.SetActive(false);
                    if (aircraft3) aircraft3.SetActive(true);
                    if (aircraft4) aircraft4.SetActive(false);
                    if (aircraft5) aircraft5.SetActive(false);
                }

                if (Input.GetKeyDown(key4))
                {
                    if (aircraft1) aircraft1.SetActive(false);
                    if (aircraft2) aircraft2.SetActive(false);
                    if (aircraft3) aircraft3.SetActive(false);
                    if (aircraft4) aircraft4.SetActive(true);
                    if (aircraft5) aircraft5.SetActive(false);
                }

                if (Input.GetKeyDown(key5))
                {
                    if (aircraft1) aircraft1.SetActive(false);
                    if (aircraft2) aircraft2.SetActive(false);
                    if (aircraft3) aircraft3.SetActive(false);
                    if (aircraft4) aircraft4.SetActive(false);
                    if (aircraft5) aircraft5.SetActive(true);
                }
                /////////// Demo Functions





            }
            //



            //
            static void ClearConsole()
            {
                var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
                var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod.Invoke(null, null);
            }
            //

        }
    }
}