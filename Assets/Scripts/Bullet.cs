using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody _rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);

        ObjectPool.instance.ReturnBullet(gameObject);
    }

    /// <summary>
    /// 创建子弹碰撞特效
    /// </summary>
    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            // 第一个碰撞点
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFX, 0.5f);
        }
    }
}
