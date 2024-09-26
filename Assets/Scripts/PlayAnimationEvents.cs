using UnityEngine;

public class PlayAnimationEvents : MonoBehaviour
{
   private PlayerWeaponVisuals _visuals;

   private void Start()
   {
      _visuals = GetComponentInParent<PlayerWeaponVisuals>();
   }

   /// <summary>
   /// 换弹完毕
   /// </summary>
   public void ReloadIsOver()
   {
      _visuals.MaximizeRigWeight();
      
      // 补充子弹
   }

   /// <summary>
   /// 权重恢复
   /// </summary>
   public void ReturnRig()
   {
      _visuals.MaximizeRigWeight();
      _visuals.MaximizeLeftHandWeight();
   }

   /// <summary>
   /// 武器拿取动画播放完毕
   /// </summary>
   public void WeaponGrabIsOver()
   {
      _visuals.SetBusyGrabbingWeaponTo(false);
   }
}
