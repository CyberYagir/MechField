using Content.Scripts.Weapons;
using UnityEngine;

namespace Content.Scripts.Game.InputService
{
    public enum ShootType
    {
        Cannon = 0, Laser = 1
    }
    public class ShootAnimation : MonoBehaviour
    {
        private Animator animator;
        private ShootType type;
        private static readonly int WeaponType = Animator.StringToHash("WeaponType");
        private static readonly int Shoot = Animator.StringToHash("Shoot");
        private static readonly int Hold = Animator.StringToHash("IsHold");

        public void Init(ShootType type, IWeapon weapon)
        {
            this.type = type;

            animator = GetComponent<Animator>();
            animator.SetInteger(WeaponType,(int)type);
            weapon.OnChangeShootState += WeaponOnOnChangeShootState;
        }

        private void WeaponOnOnChangeShootState(bool state)
        {
            animator.SetBool(Hold, state);
            if (state)
            {
                animator.ResetTrigger(Shoot);
                animator.SetTrigger(Shoot);
            }
        }
    }
}
