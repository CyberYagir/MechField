using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Game.Player;
using Content.Scripts.Global;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Game.Mechs
{
    public class MechPart : MonoBehaviour
    {   
        public enum PartType
        {
            Torso = 1, LeftHand = 2, RightHand = 3, LeftLeg = 4, RightLeg = 5,
        }
        public enum PartDestroyType
        {
            None, Ragdoll
        }

        
        public const float ARMOR_PENETRATION = 0.8f;
        
        
        [SerializeField] private PartType type;
        [SerializeField] private PartDestroyType destroyType;
        [SerializeField] private float health, armor;


        private List<Transform> subMeshes = new List<Transform>();


        private float maxHealth, maxArmor;
        private MechBuilderBase mech;
        private bool isBreaked;

        public event Action<float> OnTakeDamage;
        public event Action<MechPart> OnBreak;
        public bool IsWorking => health > 0;
        public PartType Type => type;
        public float HealthPercent => Mathf.Clamp01(health / maxHealth);
        public float ArmorPercent => Mathf.Clamp01(armor / maxArmor);

        public MechBuilderBase Mech => mech;


        public void Init()
        {
            subMeshes = GetComponentsInChildren<Transform>().ToList();
            OnTakeDamage += CheckPartLive;
            mech = transform.GetComponentInParent<MechBuilderBase>();
        }

        private void CheckPartLive(float health)
        {
            if (isBreaked) return;
            if (health <= 0)
            {
                OnBreak?.Invoke(this);
                
                if (destroyType == PartDestroyType.Ragdoll)
                {
                    transform.ChangeLayerWithChilds(LayerMask.NameToLayer("DefaultGroundOnly"));
                    
                    var rb = transform.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    
                    if (Mech.MechBuilder.CabineCamera != null && Mech.MechBuilder.HandsCamera != null && Mech is PlayerBuilder)
                    {
                        var cam1Pos = Mech.MechBuilder.Torso.InverseTransformPoint(Mech.MechBuilder.CabineCamera.transform.position);
                        var cam2Pos = Mech.MechBuilder.Torso.InverseTransformPoint(Mech.MechBuilder.HandsCamera.transform.position);
                        transform.DOLocalMoveY(cam1Pos.y - cam2Pos.y, 0.2f).onComplete += () => DOVirtual.DelayedCall(0.1f, delegate { Ragdoll(rb); });
                    }
                    else
                    {
                        Ragdoll(rb);
                    }
                }
                
                if (type == PartType.LeftLeg || type == PartType.RightLeg)
                {
                    transform.GetComponent<MechIK>().enabled = false;
                }

                isBreaked = true;
            }

            
            void Ragdoll(Rigidbody rb)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(2000, Mech.MechBuilder.Torso.transform.position - Vector3.down * 2f, 10);
                transform.parent = null;
            }
        }

        public void AddValues(ModuleArmorAdder armorAdder)
        {
            if (armorAdder != null)
            {
                this.health += armorAdder.Health;
                this.armor += armorAdder.Armor;
            }

            maxHealth = health;
            maxArmor = armor;
        }

        public void TakeDamage(float damage)
        {
            if (armor > 0)
            {
                health -= damage * (1f - ARMOR_PENETRATION);
                armor -= damage * ARMOR_PENETRATION;
            }
            else
            {
                health -= damage;
            }
            OnTakeDamage?.Invoke(health);
        }


        public Transform RandomSubMesh()
        {
            return subMeshes.GetRandomItem();
        }

        public void TakeDamageDirect(float damage)
        {
            health -= damage;
            OnTakeDamage?.Invoke(health);
        }

        public void SetDestroyType(PartDestroyType ragdoll)
        {
            destroyType = ragdoll;
        }
    }
}
