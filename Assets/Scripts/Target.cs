using UnityEngine;

/// <summary>
/// 可锁定瞄准目标
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
