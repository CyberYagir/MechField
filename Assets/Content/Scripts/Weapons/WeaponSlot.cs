using Content.Scripts.Game.InputService;
using UnityEngine;

namespace Content.Scripts.Weapons
{
    public class WeaponSlot : Slot
    {
        [SerializeField] private new ShootAnimation animation;
        private IWeapon currentWeapon;

        public void SetWeapon(IWeapon weapon)
        {
            this.currentWeapon = weapon;

            if (weapon != null)
            {
                weapon.Transform.parent = transform;
                weapon.Transform.localPosition = Vector3.zero;

                if (animation != null)
                {
                    animation.Init(currentWeapon.Data.Type, currentWeapon);
                }
            }
        }
    }
}
