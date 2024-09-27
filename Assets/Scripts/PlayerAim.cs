using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private PlayerControls _controls;

    [Header("Aim info")]
    [SerializeField][Range(0.5f, 1f)] private float minCameraDistance = 1f;
    [SerializeField][Range(1f, 3f)] private float maxCameraDistance = 3f;
    [SerializeField][Range(3f, 5f)] private float aimSensitivity = 4f;

    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;

    [Tooltip("根据输入系统事件分配值")] private Vector2 _aimInput;

    void Start()
    {
        _player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        // 控制瞄准目标Transform为期望瞄准目标
        aim.position = Vector3.Lerp(aim.position, DesiredAimPosition(), aimSensitivity * Time.deltaTime);
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(_aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    private void AssignInputEvents()
    {
        _controls = _player.controls;

        _controls.Character.Aim.performed += context => _aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => _aimInput = Vector2.zero;
    }

    /// <summary>
    /// 期望瞄准Transform所在的位置
    /// </summary>
    private Vector3 DesiredAimPosition()
    {
        // 实际相机最大距离(向后走的话，调整瞄准Transform可以达到的最大距离，防止向后走player走出相机边界)
        float actualMaxCameraDistance = _player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredAimPosition = GetMousePosition(); // 鼠标位置 
        Vector3 aimDirection = (desiredAimPosition - transform.position).normalized;
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredAimPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance); // 限制距离

        desiredAimPosition = transform.position + aimDirection * clampedDistance;
        desiredAimPosition.y = transform.position.y + 1;

        return desiredAimPosition;
    }
}
