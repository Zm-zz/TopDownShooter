using UnityEngine;

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
    [Header("武器类型")]
    public WeaponType weaponType;

    #region Shooting spesifics parameter

    [Space]
    [Header("Shooting spesifics")]
    [Header("射击类型")]
    public ShootType shootType;

    [Header("子弹发射量")]
    public int bulletsPerShot;

    [Header("默认子弹攻击速率/s")]
    public float defaultFireRate;

    [Header("攻击速率/s")]
    public float fireRate = 1;

    private float _lastShotTime;

    #endregion

    #region Burst Fire Parameter

    [Space]
    [Header("Burst fire")]
    [Header("连发是否可启用")]
    public bool burstAvailable;

    [Header("连发是否启用")]
    public bool burstActive;

    [Header("连发模式子弹发射量")]
    public int burstBulletsPerShot;

    [Header("连发模式子弹速率")]
    public float burstFireRate;

    [Header("每射击延迟")]
    public float burstFireDelay = 0.1f;

    #endregion

    #region Magazine Details Parameter

    [Space]
    [Header("Magazine details")]
    [Header("弹药")]
    public int bulletsInMagazine;

    [Header("弹夹容量")]
    public int magazineCapacity;

    [Header("总弹药")]
    public int totalReserveAmmo;

    [Header("换弹速度")]
    [Range(1, 3)]
    public float reloadSpeed = 1;

    [Header("装备武器速度")]
    [Range(1, 3)]
    public float equipmentSpeed = 1;

    [Header("射击距离")]
    [Range(2, 12)]
    public float gunDistance = 4;

    [Header("武器相机距离")]
    [Range(3, 8)]
    public float cameraDistance = 6;

    #endregion

    #region Spread
    [Space]
    [Header("Spread")]
    [Header("基础子弹分散")]
    public float baseSpread = 1;

    [Header("当前子弹分散")]
    private float _currentSpread;

    [Header("最大子弹分散")]
    public float maximumSpread = 3;

    [Header("子弹分散增长率")]
    public float spreadIncreaseRate = .15f;

    [Header("分散重置时间")]
    public float spreadCooldowm = 1;

    private float _lastSpreadUpdateTime;

    #endregion

    #region Burst Methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun) // 霰弹枪一直启用连发状态
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    /// <summary>
    /// 武器连发控制开关
    /// </summary>
    public void ToggleBurst()
    {
        if (!burstAvailable)
        {
            return;
        }

        burstActive = !burstActive;

        string debugContent = burstActive ? "连发启用" : "连发禁用";
        Debug.LogError(debugContent);

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    /// <summary>
    /// 是否满足射击频率
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

    #region Spread Methods

    /// <summary>
    /// 应用子弹分散
    /// </summary>
    /// <returns>分散后的方向</returns>
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-_currentSpread, _currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * originalDirection;
    }

    /// <summary>
    /// 刷新子弹分散
    /// </summary>
    private void UpdateSpread()
    {
        // 达到冷却时间，分散重置
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
    /// 子弹分散增长
    /// </summary>
    private void IncreaseSpread()
    {
        _currentSpread = Mathf.Clamp(_currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Reload Methods

    /// <summary>
    /// 是否有子弹可以射击
    /// </summary>
    /// <returns></returns>
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        // 子弹已满
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
        // 这会使弹夹中子弹回到总子弹数中
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
