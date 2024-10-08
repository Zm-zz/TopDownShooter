using UnityEngine;

public class PlayAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals _visuals;
    private PlayerWeaponController _controller;

    private void Start()
    {
        _visuals = GetComponentInParent<PlayerWeaponVisuals>();
        _controller = GetComponentInParent<PlayerWeaponController>();
    }

    /// <summary>
    /// 换弹完毕
    /// </summary>
    public void ReloadIsOver()
    {
        _visuals.MaximizeRigWeight();

        // 补充子弹
        _controller.CurrentWeapon().RefillBullets();
        _controller.SetWeaponReady(true);
    }

    /// <summary>
    /// 权重恢复
    /// </summary>
    public void ReturnRig()
    {
        _visuals.MaximizeRigWeight();
        _visuals.MaximizeLeftHandWeight();
    }

    /// <summary>
    /// 武器拿取动画播放完毕
    /// </summary>
    public void WeaponEquipingIsOver()
    {
        _controller.SetWeaponReady(true);
    }

    /// <summary>
    /// 切换到当前武器模型
    /// </summary>
    public void SwitchOnWeaponModel()
    {
        _visuals.SwitchOnCurrentWeaponModel();
    }
}
