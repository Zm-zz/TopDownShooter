using UnityEngine;

/// <summary>
/// ��������׼Ŀ��
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
