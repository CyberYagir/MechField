using System.Collections;
using Content.Scripts.Game.GameStateService;
using DG.Tweening;
using RayFire;
using UnityEngine;

namespace Content.Scripts.Weapons.Energy
{
    public class EnergyBullet : Bullet
    {
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private ParticleSystem explosion;
        [SerializeField] private ParticleSystem orb;
        [SerializeField] private float radius = 2;
        [SerializeField] private bool exploded;

        private Tween timerTween;
        private Collider[] results = new Collider[10];
            
        public override void Init(WeaponData data, bool isPlayer, IStateService service)
        {
            base.Init(data, isPlayer, service);
            
            mesh.enabled = true;
            orb.Play(true);
            exploded = false;
            TimerTween(data);

            service.OnChangeState += state =>
            {
                if (state != GameState.Game)
                {
                    if (timerTween != null)
                    {
                        timerTween.Kill();
                    }
                }
                else
                {
                    TimerTween(data);
                }
            };

            StartCoroutine(Loop());

        }


        IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                var size = Physics.OverlapSphereNonAlloc(rb.position, radius, results, mask);
                if (size > 0)
                {
                    foreach (var c in results)
                    {
                        if (c != null)
                        {
                            if (Collide(c))
                            {
                                Hit();
                                yield break;
                            }
                        }
                    }
                    Hit();
                    yield break;
                }
            }
        }
        

        private void TimerTween(WeaponData data)
        {
            timerTween = DOVirtual.DelayedCall(data.MaxDistance / speed, Hit);
        }


        protected override void Hit()
        {
            base.Hit();
            if (exploded) return;

            if (timerTween != null)
            {
                timerTween.Kill();
            }

            transform.DOKill();
            GetComponent<RayfireBomb>().Explode(0);
            rb.velocity = Vector3.zero;
            
            mesh.enabled = false;
            explosion.Play();
            orb.Stop(true);
            
            exploded = true;
            
            DOVirtual.DelayedCall(4, InvokeBulletEnd);
        }
    }
}
