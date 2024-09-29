using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    /// <summary>
    /// 参考速度，子弹速度20 对应 子弹质量1（速度越快质量越小，才能达到不同速度一样的撞击效果）
    /// </summary>
    private const float REFERENCE_BULLET_SPEED = 20;

    private Player _player;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        _player = GetComponent<Player>();
        _player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        // 生成子弹
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        // 计算子弹质量公式：如果子弹速度增加，必须缩小子弹质量才可以产生一样的撞击效果（默认速度20 对应 质量1，若速度80，那么质量为1/4）
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    /// <summary>
    /// 子弹发射方向
    /// </summary>
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();

        // 枪口指向瞄准目标
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        // 未开启精准瞄准 && 未检测到攻击目标
        if (!_player.aim.CanAimPrecisely() && _player.aim.Target() == null)
        {
            direction.y = 0;
        }

        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim); // TODO: find a better place for it.

        return direction;
    }

    /// <summary>
    /// 枪口位置
    /// </summary>
    public Transform GunPoint() => gunPoint;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    //}
}