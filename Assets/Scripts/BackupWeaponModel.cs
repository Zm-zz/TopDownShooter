using UnityEngine;

public enum HangType { LowBackHang, BackHang, SideHang }

/// <summary>
/// 备用武器模型
/// </summary>
public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;

    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);

    /// <summary>
    /// 武器悬挂是否为该方式
    /// </summary>
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}
