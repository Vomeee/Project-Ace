using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponSystem : MonoBehaviour
{
    public AircrafSimpleHUD infoGetter; //속도 높이 가져오는 instance
    
    #region weaponCounts variables
    [SerializeField] private int gunCount;
    [SerializeField] private int missileCount;
    [SerializeField] private int specialWeaponCount;
    #endregion

    #region weapon prefabs
    public GameObject bulletPrefab; // 총알 프리팹
    public GameObject missilePrefab; // 미사일 프리팹
    public GameObject specialWeaponPrefab; // 특수 무기 프리팹

    #endregion

    #region gunVariables
    public float gunFireRate = 0.02f;
    public bool isGunFiring = false;
    private float fireCooldown;

    public Transform gunPointL; // 발사 위치 L
    public Transform gunPointR; // 발사 위치 R
    [SerializeField] Transform currentGunPoint; // 현재 발사 위치
    public AudioSource gunAudioSource; // AudioSource를 참조
    #endregion

    #region missileVariables
    public Transform leftMissileTransform;
    public Transform rightMissileTransform;
    #endregion

    #region references
    public int weaponSelection = 0; // 0 : stdm, 1 : sp
    [SerializeField] AudioSource weaponChangeToSpecialWeaponSound;
    [SerializeField] AudioSource weaponChangeToSTDMSound;
    
    
    public Transform playerTransform; //무기 생성시 활용
    public Transform currentTargetTransform; //firemissile 시 활용

    public TagController tagController;
    public TargettingSystem targettingSystem; //현재 타겟 받아오는데 필요함.
    public float aircraftSpeed; //기체 현재 속도
    #endregion

    #region UI references
    [SerializeField] Canvas STDMCanvas;
    [SerializeField] Canvas SPCanvas;
    [SerializeField] TextMeshProUGUI gunReady;
    [SerializeField] TextMeshProUGUI leftSTDMReady;
    [SerializeField] TextMeshProUGUI rightSTDMReady;
    [SerializeField] TextMeshProUGUI leftSPReady;
    [SerializeField] TextMeshProUGUI rightSPReady;
    #endregion

    void Start()
    {
        //gunCount = 1600;
        //missileCount = 125;
        //specialWeaponCount = 16;

        //specialWeaponCooldowns = new List<float>();
        //specialWeaponFirePoints = new List<Transform>();

        //for (int i = 0; i < specialWeaponCount; i++)
        //{
        //    specialWeaponCooldowns.Add(specialWeaponCooldown);
        //    if(i % 2 == 0)
        //    {
        //        specialWeaponFirePoints.Add(leftMissileTransform);
        //    }
        //    else
        //    {
        //        specialWeaponFirePoints.Add(rightMissileTransform);
        //    }
        //}

        gunCountUIUpdate();
        stdmCountUIUpdate();
        specialWeaponCountUIUpdate();
    }


    [Space]
    #region STDM instances

    public float missileCoolDownTime;

    public float rightMissileCoolDown;
    public float leftMissileCoolDown;

    #endregion

    [Space]
    #region Special weapon references.
    [SerializeField] int specialWeaponSize;
    [SerializeField]
    public float spCoolDownTime;

    public float spRightCoolDown;
    public float spLeftCoolDown;
    //List<Transform> specialWeaponFirePoints;
    //List<Transform> currentSPTargets;

    #endregion

    #region weaponUI instances
    [SerializeField] RectTransform weaponPointer; // 무기 ui 포인터
    [SerializeField] TextMeshProUGUI gunCountText; // 기총 잔량
    [SerializeField] TextMeshProUGUI missileCountText; // 기본미사일 잔량
    [SerializeField] TextMeshProUGUI specialWeaponCountText; // 특수무기 잔량
    #endregion


    void Update()
    {
        #region Weapon Change and Fire

        if (Input.GetMouseButtonDown(1))
        {
            switch (weaponSelection)
            {
                case 0:
                    FireMissile();                  
                    break;
                case 1:
                    FireSpecialWeapon();
                    break;
                  
            }
        }

        //무기 전환
        if (Input.GetMouseButtonDown(2))
        {
            if (weaponSelection == 0)
            {
                weaponSelection = 1;
                weaponChangeToSpecialWeaponSound.Play(); // 소리 재생
                
            }
            else if(weaponSelection == 1)
            {
                weaponSelection = 0;

                weaponChangeToSTDMSound.Play();
            }
            weaponPointerUpdate(); //무기 포인터 업데이트
            ShowCanvas(weaponSelection);
        }

        #endregion

        #region gunfire updates

        if (Input.GetMouseButton(0)) // H 키를 누르고 있는 동안
        {
            isGunFiring = true; // 총 발사 상태를 true로 설정
            if (!gunAudioSource.isPlaying) // 소리가 재생 중이지 않다면
            {
                gunAudioSource.Play(); // 소리 재생
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isGunFiring = false; // 총 발사 상태를 false로 설정
            gunAudioSource.Stop(); // 소리 정지
        }

        // 총기 연속 발사 처리
        if (isGunFiring && fireCooldown <= 0f)
        {
            FireGun();
            fireCooldown = gunFireRate;
        }

        // 쿨다운 타이머
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

        

        #endregion

        #region missile Cooldown updates

        STDMCoolDown(ref rightMissileCoolDown);
        STDMCoolDown(ref leftMissileCoolDown);
        STDMCoolDown(ref spRightCoolDown);
        STDMCoolDown(ref spLeftCoolDown);

        if(rightMissileCoolDown == 0 && missileCount > 0)
        {
            rightSTDMReady.text = "[RIGHT STDM READY]";
        }
        if (leftMissileCoolDown == 0 && missileCount > 1)
        {
            leftSTDMReady.text = "[LEFT STDM READY]";
        }
        if (spRightCoolDown == 0 && specialWeaponCount > 0)
        {
            rightSPReady.text = "[RIGHT QAAM READY]";
        }
        if (spLeftCoolDown == 0 && specialWeaponCount > 1)
        {
            leftSPReady.text = "[LEFT QAAM READY]";
        }

        aircraftSpeed = infoGetter.getSpeed();

        #endregion

        #region MXAA updates
        //if(weaponSelection == 1)
        //{
        //    int ind = 0;
        //    foreach(Transform tgt in targettingSystem.potentialTargetTransforms)
        //    {
        //        currentSPTargets.Clear();   
        //        if(targettingSystem.IsInCone(tgt))
        //        {
        //            currentSPTargets.Add(tgt);
        //            ind++;

        //            if (ind >= specialWeaponSize) break;
        //            UnityEngine.UI.Image img = tgt.gameObject.GetComponentInChildren<UnityEngine.UI.Image>();
        //            img.color = Color.red;
        //        }
        //    }
        //}
        #endregion
    }

    void ShowCanvas(int index)
    {
        if (index == 0)
        {
            STDMCanvas.gameObject.SetActive(true);
            SPCanvas.gameObject.SetActive(false);
        }
        else if (index == 1) 
        {
            STDMCanvas.gameObject.SetActive(false);
            SPCanvas.gameObject.SetActive(true);
        }
    }

    void FireGun()
    {
        //ebug.Log("gunfireTriggered");
        if (bulletPrefab != null && gunPointL != null && gunCount > 0)
        {
            if(gunCount % 2 == 0)
            {
                 currentGunPoint = gunPointL;
            }
            else
            {
                 currentGunPoint = gunPointR;
            }
            GameObject bullet = Instantiate(bulletPrefab, currentGunPoint.position, currentGunPoint.rotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.bulletSpeed = 1200f; // 원하는 속도로 설정
            }

            gunCount--;
            gunCountUIUpdate(); //잔탄 업데이트
            //Debug.Log("Gun fired");
        }
    }


    

    #region stdmCodes
    void FireMissile()
    {
        if (missileCount <= 0)
        {
            return; // 잔탄 0.
        }
        if (leftMissileCoolDown > 0 && rightMissileCoolDown > 0)
        {
            return; // 재장전 안됨.
        }

        Vector3 missilePosition;

        if(missileCount % 2 == 1) // 남은 미사일 수가 홀수
        {
            missilePosition = rightMissileTransform.position;
            rightMissileCoolDown = missileCoolDownTime;
            rightSTDMReady.text = missileCount == 1 ? "[RIGHT STDM N/A]" : "[RIGHT STDM RELOADING]";
        }
        else // 짝수
        {
            missilePosition = leftMissileTransform.position;
            leftMissileCoolDown = missileCoolDownTime;
            leftSTDMReady.text = missileCount <= 2 ? "[LEFT STDM N/A]" : "[LEFT STDM RELOADING]";
        }

        GameObject stdm = Instantiate(missilePrefab, missilePosition, playerTransform.rotation); //미사일 생성
        STDM missileScript = stdm.GetComponent<STDM>();

       
        currentTargetTransform = targettingSystem.currentTargetTransform;
        if(targettingSystem.IsInCone(currentTargetTransform))
        {
            missileScript.Launch(currentTargetTransform, infoGetter.getSpeed() / 10 + 20, tagController, targettingSystem.currentTargetLockingTime); ////////확인!!!!!
        }
        else
        {
            missileScript.Launch(null, infoGetter.getSpeed() / 5, tagController, 0);
        }
        
        missileCount--;
        stdmCountUIUpdate();
    }

    void STDMCoolDown(ref float cooldown)
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            if (cooldown < 0) cooldown = 0;
        }
        else return;
    }

    #endregion

    void FireSpecialWeapon()
    {
        if (specialWeaponCount <= 0)
        {
            return; // 잔탄 0.
        }
        if (spLeftCoolDown > 0 && spRightCoolDown > 0)
        {
            return; // 재장전 안됨.
        }

        Vector3 missilePosition;

        if (specialWeaponCount % 2 == 1) // 남은 미사일 수가 홀수
        {
            missilePosition = rightMissileTransform.position;
            rightSPReady.text = specialWeaponCount == 1 ? "[RIGHT QAAM N/A]" : "[RIGHT QAAM RELOADING]";
            spRightCoolDown = spCoolDownTime;
        }
        else // 짝수
        {
            missilePosition = leftMissileTransform.position;
            leftSPReady.text = specialWeaponCount <= 2 ? "[LEFT QAAM N/A]" : "[LEFT QAAM RELOADING]";
            spLeftCoolDown = spCoolDownTime;
        }

        GameObject stdm = Instantiate(specialWeaponPrefab, missilePosition, playerTransform.rotation); //미사일 생성
        STDM missileScript = stdm.GetComponent<STDM>();


        currentTargetTransform = targettingSystem.currentTargetTransform;
        if (targettingSystem.IsInCone(currentTargetTransform))
        {
            missileScript.Launch(currentTargetTransform, infoGetter.getSpeed() / 10 + 20, tagController, targettingSystem.currentTargetLockingTime); ////////확인!!!!!
        }
        else
        {
            missileScript.Launch(null, infoGetter.getSpeed() / 5, tagController, 0);
        }

        specialWeaponCount--;
        specialWeaponCountUIUpdate();
    }

    #region weaponUI update funcs
    void weaponPointerUpdate()
    {
        if (weaponPointer != null)
        {
            if(weaponSelection == 0)
            {
                weaponPointer.anchoredPosition = new Vector3(-308, 446, 0);
            }
            else if(weaponSelection == 1)
            {
                weaponPointer.anchoredPosition = new Vector3(-308, 386, 0);
            }
        }
    }

    void gunCountUIUpdate()
    {
        string gunText = gunCount.ToString();
        gunCountText.text = "<align=left>GUN<line-height=0>" + "\n" + "<align=right>" + gunText + "<line-height=1em>";
        gunReady.text = gunCount > 0 ? "[GUN READY]" : "[GUN N/A]";
    }

    void stdmCountUIUpdate()
    {
        string mslText = missileCount.ToString();
        missileCountText.text = "<align=left>MSL<line-height=0>" + "\n" + "<align=right>" + mslText + "<line-height=1em>";
        
    }

    void specialWeaponCountUIUpdate()
    {
        string mslText = specialWeaponCount.ToString();
        specialWeaponCountText.text = "<align=left>QAAM<line-height=0>" + "\n" + "<align=right>" + mslText + "<line-height=1em>";

    }

    void STDMFrameUpdate()
    {

    }
    #endregion
}
