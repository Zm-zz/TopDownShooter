using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
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
        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

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