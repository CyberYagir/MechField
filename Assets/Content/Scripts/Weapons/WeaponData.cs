using System;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Mechs;
using UnityEngine;

namespace Content.Scripts.Weapons
{
    [System.Serializable]
    public class WeaponData
    {
        [SerializeField] private ShootType type;
        [SerializeField] private float damage;
        [SerializeField] private float maxDistance;
        [SerializeField] private float cooldown;
        [SerializeField] private float bulletsCount = float.PositiveInfinity;
        [SerializeField] private float heat;
        [SerializeField] private LayerMask layerMask;


        private int moduleID;
        private MechPart part;
        
        public float BulletsCount => bulletsCount;

        public float MaxDistance => maxDistance;

        public float Damage => damage;

        public ShootType Type => type;

        public float Cooldown => cooldown;
        public LayerMask Mask => layerMask;

        public MechPart Part => part;

        public float Heat => heat;

        public int ModuleID => moduleID;

        public event Action OnChangeData;

        public void SetPart(MechPart part)
        {
            this.part = part;
            part.OnTakeDamage += PartOnOnTakeDamage;
        }

        private void PartOnOnTakeDamage(float health)
        {
            OnChangeData?.Invoke();
        }

        public void SetLayerMask(LayerMask layerMask)
        {
            this.layerMask = layerMask;
        }

        public void MultiplyDamage(float attackModify)
        {
            damage *= attackModify;
        }

        public void RemoveBullet()
        {
            bulletsCount--;
            OnChangeData?.Invoke();
        }

        public void SetModule(int id) => moduleID = id;
    }
}