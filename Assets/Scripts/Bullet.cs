using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody _rb;
    private TrailRenderer _trailRenderer;
    private MeshRenderer _meshRenderer;
    private SphereCollider _cd;

    private Vector3 _startPosition;
    private float _flyDistance;

    private bool _bulletDisabled;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _cd = GetComponent<SphereCollider>();
    }

    public void BulletSetup(float flyDistance)
    {
        _bulletDisabled = false;
        _cd.enabled = true;
        _meshRenderer.enabled = true;

        _trailRenderer.time = .25f;
        _startPosition = transform.position;
        this._flyDistance = flyDistance + 1;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (_trailRenderer.time <= 0)
        {
            ObjectPool.instance.ReturnBullet(gameObject);
        }
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance && !_bulletDisabled)
        {
            _cd.enabled = false;
            _meshRenderer.enabled = false;
            _bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
        {
            _trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

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
