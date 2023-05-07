using System;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using UnityEngine;

namespace Content.Scripts.Weapons
{
    public interface IWeapon
    {
        public Transform Transform { get; }
        public WeaponData Data { get; }
        public event Action<bool> OnChangeShootState;

        
        public void Init(Camera camera, IPoolService pool, MechData playerData, bool isPlayer, int moduleID);
        
        public void ShootStart();
        public void ShootEnd();
        public void Shoot();

        public void UpdateState();
    }
}
