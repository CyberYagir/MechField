using System;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Global;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class MechBuilderBase : MonoBehaviour
    {
        [SerializeField] protected MechObject mechObject;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected bool isDead;

        protected MechBuilder mechInstance;
        protected GameDataObject gameData;

        public event Action<MechBuilderBase> OnDeath;

        public MechObject MechObject => mechObject;
        public MechBuilder MechBuilder => mechInstance;

        public bool IsDead => isDead;


        public bool IsCriticalDamage()
        {
            var list = MechBuilder.PartsList;

            var torso = list.Find(x => x.Type == MechPart.PartType.Torso);
            if (torso.HealthPercent > 0.4f)
                return false;


            var lhand = list.Find(x => x.Type == MechPart.PartType.LeftHand);
            var rhand = list.Find(x => x.Type == MechPart.PartType.RightHand);
            if (lhand.HealthPercent + rhand.HealthPercent < 0.4f * 2f)
                return false;

            var allHealths = AvgHealth();

            if (allHealths / list.Count < 0.4f) return false;


            return true;
        }

        
        
        protected void ExplodeMech()
        {
            foreach (var parts in MechBuilder.PartsList)
            {
                if (parts.IsWorking)
                {
                    parts.SetDestroyType(MechPart.PartDestroyType.Ragdoll);
                    parts.TakeDamageDirect(Mathf.Infinity);
                }
            }

            DOVirtual.DelayedCall(0.5f, () =>
            {
                var othersObjects = MechBuilder.PartsData.Plevis.GetComponentsInChildren<MeshRenderer>();
                foreach (var obj in othersObjects)
                {
                    obj.gameObject.AddComponent<Rigidbody>();
                }

                Instantiate(gameData.ExplosionParticle, mechInstance.Torso.position, Quaternion.identity)
                    .With(x => x.transform.localScale *= 5);
                
                OnDeath?.Invoke(this);
                MechBuilder.GetComponent<Collider>().enabled = false;
            });
        }
        
        public float AvgHealth()
        {
            var allHealths = 0f;
            foreach (var p in MechBuilder.PartsList)
            {
                allHealths += p.HealthPercent;
            }

            return allHealths;
        }

        public void InitDeath()
        {
            mechInstance.PartsList.ForEach(x => x.OnBreak += OnBreakElement);
        }

        private void OnBreakElement(MechPart part)
        {
            Instantiate(gameData.ExplosionParticle, part.transform.position, Quaternion.identity);
            if (IsCanDeath()) Death();
        }

        public Transform GetMostDamagedPart()
        {
            var minHealth = 99f;
            MechPart part = null;
            foreach (var p in MechBuilder.PartsList)
            {
                if (p.IsWorking)
                {
                    if (p.HealthPercent <= minHealth)
                    {
                        minHealth = p.HealthPercent;
                        part = p;
                    }
                }
            }

            if (part == null) return null;
            return part.RandomSubMesh();
        }

        public Transform GetRandomPart()
        {
            return MechBuilder.PartsList.FindAll(x=>x.IsWorking).GetRandomItem().RandomSubMesh();
        }

        public virtual void OnOverheat(bool state)
        {
            if (state)
            {
                mechInstance.PartsData.YRotator.DOLocalRotate(mechInstance.PartsData.Plevis.localEulerAngles, 0.5f);
                mechInstance.PartsData.Torso.DOLocalRotate(new Vector3(50, 0, 0), 0.5f);
            }
        }


        public bool IsCanDeath()
        {
            var list = MechBuilder.PartsList;
            var lleg = list.Find(x => x.Type == MechPart.PartType.LeftLeg);
            var rleg = list.Find(x => x.Type == MechPart.PartType.RightLeg);

            if (!lleg.IsWorking && !rleg.IsWorking)
            {
                return true;
            }

            var torso = list.Find(x => x.Type == MechPart.PartType.Torso);
            if (!torso.IsWorking)
            {
                return true;
            }

            return false;
        }

        public virtual void Death()
        {
            
        }
    }
}