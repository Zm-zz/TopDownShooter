using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player _player;

    /// <summary>
    /// �ο��ٶȣ��ӵ��ٶ�20 ��Ӧ �ӵ�����1���ٶ�Խ������ԽС�����ܴﵽ��ͬ�ٶ�һ����ײ��Ч����
    /// </summary>
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] private Weapon currentWeapon;

    private bool _weaponReady;

    private bool _isShooting;

    [Header("Bullet Details")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory(�����嵥)")]

    [SerializeField][Tooltip("������")] private float maxSlots = 2;
    [SerializeField][Tooltip("������")] private List<Weapon> weaponSlots;

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
    /// �䱸��ʼ����
    /// </summary>
    private void EquipStartingWeapon()
    {
        EquipWeapon(0);
    }

    /// <summary>
    /// ʰ������
    /// </summary>
    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("��λ����");
            return;
        }

        weaponSlots.Add(newWeapon);

        // ������������
        _player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    /// <summary>
    /// �䱸����
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
    /// ��������
    /// </summary>
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);

        // ��ǰ���� = �б���ʣ������
        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => _weaponReady = ready;

    /// <summary>
    /// �����Ƿ�׼�����ˣ����������
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

        // �����ӵ�
        GameObject newBullet = ObjectPool.instance.GetBullet();
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        // �ӵ�����ƫ�ƺ�ķ���
        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        // �����ӵ�������ʽ������ӵ��ٶ����ӣ�������С�ӵ������ſ��Բ���һ����ײ��Ч����Ĭ���ٶ�20 ��Ӧ ����1�����ٶ�80����ô����Ϊ1/4��
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
    /// �ӵ����䷽��
    /// </summary>
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();

        // ǹ��ָ����׼Ŀ��
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        // δ������׼��׼ && δ��⵽����Ŀ��
        if (!_player.aim.CanAimPrecisely() && _player.aim.Target() == null)
        {
            direction.y = 0;
        }

        return direction;
    }

    /// <summary>
    /// �Ƿ���������ֻ��һ������
    /// </summary>
    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    /// <summary>
    /// ��ȡ��ǰ����
    /// </summary>
    public Weapon CurrentWeapon() => currentWeapon;

    /// <summary>
    /// ��ȡ��������
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
    /// ��ǰ����ǹ��λ��
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