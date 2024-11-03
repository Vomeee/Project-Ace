using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;
using UnityEngine.AddressableAssets;
using TMPro;

[Serializable]
public class ScriptData
{
    public List<ScriptInfo> subtitles;
}
public class ScriptManager : MonoBehaviour
{
    ScriptData scriptData;

    [Header("Script Data")]
    [SerializeField]
    TextAsset subtitleJsonFile;
    
    [SerializeField]
    AudioSource subtitleAudioSource;

    


    string subtitleFormat = "<size=40><mspace=20><color={0}><b><<</mspace=25></color=#ff4444></b><size=35> {1} <size=40><mspace=20><color={0}><b>>>";
    [Header("UI")]
    [SerializeField]
    GameObject scriptUI;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI subtitleText;
    [SerializeField]
    Color allyColor;
    [SerializeField]
    Color enemyColor;
    [SerializeField]
    TMP_FontAsset fontAsset;

    LinkedList<ScriptInfo> scriptQueue;

    bool isPrintingScript;
    [SerializeField]
    ScriptInfo currentScript;

    // Addressable
    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioClip> audioClipHandle;

    ScriptInfo SearchScriptInfoByKey(string scriptKey)
    {
        // scriptData 또는 scriptData.subtitles가 null인지 확인
        if (scriptData == null)
        {
            Debug.LogError("scriptData");
            
        }
        if (scriptData.subtitles == null)
        {
            Debug.LogError("subtitles");
            return null;
        }

        foreach (ScriptInfo script in scriptData.subtitles)
        {
            if (script.subtitleKey == scriptKey) return script;
        }
        return null;
    }

    public void AddScript(string scriptKey) //script 추가
    {
        scriptQueue.AddLast(SearchScriptInfoByKey(scriptKey));
    }

    public void AddScript(List<string> scriptKeyList)
    {
        foreach (string scriptKey in scriptKeyList)
        {
            scriptQueue.AddLast(SearchScriptInfoByKey(scriptKey));
        }
    }

    public void AddScriptAtFront(string scriptKey)
    {
        scriptQueue.AddFirst(SearchScriptInfoByKey(scriptKey));
    }

    public void ClearScriptQueue()
    {
        scriptQueue.Clear();
    }

    Color GetColorBySide(string sideString)
    {
        switch (sideString)
        {
            case "A": return allyColor;
            case "E": return enemyColor;
            default: return allyColor;
        }
    }

    void SetScript()
    {
        // Dequeue
        currentScript = scriptQueue.First.Value;
        scriptQueue.RemoveFirst();

        string subtitleKey = currentScript.subtitleKey;

        // Name
        Color textColor = GetColorBySide(currentScript.side);
        nameText.text = currentScript.name;
        nameText.color = textColor;

        // Subtitle
        string colorHexCode = "#" + ColorUtility.ToHtmlStringRGB(textColor);
        string subtitle = currentScript.subTitle; //현재 script subtitle.
        subtitleText.font = fontAsset;
        subtitleText.text = string.Format(subtitleFormat, colorHexCode, subtitle);
        

        //// Portrait //초상화.
        //string portraitKey = currentScript.name;
        //if (AddressableResourceExists(portraitKey) == true)
        //{
        //    portraitHandle = Addressables.LoadAssetAsync<Texture>(portraitKey);
        //    portraitHandle.Completed += (operationHandle) =>
        //    {
        //        portraitUI.SetActive(true);
        //        portraitImage.texture = operationHandle.Result;
        //    };
        //}
        //else
        //{
        //    portraitUI.SetActive(false);
        //}


        // Get AudioClip
        audioClipHandle = Addressables.LoadAssetAsync<AudioClip>(subtitleKey);
        audioClipHandle.Completed += (operationHandle) =>
        {
            AudioClip audioClip = operationHandle.Result;
            subtitleAudioSource.clip = audioClip;
        };

        Invoke("ShowScript", currentScript.preDelay);
    }

    void Awake()
    {
        scriptQueue = new LinkedList<ScriptInfo>();
        scriptUI.SetActive(false);
    }

    void Start()
    {
        string jsonString = subtitleJsonFile.text;
        // Load Script JSON
        scriptData = JsonUtility.FromJson<ScriptData>(jsonString);

        Debug.Log(scriptData.subtitles[0].name);
    }

    void ShowScript()
    {
        scriptUI.SetActive(true);
        subtitleAudioSource.Play();

        if (currentScript.invokeFunctionName != string.Empty)
        {
            //GameManager.MissionManager.InvokeMethod(currentScript.invokeMethodName, currentScript.invokeMethodDelay);
        }
        Invoke("HideScript", subtitleAudioSource.clip.length);
    }

    void HideScript()
    {
        scriptUI.SetActive(false);
        isPrintingScript = false;

        Addressables.Release(audioClipHandle);

        //if (portraitUI.activeSelf == true)
        //{
        //    Addressables.Release(portraitHandle);
        //    portraitUI.SetActive(false);
        //}

        currentScript = null;
    }

    void Update()
    {
        if (isPrintingScript == false && scriptQueue.Count > 0)
        {
            SetScript();
            isPrintingScript = true;
        }
    }
}
