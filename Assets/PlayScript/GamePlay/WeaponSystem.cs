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
    public GameObject bulletPrefab; // �Ѿ� ������
    public GameObject missilePrefab; // �̻��� ������
    public GameObject specialWeaponPrefab; // Ư�� ���� ������
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

        // ���� ��ȯ (��Ŭ��)
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

        speed = infoGetter.getSpeed(); 

        #endregion

    }

    public AircrafSimpleHUD infoGetter;

    public Transform gunPoint; // �߻� ��ġ
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
            return; // ��ź 0.
        }
        if (leftMissileCoolDown > 0 && rightMissileCoolDown > 0)
        {
            return; // ������ �ȵ�.
        }

        Vector3 missilePosition;

        if(missileCnt % 2 == 1) // ���� �̻��� ���� Ȧ��
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
