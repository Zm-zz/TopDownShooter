using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    /// <summary>
    /// �ο��ٶȣ��ӵ��ٶ�20 ��Ӧ �ӵ�����1���ٶ�Խ������ԽС�����ܴﵽ��ͬ�ٶ�һ����ײ��Ч����
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
        // �����ӵ�
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        // �����ӵ�������ʽ������ӵ��ٶ����ӣ�������С�ӵ������ſ��Բ���һ����ײ��Ч����Ĭ���ٶ�20 ��Ӧ ����1�����ٶ�80����ô����Ϊ1/4��
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    /// <summary>
    /// �ӵ����䷽��
    /// </summary>
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();

        // ǹ��ָ����׼Ŀ��
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        // δ������׼��׼ && δ��⵽����Ŀ��
        if (!_player.aim.CanAimPrecisely() && _player.aim.Target() == null)
        {
            direction.y = 0;
        }

        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim); // TODO: find a better place for it.

        return direction;
    }

    /// <summary>
    /// ǹ��λ��
    /// </summary>
    public Transform GunPoint() => gunPoint;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    //}
}