using System.Collections;
using System.Collections.Generic;
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
        bool currentWarningState = currentEnemyMissile >= 1; // ���� ��� ���� ���

        // ���°� ����� ��쿡�� ���� ����
        if (currentWarningState != isWarningState)
        {
            isWarningState = currentWarningState; // ���� ������Ʈ
            ChangeImageUIColor(isWarningState ? imageWarningColor : imageNormalColor); // ���� ����
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
        // ���� ���� ������Ʈ �Ʒ��� �ִ� ��� Image ������Ʈ ã��
        
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
