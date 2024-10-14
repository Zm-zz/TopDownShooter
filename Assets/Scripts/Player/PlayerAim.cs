using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private PlayerControls _controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser; // 在Weapon Holder （Player的子物体）

    [Header("Aim control")]
    [SerializeField] private Transform aim;
    [SerializeField][Tooltip("精确瞄准")] private bool isAimingPrecisely;
    [SerializeField][Tooltip("锁敌")] private bool isLockingToTarget;


    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget;

    [SerializeField][Range(0.5f, 1f)] private float minCameraDistance = 1f;
    [SerializeField][Range(1f, 3f)] private float maxCameraDistance = 3f;
    [SerializeField][Range(3f, 5f)] private float cameraSensitivity = 4f;

    [Space]
    [SerializeField] private LayerMask aimLayerMask;

    [Tooltip("根据输入系统事件分配值")] private Vector2 _mouseInput;
    [Tooltip("最后接收到的鼠标射线信息(脱离可交互层时使用)")] private RaycastHit _lastKnowMouseHit;

    void Start()
    {
        _player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        // 开启/关闭精确瞄准
        if (Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisely = !isAimingPrecisely;

            string warningContent = isAimingPrecisely ? "开启精确瞄准..." : "关闭精确瞄准...";
            Debug.LogWarning(warningContent);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;

            string warningContent = isLockingToTarget ? "开启锁敌..." : "关闭锁敌...";
            Debug.LogWarning(warningContent);
        }


        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    /// <summary>
    /// 瞄准可视化激光
    /// </summary>
    private void UpdateAimVisuals()
    {
        aimLaser.enabled = _player.weapon.WeaponReady();

        if (!aimLaser.enabled)
        {
            return;
        }

        WeaponModel weaponModel = _player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = _player.weapon.GunPoint();
        Vector3 laserDirection = _player.weapon.BulletDirection();

        float laserTipLength = .5f; // 激光终点之后延伸出的渐变透明的激光束的长度
        float gunDistance = _player.weapon.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        // 如果激光碰到了物体，重新设定激光重点，使激光不能穿透物体
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position); // 激光起点
        aimLaser.SetPosition(1, endPoint); // 激光终点
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength); // 该点为激光终点延伸出的渐变透明的激光的末端
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (isLockingToTarget && target)
        {
            if (target.GetComponent<Renderer>() != null)
            {
                aim.position = target.GetComponent<Renderer>().bounds.center;
            }
            else
            {
                aim.position = target.position;
            }
            return;
        }

        aim.position = GetMouseHitInfo().point;

        if (!isAimingPrecisely)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }

    /// <summary>
    /// 带有Target组件的可锁敌目标
    /// </summary>
    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aim;

    /// <summary>
    /// 是否要精确瞄准
    /// </summary>
    public bool CanAimPrecisely() => isAimingPrecisely;



    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            _lastKnowMouseHit = hitInfo;
        }

        return _lastKnowMouseHit;
    }

    #region Camera Region

    /// <summary>
    /// 设置摄像机跟随目标为期望位置
    /// </summary>
    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    /// <summary>
    /// 期望瞄准Transform所在的位置
    /// </summary>
    private Vector3 DesiredCameraPosition()
    {
        // 实际相机最大距离(向后走的话，调整瞄准Transform可以达到的最大距离，防止向后走player走出相机边界)
        float actualMaxCameraDistance = _player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point; // 鼠标位置 
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance); // 限制距离

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    #endregion

    private void AssignInputEvents()
    {
        _controls = _player.controls;

        _controls.Character.Aim.performed += context => _mouseInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => _mouseInput = Vector2.zero;
    }
}
