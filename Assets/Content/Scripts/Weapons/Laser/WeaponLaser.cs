using System;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using UnityEngine;

namespace Content.Scripts.Weapons.Laser
{
    public class RaysData
    {
        private RaycastHit availableHit;
        private Vector3 nextPoint;

        public Vector3 NextPoint { get; set; }

        public RaycastHit AvailableHit { get; set; }
    }
    public class WeaponLaser : WeaponBase, IWeapon
    {
        
        [SerializeField] private LineRenderer line;
        [SerializeField] private ParticleSystem particles;
        
        private RaysData raysData;
        private bool isShooted;
        private bool isPlayer;

        public Transform Transform => transform;
        public event Action<bool> OnChangeShootState;

        public void Init(Camera camera, IPoolService pool, MechData playerData, bool isPlayer, int moduleID)
        {
            this.isPlayer = isPlayer;
            this.playerData = playerData;
            this.camera = camera;
            data.MultiplyDamage(playerData.AttackModify);
            particles.transform.parent = null;
            data.SetPart(GetComponentInParent<MechPart>());
            data.SetModule(moduleID);
            if (!isPlayer)
            {
                particles.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Enemy"));
            }
        }

        public override void OnUpdateMesh()
        {
            base.OnUpdateMesh();
            line.SetPosition(0, line.transform.InverseTransformPoint(meshRotator.position));
        }

        protected override void OnRayNotHit()
        {
            base.OnRayNotHit();
            particles.Stop();
        }
        

        public void ShootStart()
        {
            if (IsCantShoot)
            {
                ShootEnd();
                return;
            }
            
            line.gameObject.SetActive(true);
            particles.Stop();
            isShooted = true;
            ShootCalculations();
        }

        public void ShootEnd()
        {
            isShooted = false;
            particles.Stop();
            line.gameObject.SetActive(false);
        }

        public void Shoot()
        {
            if (IsCantShoot)
            {
                ShootEnd();
                return;
            }
            ShootCalculations();
            UpdateVisuals();
        }

        public void UpdateState()
        {
            if (isShooted)
            {
                UpdateVisuals();
            }
            UpdateMesh();
        }


        private void UpdateVisuals()
        {
            line.SetPosition(1, line.transform.InverseTransformPoint(raysData.NextPoint));
        }

        protected void ShootCalculations()
        {
            raysData = ShootRaycasts();
            ParticlesCalculations();
            DamageCalculations();
            
            playerData.AddHeat(data.Heat * Time.deltaTime);
        }




        private void DamageCalculations()
        {
            if (raysData.AvailableHit.collider)
            {
                var part = raysData.AvailableHit.collider.GetComponentInParent<MechPart>();
                if (part == null)
                {
                    part = raysData.AvailableHit.collider.GetComponent<MechPart>();
                }
                if (part != null)
                {
                    part.TakeDamage(Data.Damage * Time.deltaTime);
                }
            }
        }


        private void ParticlesCalculations()
        {
            if (raysData.AvailableHit.collider)
            {
                particles.transform.rotation = Quaternion.FromToRotation(transform.up, raysData.AvailableHit.normal);
                particles.transform.position = raysData.AvailableHit.point + raysData.AvailableHit.normal * 0.01f;
                if (!particles.isPlaying)
                {
                    particles.Play();
                }
            }
        }

        
    }
}
