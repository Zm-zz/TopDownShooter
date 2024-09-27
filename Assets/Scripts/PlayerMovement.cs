using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player _player;

    private PlayerControls _controls;
    private CharacterController _characterController;
    private Animator _animator;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float _speed;
    [Tooltip("向下力，用于模拟重力")] private float _verticalVelocity;

    public Vector2 moveInput { get; private set; }
    private Vector3 _movementDirection;

    private bool _isRunning;


    private void Start()
    {
        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);

        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        // 按了快跑键 && 有移动输入信息
        bool playRunAnimation = _isRunning && _movementDirection.magnitude > 0;
        _animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        Vector3 _lookingDirection = _player.aim.GetMousePosition() - transform.position;
        _lookingDirection.y = 0f;
        _lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(_lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        _movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
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

    /// <summary>
    /// 输入事件分配
    /// </summary>
    private void AssignInputEvents()
    {
        _controls = _player.controls;

        _controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

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
}