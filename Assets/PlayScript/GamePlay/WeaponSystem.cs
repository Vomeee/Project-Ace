using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponSystem : MonoBehaviour
{
    public AircrafSimpleHUD infoGetter; //�ӵ� ���� �������� instance
    
    #region weaponCounts variables
    [SerializeField]private int gunCount;
    [SerializeField] private int missileCount;
    //[SerializeField] private int specialWeaponCount;
    #endregion

    

    #region weapon prefabs
    public GameObject bulletPrefab; // �Ѿ� ������
    public GameObject missilePrefab; // �̻��� ������
    public GameObject specialWeaponPrefab; // Ư�� ���� ������

    #endregion

    #region gunVariables
    public float gunFireRate = 0.02f;
    public bool isGunFiring = false;
    private float fireCooldown;

    public Transform gunPoint; // �߻� ��ġ
    public AudioSource gunAudioSource; // AudioSource�� ����
    #endregion

    #region missileVariables
    public Transform leftMissileTransform;
    public Transform rightMissileTransform;
    #endregion

    public int weaponSelection = 0; // 0 : stdm, 1 : sp

    public Transform playerTransform;

    public Transform currentTargetTransform;

    public TargettingSystem targettingSystem;

    public float aircraftSpeed; //��ü ���� �ӵ�

    void Start()
    {
        gunCount = 1600;
        missileCount = 125;
        //specialWeaponCount = 16;

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

    #region weaponUI instances
    [SerializeField] RectTransform weaponPointer; // ���� ui ������
    [SerializeField] TextMeshProUGUI gunCountText; // ���� �ܷ�
    [SerializeField] TextMeshProUGUI missileCountText; // �⺻�̻��� �ܷ�
    [SerializeField] TextMeshProUGUI specialWeaponCountText; // Ư������ �ܷ�
    #endregion


    void Update()
    {
        #region Weapon Change and Fire

        if (Input.GetMouseButtonDown(0))
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

        // ���� ��ȯ (��Ŭ��)
        if (Input.GetMouseButtonDown(1))
        {
            
            
            if (weaponSelection == 0)
            {
                weaponSelection = 1;
            }
            else if(weaponSelection == 1)
            {
                weaponSelection = 0;
            }
            weaponPointerUpdate(); //���� ������ ������Ʈ
            //Beep(); //���� ��ȯ �Ҹ�
        }

        #endregion

        #region gunfire updates

        if (Input.GetKey(KeyCode.H)) // H Ű�� ������ �ִ� ����
        {
            isGunFiring = true; // �� �߻� ���¸� true�� ����
            if (!gunAudioSource.isPlaying) // �Ҹ��� ��� ������ �ʴٸ�
            {
                gunAudioSource.Play(); // �Ҹ� ���
            }
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            isGunFiring = false; // �� �߻� ���¸� false�� ����
            gunAudioSource.Stop(); // �Ҹ� ����
        }

        // �ѱ� ���� �߻� ó��
        if (isGunFiring && fireCooldown <= 0f)
        {
            FireGun();
            fireCooldown = gunFireRate;
        }

        // ��ٿ� Ÿ�̸�
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

        

        #endregion

        #region STDM updates

        STDMCoolDown(ref rightMissileCoolDown);
        STDMCoolDown(ref leftMissileCoolDown);

        aircraftSpeed = infoGetter.getSpeed();

        #endregion

        


    }

    void FireGun()
    {
        Debug.Log("gunfireTriggered");
        if (bulletPrefab != null && gunPoint != null && gunCount > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.bulletSpeed = 1200f; // ���ϴ� �ӵ��� ����
            }

            gunCount--;
            gunCountUIUpdate(); //��ź ������Ʈ
            Debug.Log("Gun fired");
        }
    }


    

    #region stdmCodes
    void FireMissile()
    {
        if (missileCount <= 0)
        {
            return; // ��ź 0.
        }
        if (leftMissileCoolDown > 0 && rightMissileCoolDown > 0)
        {
            return; // ������ �ȵ�.
        }

        Vector3 missilePosition;

        if(missileCount % 2 == 1) // ���� �̻��� ���� Ȧ��
        {
            missilePosition = rightMissileTransform.position;
            rightMissileCoolDown = missileCoolDownTime;
        }
        else // ¦��
        {
            missilePosition = leftMissileTransform.position;
            leftMissileCoolDown = missileCoolDownTime;
        }

        GameObject stdm = Instantiate(missilePrefab, missilePosition, playerTransform.rotation); //�̻��� ����
        STDM missileScript = stdm.GetComponent<STDM>();

       
        currentTargetTransform = targettingSystem.currentTarget;
        if(targettingSystem.IsInCone(currentTargetTransform))
        {
            missileScript.Launch(currentTargetTransform, infoGetter.getSpeed() / 10 ); ////////Ȯ��!!!!!
        }
        else
        {
            missileScript.Launch(null, infoGetter.getSpeed() / 10 + 20);
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
    }

    void stdmCountUIUpdate()
    {
        string mslText = missileCount.ToString();
        missileCountText.text = "<align=left>MSL<line-height=0>" + "\n" + "<align=right>" + mslText + "<line-height=1em>";
    }

    void specialWeaponCountUIUpdate()
    {

    }
    #endregion
}
