using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player _player;

    /// <summary>
    /// 参考速度，子弹速度20 对应 子弹质量1（速度越快质量越小，才能达到不同速度一样的撞击效果）
    /// </summary>
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] private Weapon currentWeapon;

    private bool _weaponReady;

    private bool _isShooting;

    [Header("Bullet Details")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory(武器清单)")]

    [SerializeField][Tooltip("最大槽数")] private float maxSlots = 2;
    [SerializeField][Tooltip("武器槽")] private List<Weapon> weaponSlots;

    private void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();

        Invoke("EquipStartingWeapon", .1f);
    }

    private void Update()
    {
        if (_isShooting)
        {
            Shoot();
        }
    }

    private void EquipSlot1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    #region Slots management - Pickup/Equip/Drop/Ready Weapon

    /// <summary>
    /// 配备初始武器
    /// </summary>
    private void EquipStartingWeapon()
    {
        EquipWeapon(0);
    }

    /// <summary>
    /// 拾起武器
    /// </summary>
    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("槽位已满");
            return;
        }

        weaponSlots.Add(newWeapon);

        // 当做备用武器
        _player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    /// <summary>
    /// 配备武器
    /// </summary>
    private void EquipWeapon(int index)
    {
        SetWeaponReady(false);

        if (weaponSlots[index] != null)
        {
            currentWeapon = weaponSlots[index];

            // _player.weaponVisuals.SwitchOffWeaponModels();
            _player.weaponVisuals.PlayWeaponEquipAnimation();
        }
    }

    /// <summary>
    /// 掉落武器
    /// </summary>
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);

        // 当前武器 = 列表中剩余武器
        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => _weaponReady = ready;

    /// <summary>
    /// 武器是否准备好了（可以射击）
    /// </summary>
    public bool WeaponReady() => _weaponReady;

    #endregion



    private void Shoot()
    {
        if (!currentWeapon.CanShoot() || !WeaponReady()) return;

        if (currentWeapon.shootType == ShootType.Single)
        {
            _isShooting = false;
        }

        // 生成子弹
        GameObject newBullet = ObjectPool.instance.GetBullet();
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        // 子弹稍许偏移后的方向
        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        // 计算子弹质量公式：如果子弹速度增加，必须缩小子弹质量才可以产生一样的撞击效果（默认速度20 对应 质量1，若速度80，那么质量为1/4）
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;

        _player.weaponVisuals.PlayFireAnimation();
    }

    private void Reload()
    {
        SetWeaponReady(false);
        _player.weaponVisuals.PlayReloadAnimation();
    }

    /// <summary>
    /// 子弹发射方向
    /// </summary>
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();

        // 枪口指向瞄准目标
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        // 未开启精准瞄准 && 未检测到攻击目标
        if (!_player.aim.CanAimPrecisely() && _player.aim.Target() == null)
        {
            direction.y = 0;
        }

        return direction;
    }

    /// <summary>
    /// 是否武器槽中只有一个武器
    /// </summary>
    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    /// <summary>
    /// 获取当前武器
    /// </summary>
    public Weapon CurrentWeapon() => currentWeapon;

    /// <summary>
    /// 获取备用武器
    /// </summary>
    public Weapon BackupWeapon()
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
            {
                return weapon;
            }
        }

        return null;
    }

    /// <summary>
    /// 当前武器枪口位置
    /// </summary>
    public Transform GunPoint() => _player.weaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = _player.controls;

        controls.Character.Fire.performed += context => _isShooting = true;
        controls.Character.Fire.canceled += context => _isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

    }

    #endregion
}