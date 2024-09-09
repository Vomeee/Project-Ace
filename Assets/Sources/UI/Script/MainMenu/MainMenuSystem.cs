using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuScreen; //초기 메뉴화면
    [SerializeField] GameObject missionSelectScreen; // 미션 선택화면

    public GameObject currentActiveScreen = null; //현재 활성화된 메뉴화면
    [SerializeField] MenuScreen currentMenuScreen;

    [SerializeField] private int currentIndex; //현재 선택 인덱스
    [SerializeField] private int maxIndex; //현재 화면 최대 인덱스
    [SerializeField] private float currentpointerTopPosY; //현재화면 포인터 최대 높이

    void SetCurrentActiveScreen(GameObject screenObject) //현재 메뉴를 전환.
    {
        currentActiveScreen?.SetActive(false);
        currentActiveScreen = screenObject;
        currentMenuScreen = currentActiveScreen.GetComponent<MenuScreen>();
        currentIndex = 0; //화면 전환시 새로운 인덱스는 0.
        maxIndex = currentMenuScreen.getMaxIndex(); //전환된 화면의 최대 인덱스 가져오기
        currentpointerTopPosY = currentMenuScreen.getPointerTopPosY(); //전환된 화면의 포인터 최대높이 가져오기.
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

    public void ToMissionSelectMenu()
    {

    }
    public void StartMission1() //추가 가능
    {
        SceneManager.LoadScene("MissionZero");
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    void Start() //오직 게임 실행시 시작
    {
        SetCurrentActiveScreen(mainMenuScreen); //메인메뉴 화면 활성화
    }

    
    private bool canInput = true;

    void Update()
    {
        if (currentActiveScreen != null)
        {

            if (!canInput) return; // 입력이 비활성화된 상태라면 Update를 무시

            if (Input.GetKeyDown(KeyCode.Space)) //선택
            {
                StartCoroutine(ConfirmCoroutine());
            }

            if (Input.GetKeyDown(KeyCode.W)) //위
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
            
            if (Input.GetKeyDown(KeyCode.S)) //아래
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
        audioSource = gameObject.AddComponent<AudioSource>(); // 오디오 소스 컴포넌트 추가
    }
    void playSelectionBeep() //선택지 전환시 효과음 출력함수.
    {
        audioSource.PlayOneShot(menuBrowseSound); //선택지 변경시의 효과음 재생
    }
    void playConfirmBeep()
    {
        audioSource.PlayOneShot(menuConfirmSound); //선택지 확정시의 효과음 재생
    }

    #endregion

    [SerializeField] Material pointerBarMaterial; // 포인터 머티리얼
    [SerializeField] Color colorOnConfirm;
    [SerializeField] Transform pointer;
    
    IEnumerator ConfirmCoroutine()
    {
        // 입력 비활성화
        canInput = false;

        // 선택시의 효과음 재생
        playConfirmBeep();
        Color originalColor = pointerBarMaterial.color;
        pointerBarMaterial.color = colorOnConfirm; // 원하는 색으로 변경

        // 0.2초 대기
        yield return new WaitForSeconds(0.1f);

        pointerBarMaterial.color = originalColor;

        yield return new WaitForSeconds(0.2f);

        // 선택된 UI 항목 실행
        currentMenuScreen.ExecuteCurrentSelection(currentIndex);

        // 입력 다시 활성화
        canInput = true;

        
    }

    void PointerUpdate()
    {
        pointer.position = new Vector3(-5.6f, currentpointerTopPosY - (currentIndex * 0.42f), -1.74f);
    }

}
