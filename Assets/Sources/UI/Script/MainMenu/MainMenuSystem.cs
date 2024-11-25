using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuScreen; //�ʱ� �޴�ȭ��
    [SerializeField] GameObject missionSelectScreen; // �̼�
    [SerializeField] GameObject aircraftSelectScreen;
    [SerializeField] GameObject controlScreen; //��Ʈ�ѹ�����˷��ִ� ȭ��.
    [SerializeField] GameObject MissionEndScreen; //�̼� ����� ���â.
    [SerializeField] GameObject TipScreen1;
    [SerializeField] GameObject TipScreen2;
    [SerializeField] GameObject TipScreen3;

    public GameObject currentActiveScreen = null; //���� Ȱ��ȭ�� �޴�ȭ��
    [SerializeField] GameManager gameManager;
    [SerializeField] MenuScreen currentMenuScreen;

    [SerializeField] private int currentIndex; //���� ���� �ε���
    [SerializeField] private int maxIndex; //���� ȭ�� �ִ� �ε���
    [SerializeField] private float currentpointerTopPosY; //����ȭ�� ������ �ִ� ����

    void SetCurrentActiveScreen(GameObject screenObject) //���� �޴��� ��ȯ.
    {
        currentActiveScreen?.SetActive(false);
        currentActiveScreen = screenObject;
        currentMenuScreen = currentActiveScreen.GetComponent<MenuScreen>();
        currentIndex = 0; //ȭ�� ��ȯ�� ���ο� �ε����� 0.
        maxIndex = currentMenuScreen.getMaxIndex(); //��ȯ�� ȭ���� �ִ� �ε��� ��������
        currentpointerTopPosY = currentMenuScreen.getPointerTopPosY(); //��ȯ�� ȭ���� ������ �ִ���� ��������.
        currentActiveScreen.SetActive(true);
        PointerUpdate();
    }

    #region executions
    public void ShowMainMenu()
    {
        SetCurrentActiveScreen(mainMenuScreen);
    }

    public void ShowMissionSelectMenu()
    {
        SetCurrentActiveScreen(missionSelectScreen);
    }
    public void ShowAircraftSelectMenu()
    {
        SetCurrentActiveScreen(aircraftSelectScreen);
    }
    public void ShowControlScreen()
    {
        SetCurrentActiveScreen(controlScreen);
    }
    public void ShowTipScreen1()
    {
        SetCurrentActiveScreen(TipScreen1);
    }
    public void ShowTipScreen2()
    {
        SetCurrentActiveScreen(TipScreen2);
    }
    public void ShowTipScreen3()
    {
        SetCurrentActiveScreen(TipScreen3);
    }

    public void StartMission1() //�߰� ����
    {
        SceneManager.LoadScene("MissionZero");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetPlayerAircraft(AircraftBase currentAircraftSelection)
    {
        gameManager.playerAircraftObject = currentAircraftSelection;
    }

    #endregion

    void Start() //���� ���� ����� ����
    {
        Debug.Log(PlayerPrefs.GetInt("isMissionEnd"));
        /////If Mission End -> Set Screen mission end screen.
        if (PlayerPrefs.GetInt("isMissionEnd") == 1)
        {
            SetCurrentActiveScreen(MissionEndScreen);
        }
        else
        {
            SetCurrentActiveScreen(mainMenuScreen); //���θ޴� ȭ�� Ȱ��ȭ
            
        }
        PlayerPrefs.SetInt("isMissionEnd", 0);
    }

    
    private bool canInput = true;

    void Update()
    {
        if (currentActiveScreen != null)
        {

            if (!canInput) return; // �Է��� ��Ȱ��ȭ�� ���¶�� Update�� ����

            if (Input.GetKeyDown(KeyCode.Return)) //����
            {
                StartCoroutine(ConfirmCoroutine());
            }

            if (Input.GetKeyDown(KeyCode.W)) //��
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                }
                else
                {
                    playSelectionBeep();
                }
                PointerUpdate();
            }
            
            if (Input.GetKeyDown(KeyCode.S)) //�Ʒ�
            {
                currentIndex++;
               
                if (currentIndex > maxIndex)
                {
                    currentIndex = maxIndex;
                }
                else
                {
                    playSelectionBeep();
                }
                PointerUpdate();
            }
        }
    }


    #region soundEffectsCodes

    [SerializeField] AudioClip menuBrowseSound;
    [SerializeField] AudioClip menuConfirmSound;
    [SerializeField] AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // ����� �ҽ� ������Ʈ �߰�
    }
    void playSelectionBeep() //������ ��ȯ�� ȿ���� ����Լ�.
    {
        audioSource.PlayOneShot(menuBrowseSound); //������ ������� ȿ���� ���
    }
    void playConfirmBeep()
    {
        audioSource.PlayOneShot(menuConfirmSound); //������ Ȯ������ ȿ���� ���
    }

    #endregion

    [SerializeField] Material pointerBarMaterial; // ������ ��Ƽ����
    [SerializeField] Color colorOnConfirm;
    [SerializeField] Transform pointer;
    
    IEnumerator ConfirmCoroutine()
    {
        // �Է� ��Ȱ��ȭ
        canInput = false;

        // ���ý��� ȿ���� ���
        playConfirmBeep();
        Color originalColor = pointerBarMaterial.color;
        pointerBarMaterial.color = colorOnConfirm; // ���ϴ� ������ ����

        // 0.2�� ���
        yield return new WaitForSeconds(0.1f);

        pointerBarMaterial.color = originalColor;

        yield return new WaitForSeconds(0.2f);

        // ���õ� UI �׸� ����
        currentMenuScreen.ExecuteCurrentSelection(currentIndex);

        // �Է� �ٽ� Ȱ��ȭ
        canInput = true;

        
    }

    void PointerUpdate()
    {
        pointer.position = new Vector3(-5.6f, currentpointerTopPosY - (currentIndex * 0.42f), -1.74f);
    }

}
