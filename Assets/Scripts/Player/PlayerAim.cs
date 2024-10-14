using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private PlayerControls _controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser; // ��Weapon Holder ��Player�������壩

    [Header("Aim control")]
    [SerializeField] private Transform aim;
    [SerializeField][Tooltip("��ȷ��׼")] private bool isAimingPrecisely;
    [SerializeField][Tooltip("����")] private bool isLockingToTarget;


    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget;

    [SerializeField][Range(0.5f, 1f)] private float minCameraDistance = 1f;
    [SerializeField][Range(1f, 3f)] private float maxCameraDistance = 3f;
    [SerializeField][Range(3f, 5f)] private float cameraSensitivity = 4f;

    [Space]
    [SerializeField] private LayerMask aimLayerMask;

    [Tooltip("��������ϵͳ�¼�����ֵ")] private Vector2 _mouseInput;
    [Tooltip("�����յ������������Ϣ(����ɽ�����ʱʹ��)")] private RaycastHit _lastKnowMouseHit;

    void Start()
    {
        _player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        // ����/�رվ�ȷ��׼
        if (Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisely = !isAimingPrecisely;

            string warningContent = isAimingPrecisely ? "������ȷ��׼..." : "�رվ�ȷ��׼...";
            Debug.LogWarning(warningContent);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;

            string warningContent = isLockingToTarget ? "��������..." : "�ر�����...";
            Debug.LogWarning(warningContent);
        }


        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    /// <summary>
    /// ��׼���ӻ�����
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

        float laserTipLength = .5f; // �����յ�֮��������Ľ���͸���ļ������ĳ���
        float gunDistance = _player.weapon.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        // ����������������壬�����趨�����ص㣬ʹ���ⲻ�ܴ�͸����
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position); // �������
        aimLaser.SetPosition(1, endPoint); // �����յ�
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength); // �õ�Ϊ�����յ�������Ľ���͸���ļ����ĩ��
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
    /// ����Target����Ŀ�����Ŀ��
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
    /// �Ƿ�Ҫ��ȷ��׼
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
    /// �������������Ŀ��Ϊ����λ��
    /// </summary>
    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    /// <summary>
    /// ������׼Transform���ڵ�λ��
    /// </summary>
    private Vector3 DesiredCameraPosition()
    {
        // ʵ�����������(����ߵĻ���������׼Transform���Դﵽ�������룬��ֹ�����player�߳�����߽�)
        float actualMaxCameraDistance = _player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point; // ���λ�� 
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance); // ���ƾ���

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
