using UnityEngine;

/// <summary>
/// ������ȡ���ͣ���ʲô��λ��ȡ��
/// </summary>
public enum EquipType { SideEquipAnimation, BackEquipAnimation }

/// <summary>
/// �����ճ����ͣ�������ӦAnimator�ж���Layer
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
