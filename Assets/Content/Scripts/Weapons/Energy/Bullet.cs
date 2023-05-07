using System;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using UnityEngine;

namespace Content.Scripts.Weapons.Energy
{
    public abstract class Bullet : MonoBehaviour, IDamage
    {
        [SerializeField] protected float speed;
        [SerializeField] private float damage;
        [SerializeField] private PlayerCamerasShaker.ShakeOptions shake;
        protected Rigidbody rb;
        protected LayerMask mask;
        
        public float Damage => damage;

        public event Action<Bullet> OnBulletEnd;

        protected bool isHitted;
        private IStateService stateService;

        public virtual void Init(WeaponData data, bool isPlayer, IStateService stateService)
        {
            this.stateService = stateService;
            if (isPlayer)
            {
                mask = data.Mask;
            }
            else
            {
                
                mask = LayerMask.GetMask("Default", "Demolish", "Ground", "PlayerMech", "PlayerMechHands");
            }

            rb = GetComponent<Rigidbody>();
            SetBulletSpeed();


            stateService.OnChangeState += OnChangeGameState;
        }

        private void SetBulletSpeed()
        {
            rb.velocity = transform.forward * speed;
        }

        public virtual void OnChangeGameState(GameState state)
        {
            if (state != GameState.Game)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                if (!isHitted)
                {
                    SetBulletSpeed();
                }
            }
        }


        public MechPart GetPart(Transform trns)
        {
            
            var part = trns.GetComponentInParent<MechPart>();
            if (part == null)
            {
                part = trns.GetComponent<MechPart>();
            }
            if (part)
            {
                return part;
            }

            return null;
        }

        public void AddDamage(IDamagable damagable)
        {
            
        }
        
        
        // private void OnTriggerEnter(Collider other)
        // {
        //     Collide(other);
        // }

        public bool Collide(Collider other)
        {
            if (!other.isTrigger)
            {
                var part = other.GetComponentInParent<MechPart>();
                if (part == null)
                {
                    part = other.GetComponent<MechPart>();
                }

                if (part)
                {
                    print(part.transform.name);
                    part.TakeDamage(Damage);
                    if (part.Mech is PlayerBuilder player)
                    {
                        player.ShakeCamera(shake);
                    }

                    return true;
                }
            }

            return false;
        }

        protected virtual void Hit()
        {

        }

        protected void InvokeBulletEnd() => OnBulletEnd?.Invoke(this);
        
    }
}