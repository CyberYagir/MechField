using System;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Weapons.Laser;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Weapons.Energy
{
    public class WeaponEnergy : WeaponBase, IWeapon
    {
        [SerializeField] private PoolService.BulletType bulletsType;
        [SerializeField] protected Transform cannonMesh;

        private float cannonMeshStartZ;
        private float time;
        private bool animationState = false;
        private IPoolService pool;
        private MechPart part;
        private bool isPlayer;

        public Transform Transform => transform;
        public event Action<bool> OnChangeShootState;

        public void Init(Camera camera, IPoolService pool, MechData playerData, bool isPlayer, int moduleID)
        {
            this.isPlayer = isPlayer;
            this.playerData = playerData;
            this.pool = pool;
            this.camera = camera;
            time = data.Cooldown;
            cannonMeshStartZ = cannonMesh.localPosition.z;
            data.SetPart(GetComponentInParent<MechPart>());
            data.SetModule(moduleID);
            
        }

        public void ShootStart()
        {
            if (IsCantShoot) return;
            
            if (time >= data.Cooldown && HaveBullets())
            {
                OnShoot();
                
                ChangeAnimationState(true);
                
                var bullet = pool.GetItemFromPool(bulletsType, transform.position, transform.rotation);
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, data.MaxDistance, data.Mask))
                {
                    bullet.transform.LookAt(hit.point);
                }
                else
                {
                    bullet.transform.LookAt(camera.transform.position + camera.transform.forward * data.MaxDistance);
                }
                bullet.Init(data, isPlayer, playerData.StateService);
                playerData.AddHeat(data.Heat);
                time = 0;
            }
        }


        public virtual bool HaveBullets() => true;
        public virtual void OnShoot()
        {
            cannonMesh.DOLocalMoveZ(0, 0.1f).onComplete += () =>
                cannonMesh.DOLocalMoveZ(cannonMeshStartZ, 0.2f);
        }

        public void ShootEnd()
        {
            ChangeAnimationState(false);
        }

        public void Shoot()
        {
            if (IsCantShoot)
            {
                ShootEnd();
                return;
            }
            ChangeAnimationState(false);
            ShootStart();
            UpdateVisuals();
        }

        public virtual void UpdateVisuals()
        {
            
        }


        public void ChangeAnimationState(bool state)
        {
            if (animationState != state)
            {
                animationState = state;
                OnChangeShootState?.Invoke(state);
            }
        }

        public void UpdateState()
        {
            time += Time.deltaTime;
            UpdateMesh();
        }
    }
}
