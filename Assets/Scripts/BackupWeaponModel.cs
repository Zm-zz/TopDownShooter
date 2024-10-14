using UnityEngine;

public enum HangType { LowBackHang, BackHang, SideHang }

/// <summary>
/// ��������ģ��
/// </summary>
public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;

    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);

    /// <summary>
    /// ���������Ƿ�Ϊ�÷�ʽ
    /// </summary>
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}
