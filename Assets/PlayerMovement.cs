using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls _controls;
    private CharacterController _characterController;
    private Animator _animator;

    [Header("Movement Info")] [SerializeField]
    private float walkSpeed;

    [SerializeField] private float runSpeed;
    private float _speed;
    private Vector3 _movementDirection;
    private float _verticalVelocity;
    private bool _isRunning;

    [Header("Aim info")] [SerializeField] private Transform aim;
    [SerializeField] private LayerMask _aimLayerMask;
    private Vector3 _lookingDirection;

    private Vector2 _moveInput;
    private Vector2 _aimInput;

    private void Awake()
    {
        AssignInputEvents();
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _speed = walkSpeed;
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
        AnimatorControllers();
    }

    private void Shoot()
    {
        Debug.Log("Fire!");
        _animator.SetTrigger("Fire");
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);

        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = _isRunning && _movementDirection.magnitude > 0;
        _animator.SetBool("isRunning", playRunAnimation);
    }

    private void AimTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(_aimInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            _lookingDirection = hitInfo.point - transform.position;
            _lookingDirection.y = 0f;
            _lookingDirection.Normalize();

            transform.forward = _lookingDirection;

            aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        _movementDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
        ApplyGravity();

        if (_movementDirection.magnitude > 0)
        {
            _characterController.Move(_movementDirection * Time.deltaTime * _speed);
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded == false)
        {
            _verticalVelocity -= 9.81f * Time.deltaTime;
            _movementDirection.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = -.5f;
        }
    }

    #region Input System

    private void AssignInputEvents()
    {
        _controls = new PlayerControls();
        
        _controls.Character.Fire.performed += context => Shoot();

        _controls.Character.Movement.performed += context => _moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => _moveInput = Vector2.zero;

        _controls.Character.Aim.performed += context => _aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => _aimInput = Vector2.zero;

        _controls.Character.Run.performed += context =>
        {
            _isRunning = true;
            _speed = runSpeed;
        };
        _controls.Character.Run.canceled += context =>
        {
            _isRunning = false;
            _speed = walkSpeed;
        };
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    #endregion
}