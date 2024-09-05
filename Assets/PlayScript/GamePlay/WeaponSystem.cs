using MGAssets.AircraftPhysics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public int missileCnt;



    public int weaponSelection = 0; // 0: Gun, 1: Missile, 2: Special Weapon
    public bool isGunFiring = false;
    public Transform playerTransform;
    public GameObject bulletPrefab; // 총알 프리팹
    public GameObject missilePrefab; // 미사일 프리팹
    public GameObject specialWeaponPrefab; // 특수 무기 프리팹
    public float gunFireRate = 0.1f;
    private float fireCooldown;

    public Transform currentTargetTransform;



    void Start()
    {

    }





    public float missileCoolDownTime;

    public float rightMissileCoolDown;
    public float leftMissileCoolDown;

    void Update()
    {
        #region Weapon Change and Fire

        if (Input.GetMouseButtonDown(0))
        {
            switch (weaponSelection)
            {
                case 0:
                    isGunFiring = true;
                    break;
                case 1:
                    FireMissile();
                    break;
                case 2:
                    FireSpecialWeapon();
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0) && weaponSelection == 0)
        {
            isGunFiring = false;
        }

        // 무기 전환 (우클릭)
        if (Input.GetMouseButtonDown(1))
        {
            weaponSelection++;
            if (weaponSelection > 2)
            {
                weaponSelection = 0;
            }
        }

        #endregion

        #region gunfire updates

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

        #region STDM updates

        STDMCoolDown(ref rightMissileCoolDown);
        STDMCoolDown(ref leftMissileCoolDown);

        speed = infoGetter.getSpeed(); 

        #endregion

    }

    public AircrafSimpleHUD infoGetter;

    public Transform gunPoint; // 발사 위치
    void FireGun()
    {
        if (bulletPrefab != null && gunPoint != null)
        {
            Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
            Debug.Log("Gun fired");
        }
    }

    public Transform leftMissileTransform;
    public Transform rightMissileTransform;

    public TargettingSystem targettingSystem;

    public float speed;
    

    void FireMissile()
    {
        if (missileCnt <= 0)
        {
            return; // 잔탄 0.
        }
        if (leftMissileCoolDown > 0 && rightMissileCoolDown > 0)
        {
            return; // 재장전 안됨.
        }

        Vector3 missilePosition;

        if(missileCnt % 2 == 1) // 남은 미사일 수가 홀수
        {
            missilePosition = rightMissileTransform.position;
            rightMissileCoolDown = missileCoolDownTime;
        }
        else // 짝수
        {
            missilePosition = leftMissileTransform.position;
            leftMissileCoolDown = missileCoolDownTime;
        }

        GameObject stdm = Instantiate(missilePrefab, missilePosition, playerTransform.rotation); //미사일 생성
        STDM missileScript = stdm.GetComponent<STDM>();

       
        currentTargetTransform = targettingSystem.currentTarget;
        if(targettingSystem.IsInCone(currentTargetTransform))
        {
            missileScript.Launch(currentTargetTransform, infoGetter.getSpeed() / 10 ); ////////확인!!!!!
        }
        else
        {
            missileScript.Launch(null, infoGetter.getSpeed() / 10 + 20);
        }
        
        missileCnt--;
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

    void FireSpecialWeapon()
    {
        
    }
}
