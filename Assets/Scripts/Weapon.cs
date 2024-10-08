using UnityEngine;
using UnityEngine.Rendering;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle,
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon
{
    [Header("��������")]
    public WeaponType weaponType;

    [Space]
    [Header("Shooting spesifics")]
    [Header("�������")]
    public ShootType shootType;

    [Header("��������/s")]
    public float fireRate = 1;

    private float _lastShotTime;


    [Space]
    [Header("Magazine details")]
    [Header("��ҩ")]
    public int bulletsInMagazine;

    [Header("��������")]
    public int magazineCapacity;

    [Header("�ܵ�ҩ")]
    public int totalReserveAmmo;

    [Header("�����ٶ�")]
    [Range(1, 3)]
    public float reloadSpeed = 1;

    [Header("װ�������ٶ�")]
    [Range(1, 3)]
    public float equipmentSpeed = 1;

    [Space]
    [Header("Spread")]
    [Header("�����ӵ���ɢ")]
    public float baseSpread = 1;

    [Header("��ǰ�ӵ���ɢ")]
    private float _currentSpread;

    [Header("����ӵ���ɢ")]
    public float maximumSpread = 3;

    [Header("�ӵ���ɢ������")]
    public float spreadIncreaseRate = .15f;

    [Header("��ɢ����ʱ��")]
    public float spreadCooldowm = 1;

    private float _lastSpreadUpdateTime;

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// �Ƿ��������Ƶ��
    /// </summary>
    /// <returns></returns>
    private bool ReadyToFire()
    {
        if (Time.time > _lastShotTime + 1 / fireRate)
        {
            _lastShotTime = Time.time;
            return true;
        }

        return false;
    }

    #region �ӵ���ɢ

    /// <summary>
    /// Ӧ���ӵ���ɢ
    /// </summary>
    /// <returns>��ɢ��ķ���</returns>
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-_currentSpread, _currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * originalDirection;
    }

    /// <summary>
    /// ˢ���ӵ���ɢ
    /// </summary>
    private void UpdateSpread()
    {
        // �ﵽ��ȴʱ�䣬��ɢ����
        if (Time.time > _lastSpreadUpdateTime + spreadCooldowm)
        {
            _currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        _lastSpreadUpdateTime = Time.time;
    }

    /// <summary>
    /// �ӵ���ɢ����
    /// </summary>
    private void IncreaseSpread()
    {
        _currentSpread = Mathf.Clamp(_currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Reload Methods

    /// <summary>
    /// �Ƿ����ӵ��������
    /// </summary>
    /// <returns></returns>
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        // �ӵ�����
        if (bulletsInMagazine == magazineCapacity)
        {
            return false;
        }

        if (totalReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void RefillBullets()
    {
        // ���ʹ�������ӵ��ص����ӵ�����
        // totalReserveAmmo += bulletsInMagazine;

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }

    #endregion
}
