using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] bool isMissileAlert;
    [SerializeField] public int currentEnemyBehind;
    [SerializeField] public int currentEnemyMissile;

    [Space]
    [Header("References")]
    Image[] images;
    RawImage[] rawImages;
    Image[] firstPersonUIImages;
    RawImage[] firstPersonUIRawImages;

    [SerializeField] Mask firstPersonMask;
    [SerializeField] GameObject firstPersonUIs;
    [SerializeField] Material textMaterial;
    [SerializeField] AudioSource warningAudioSource;
 
    // Start is called before the first frame update
    void Start()
    {
        currentEnemyBehind = 0;
        currentEnemyMissile = 0;
        isWarningState = false;
        isMissileAlert = false;

        rawImages = GetComponentsInChildren<RawImage>();
        images = GetComponentsInChildren<Image>();
        firstPersonUIImages = firstPersonUIs.GetComponentsInChildren<Image>();
        firstPersonUIRawImages = firstPersonUIs.GetComponentsInChildren<RawImage>();

        List<RawImage> tempList = new List<RawImage>(rawImages);

        // "colorfix" 태그가 있는 RawImage 제외하기
        tempList.RemoveAll(img => img.CompareTag("color fix"));

        // 최종 배열로 변환
        rawImages = tempList.ToArray();

        ChangeTextColor(textNormalColor);
    }

    // Update is called once per frame
    void Update()
    {

        SetSmallTag();

        isWarningState = currentEnemyBehind >= 1;
        bool currentMissileState = currentEnemyMissile >= 1; // 현재 missile 상태 계산

        // 상태가 변경된 경우에만 색상 변경
        if (currentMissileState != isMissileAlert)
        {
            isMissileAlert = currentMissileState; // 상태 업데이트
            ChangeUIColor(isMissileAlert ? imageWarningColor : imageNormalColor); // 색상 변경
            ChangeTextColor(isMissileAlert ? textWarningColor : textNormalColor);
        }

        if(isMissileAlert && !warningAudioSource.isPlaying)
        {
            warningAudioSource.Play();
        }
        if(!isMissileAlert && warningAudioSource.isPlaying)
        {
            warningAudioSource.Stop();
        }
        else
        {
            //
        }
    }

    public void ChangeUIColor(Color color)
    {

        foreach (Image img in images)
        {
            img.color = color;
        }
        foreach(RawImage img in rawImages)
        {
            img.color = color;
        }

        firstPersonMask.enabled = true;
        foreach (Image img in firstPersonUIImages)
        {
            img.color = color;
        }
        foreach (RawImage img in firstPersonUIRawImages)
        {
            img.color = color;
        }

    }

    public void ChangeTextColor(Color color)
    {
        firstPersonMask.enabled = false;
        textMaterial.SetColor("_GlowColor", color);
        textMaterial.SetFloat("_GlowPower", 1.5f);
        firstPersonMask.enabled = true;
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

    [Header("small tag references")]
    [SerializeField] GameObject warningTag;
    [SerializeField] GameObject missileAlertTag;

    public void SetSmallTag()
    {
        if (isWarningState && !isMissileAlert) //only warning
        {
            missileAlertTag.SetActive(false);
            warningTag.SetActive(true);
        }
        else if (isMissileAlert) //missile alert
        {
            warningTag.SetActive(false);
            missileAlertTag.SetActive(true);
            
        }
        else
        {
            missileAlertTag.SetActive(false);
            warningTag.SetActive(false);
        }
    }

    ////////////////////////////setting missile indicator parts
    //[SerializeField] EnemySTDM[] trackingMissiles; //missiles that tracking player aircraft.
    //[SerializeField] GameObject missileIndicatorPrefab; //missile indicator prefab.

    //public void AddTrackingMissile(EnemySTDM newMissile)
    //{
    //    trackingMissiles.Append(newMissile);
    //    GameObject newMissileIndicator = Instantiate(missileIndicatorPrefab);

    //    //newMissileIndicator.enemyMissile = newMissile
    //}

    //public void RemoveTrackingMissile(EnemySTDM missile)
    //{

    //}




















}
