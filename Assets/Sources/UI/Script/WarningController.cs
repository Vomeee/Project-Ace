using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    GameObject UISet; //ui�� ����ִ� gameobject.

    [Header("UI colors")]
    [SerializeField] Color imageWarningColor; //���� ���� -> ������.
    [SerializeField] Color imageNormalColor; //�⺻ ���� -> �̹��� ����
    [SerializeField] Color textWarningColor;
    [SerializeField] Color textNormalColor;
    [SerializeField] Color aircraftWireframeColor; //���� ���̾������� ���� ����.


    [Space]
    [Header("variables")]
    [SerializeField] bool isWarningState;
    [SerializeField] bool isMissileAlert;
    [SerializeField] public int currentEnemyBehind;
    [SerializeField] public int currentEnemyMissile;
    [SerializeField] float damagedTagShowingTime;
    [SerializeField] float damagedBlinkingInterval = 0.05f;

    [Space]
    [Header("References")]
    Image[] images;
    RawImage[] rawImages;
    Image[] firstPersonUIImages;
    RawImage[] firstPersonUIRawImages;
    [SerializeField] ScriptManager scriptManager;
    [SerializeField] RawImage[] aircraftWireframeImage;

    [SerializeField] Mask firstPersonMask;
    [SerializeField] GameObject firstPersonUIs;
    [SerializeField] Material textMaterial;
    [SerializeField] MissileIndicatorController missileIndicatorController;
    AudioSource[] missileAlertSounds;

    [Space]
    [Header("Warning Subtitles")]
    [SerializeField] List<string> warningSubtitles;
    [SerializeField] List<string> missileAlertSubtitles;
    [SerializeField] float playWarningSoundProbability;
    [SerializeField] float playMissileAlertSoundProbability;

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

        // "colorfix" �±װ� �ִ� RawImage �����ϱ�
        tempList.RemoveAll(img => img.CompareTag("color fix"));

        // ���� �迭�� ��ȯ
        rawImages = tempList.ToArray();

        ChangeTextColor(textNormalColor);

        missileAlertSounds = GetComponentsInChildren<AudioSource>(); //�̻��� ����� �ޱ�.
    }

    // Update is called once per frame
    void Update()
    {

        SetUpperSmallTag();

        bool currentWarningState = currentEnemyBehind >= 1;
        bool currentMissileState = currentEnemyMissile >= 1; // ���� missile ���� ���
        // ���� ��� ����Ǿ��� ���
        if (currentWarningState != isWarningState)
        {
            isWarningState = currentWarningState;
            if(isWarningState)// 0 -> 1
            {
                PlayWarningSound();
            }
        }

        // �̻��� ��� ����Ǿ��� ���
        if (currentMissileState != isMissileAlert)
        {
            isMissileAlert = currentMissileState; // ���� ������Ʈ
            ChangeUIColor(isMissileAlert ? imageWarningColor : imageNormalColor); // ���� ����
            ChangeTextColor(isMissileAlert ? textWarningColor : textNormalColor);

            if(isMissileAlert) //�̻��� ���� ���� 
            {
                PlayMissileWarningSound();
            }
            else //�ƴ�.
            {
                ChangeAircraftWireframeUI(aircraftWireframeColor);
            }
        }

        if(!isMissileAlert)
        {
            foreach(AudioSource alert in missileAlertSounds)
            {
                alert.Stop();
            }
        }
    }

    void PlayWarningSound()
    {
        if (Random.Range(0f, 1f) < playWarningSoundProbability)
        {
            string nowPlayingSound = warningSubtitles[Random.Range(0, warningSubtitles.Count)];
            Debug.Log("play warning");
            scriptManager.AddScript(nowPlayingSound);
            
        }
    }

    void PlayMissileWarningSound()
    {
        if (Random.Range(0f, 1f) < playMissileAlertSoundProbability)
        {
            string nowPlayingSound = missileAlertSubtitles[Random.Range(0, missileAlertSubtitles.Count)];
            Debug.Log("playing alert");
            scriptManager.AddScript(nowPlayingSound);
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
    [SerializeField] GameObject damagedTag;
    

    public void SetUpperSmallTag()
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

    public void DamageUIReact() //snall tag but ignore another tags.
    {
        //color update(blinking for damage state)
        StartCoroutine(UIReactForDamagedState());
        //tag update
        StartCoroutine(DamagedTagCoroutine());
    }

    private IEnumerator DamagedTagCoroutine()
    {
        damagedTag.SetActive(true);

        yield return new WaitForSeconds(damagedTagShowingTime);

        damagedTag.SetActive(false);
    }
    
    private IEnumerator UIReactForDamagedState()
    {
        ChangeUIColor(imageWarningColor); // ���� ����
        ChangeTextColor(textWarningColor);

        yield return new WaitForSeconds(damagedBlinkingInterval);

        ChangeUIColor(imageNormalColor); // ���� ����
        ChangeTextColor(textNormalColor);
        ChangeAircraftWireframeUI(aircraftWireframeColor);

        yield return new WaitForSeconds(damagedBlinkingInterval);

        ChangeUIColor(imageWarningColor); // ���� ����
        ChangeTextColor(textWarningColor);

        yield return new WaitForSeconds(damagedBlinkingInterval);

        if (isMissileAlert)
        {
            ChangeUIColor(imageWarningColor); // ���� ����
            ChangeTextColor(textWarningColor);
        }
        else
        {
            ChangeUIColor(imageNormalColor); // ���� ����
            ChangeTextColor(textNormalColor);
        }

        ChangeAircraftWireframeUI(aircraftWireframeColor);
    }

    public void ChangeAircraftWireframeUI(Color color)
    {
        aircraftWireframeColor = color;

    }


















}
