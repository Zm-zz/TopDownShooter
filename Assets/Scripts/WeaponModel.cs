using UnityEngine;

/// <summary>
/// 武器拿取类型（从什么部位拿取）
/// </summary>
public enum EquipType { SideEquipAnimation, BackEquipAnimation }

/// <summary>
/// 武器握持类型，索引对应Animator中动画Layer
/// </summary>
public enum HoldType { CommonHold = 1, LowHold = 2, HighHold = 3 };


public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;
}
