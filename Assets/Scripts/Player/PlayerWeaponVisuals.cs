using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player _player;
    private Animator _animator;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool _shouldIncrease_RigWeight;
    [Tooltip("掌管头、枪口朝向，左手位置...")] private Rig _rig;

    [Header("Left Hand IK")]
    [SerializeField] private float leftHandIKWeightIncreaseRate;

    [SerializeField][Tooltip("左手IK目标位置")] private Transform leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    private bool _shouldIncrease_LeftHandIKWeight;

    private void Start()
    {
        _player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
        _rig = GetComponentInChildren<Rig>();

        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    /// <summary>
    /// 播放开火动画
    /// </summary>
    public void PlayFireAnimation() => _animator.SetTrigger("Fire");

    /// <summary>
    /// 获取当前的武器模型
    /// </summary>
    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = _player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    public void PlayReloadAnimation()
    {
        float reloadSpeed = _player.weapon.CurrentWeapon().reloadSpeed;

        _animator.SetFloat("ReloadSpeed", reloadSpeed);
        _animator.SetTrigger("Reload");
        ReduceRigWeight();
    }

    /// <summary>
    /// 播放配备武器动画
    /// </summary>
    public void PlayWeaponEquipAnimation()
    {
        EquipType equpiType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = _player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0; // 解除左手IK
        ReduceRigWeight();
        _animator.SetFloat("EquipType", (float)equpiType);
        _animator.SetTrigger("EquipWeapon");
        _animator.SetFloat("EquipSpeed", equipmentSpeed);
    }

    /// <summary>
    /// 打开当前武器模型
    /// </summary>
    public void SwitchOnCurrentWeaponModel()
    {
        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        // 武器槽中不只有一个武器时打开备用武器
        if (!_player.weapon.HasOnlyOneWeapon())
        {
            SwitchOnBackupWeaponModel();
        }

        SwitchAnimationLayer((int)CurrentWeaponModel().holdType);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    /// <summary>
    /// 关闭所有手持武器模型
    /// </summary>
    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 关闭所有备用武器模型
    /// </summary>
    private void SwitchOffBackupWeaponModels()
    {
        for (int i = 0; i < backupWeaponModels.Length; i++)
        {
            backupWeaponModels[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 打开备用武器模型
    /// </summary>
    public void SwitchOnBackupWeaponModel()
    {
        WeaponType weaponType = _player.weapon.BackupWeapon().weaponType;

        foreach (var backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
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

    #region Animation Rigging Methods

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
    /// 允许Rig权重恢复为1，目的为了换完子弹后右手回到枪上
    /// </summary>
    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true;

    /// <summary>
    /// 允许左手IK权重恢复为1（向枪靠近）
    /// </summary>
    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;

    /// <summary>
    /// 控制右手触摸武器
    /// </summary>
    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Target.localPosition = targetTransform.localPosition;
        leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    #endregion
}