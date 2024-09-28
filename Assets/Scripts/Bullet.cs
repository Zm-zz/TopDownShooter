using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        // ����λ��
        _rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
