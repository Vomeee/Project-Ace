using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    GameObject UISet; //ui를 담고있는 gameobject.

    [Header("UI colors")]
    [SerializeField] Color imageWarningColor; //경고시 색깔 -> 빨간색.
    [SerializeField] Color imageNormalColor; //기본 색깔 -> 이미지 한정
    [SerializeField] Color textWarningColor;
    [SerializeField] Color textNormalColor;

    [Space]
    [Header("variables")]
    [SerializeField] bool isWarningState;
    [SerializeField] int currentEnemyMissile;

    [Space]
    [Header("References")]
    Image[] images;
    RawImage[] rawImages;
    [SerializeField] Material textMaterial;
    [SerializeField] AudioSource warningAudioSource;
 
    // Start is called before the first frame update
    void Start()
    {
        currentEnemyMissile = 0;
        isWarningState = false;

        rawImages = GetComponentsInChildren<RawImage>();
        images = GetComponentsInChildren<Image>();

        ChangeTextColor(textNormalColor);
    }

    // Update is called once per frame
    void Update()
    {
        bool currentWarningState = currentEnemyMissile >= 1; // 현재 경고 상태 계산

        // 상태가 변경된 경우에만 색상 변경
        if (currentWarningState != isWarningState)
        {
            isWarningState = currentWarningState; // 상태 업데이트
            ChangeImageUIColor(isWarningState ? imageWarningColor : imageNormalColor); // 색상 변경
            ChangeTextColor(isWarningState ? textWarningColor : textNormalColor);
        }

        if(isWarningState && !warningAudioSource.isPlaying)
        {
            warningAudioSource.Play();
        }
        if(!isWarningState && warningAudioSource.isPlaying)
        {
            warningAudioSource.Stop();
        }
        else
        {
            //
        }
    }

    public void ChangeImageUIColor(Color color)
    {
        // 현재 게임 오브젝트 아래에 있는 모든 Image 컴포넌트 찾기
        
        foreach (Image img in images)
        {
            img.color = color;
        }
        foreach(RawImage img in rawImages)
        {
            img.color = color;
        }
        
    }

    public void ChangeTextColor(Color color)
    {
        textMaterial.SetColor("_GlowColor", color);
        textMaterial.SetFloat("_GlowPower", 2.2f);
    }

    public void TrackingMissileCount(int mslState)
    {
        currentEnemyMissile += mslState;

        if(currentEnemyMissile < 0)
        {
            currentEnemyMissile = 0;
        }

        //if (currentEnemyMissile > 0) isWarningState = true;
        //else isWarningState = false;
    }
    
}
