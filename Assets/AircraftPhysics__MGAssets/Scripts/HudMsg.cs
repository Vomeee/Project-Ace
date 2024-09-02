using UnityEngine;
using UnityEngine.UI;

namespace MGAssets
{
    public class HudMsg : MonoBehaviour
    {
        public static HudMsg current;
        public Text msgTxt;

        string lastTimedMsg;

        // Initialization
        void OnEnable() { current = this; showMsg(); }
        void OnDisable() { if (current == this) current = null;}
        //


        // Display Message
        void displayMsg(string msg = "", float timed = 0)
        {
            // Refresh
            if (msg == string.Empty) 
            { 
                if (lastTimedMsg != string.Empty) CancelInvoke("timedClearMsg"); 
                lastTimedMsg = string.Empty; 
                msgTxt.text = string.Empty; 
                return; 
            }

            // Sets a new Msg and Display
            msgTxt.text = msg;
            msgTxt.gameObject.SetActive(true);

            // Calls Timer
            if (timed > 0) 
            { 
                if (lastTimedMsg != string.Empty) CancelInvoke("timedClearMsg"); 
                lastTimedMsg = msg; 
                Invoke("timedClearMsg", timed); 
            }   
        }
        void timedClearMsg() 
        {
            //Clear msg after timer
            if (lastTimedMsg == msgTxt.text) 
            { 
                lastTimedMsg = string.Empty; 
                msgTxt.text = string.Empty; 
            } 

            return; 
        }                          
        //



        // Static and Public Calls
        public void showMsg(string msg = "") { displayMsg(msg, 0); }
        public void showQuick(string msg = "") { displayMsg(msg, 2); }
        public static void ShowQuick(string msg = "") { if (current != null) current.displayMsg(msg, 2); }
        public static void Show(string msg = "", float timed = 0) { if (current != null) current.displayMsg(msg, timed); }
        //


    }
}