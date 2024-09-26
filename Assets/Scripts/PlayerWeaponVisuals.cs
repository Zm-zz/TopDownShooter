using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator _animator;
    [Tooltip("是否换枪中")]private bool _isGrabbingWeapon;

    #region Gun transform region

    [SerializeField] private Transform[] gunTransforms;
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;

    private Transform _currentGun;

    #endregion

    [Header("Rig")] 
    [SerializeField] private float rigWeightIncreaseRate;
    private bool _shouldIncrease_RigWeight;
    [Tooltip("掌管头、枪口朝向，左手位置...")] private Rig _rig;

    [Header("Left Hand IK")] 
    [SerializeField] private float leftHandIKWeightIncreaseRate;

    [SerializeField] [Tooltip("左手IK目标位置")] private Transform leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    private bool _shouldIncrease_LeftHandIKWeight;


    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rig = GetComponentInChildren<Rig>();

        SwitchOn(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && _isGrabbingWeapon == false)
        {
            _animator.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (_shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
            {
                _shouldIncrease_LeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (_shouldIncrease_RigWeight)
        {
            _rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (_rig.weight >= 1)
            {
                _shouldIncrease_RigWeight = false;
            }
        }
    }

    /// <summary>
    /// 降低Rig权重
    /// </summary>
    private void ReduceRigWeight()
    {
        _rig.weight = 0.15f;
    }

    /// <summary>
    /// 切换武器动画控制
    /// </summary>
    /// <param name="grabType"></param>
    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0; // 解除左手IK
        ReduceRigWeight();
        _animator.SetFloat("WeaponGrabType", (float)grabType);
        _animator.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeaponTo(true);
    }

    /// <summary>
    /// 限制换枪时能换弹，换枪时射击...
    /// </summary>
    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        _isGrabbingWeapon = busy;
        _animator.SetBool("BusyGrabbingWeapon", _isGrabbingWeapon);
    }

    /// <summary>
    /// 允许Rig权重恢复为1，目的为了换完子弹后右手回到枪上
    /// </summary>
    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true;

    /// <summary>
    /// 允许左手IK权重恢复为1（向枪靠近）
    /// </summary>
    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;

    /// <summary>
    /// 使用某武器
    /// </summary>
    /// <param name="gunTransform"></param>
    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        _currentGun = gunTransform;

        AttachLeftHand();
    }

    /// <summary>
    /// 关闭所有武器
    /// </summary>
    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 控制右手触摸武器
    /// </summary>
    private void AttachLeftHand()
    {
        Transform targetTransform = _currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    /// <summary>
    /// 根据不同武器切换不同动画Layer，不同的Idle、Fire、Reload、Grab动画
    /// </summary>
    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }

    /// <summary>
    /// 根据输入切换对应武器
    /// </summary>
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab); // 决定该武器由什么部位拿取出
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotgun);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(rifle);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

/// <summary>
/// 武器拿取类型（从什么部位拿取）
/// </summary>
public enum GrabType
{
    SideGrab,
    BackGrab,
}