using Content.Scripts.Game.GameStateService;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Weapons.Energy
{
    public class MachinegunBullet : Bullet
    {
        [SerializeField] private float accuracity;
        [SerializeField] private TrailRenderer trail;
        private Tween autoDestroy = null;

        private bool isStopped = false;

        private Vector3 startPos;
        
        public override void Init(WeaponData data, bool isPlayer, IStateService service)
        {
            base.Init(data, isPlayer, service);

            isStopped = false;
            rb.velocity = Vector3.zero;
            trail.Clear();
            autoDestroy = DOVirtual.DelayedCall(5, Hit);
            startPos = transform.position;
            
            BulletMove(data);
        }

        private void BulletMove(WeaponData data)
        {
            var spread = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * accuracity);
            var dir = transform.forward + transform.TransformDirection(spread);
            var time = data.MaxDistance / speed;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, mask))
            {
                time = hit.distance / speed;
                var tween = transform.DOMove(hit.point, time);
                tween.onComplete += Hit;

                if (GetPart(hit.collider.transform))
                {
                    tween.onComplete += () =>
                    {
                        if (Physics.Raycast(startPos, dir, out RaycastHit hit2, Mathf.Infinity, mask))
                        {
                            var part = GetPart(hit2.collider.transform);
                            if (part)
                            {
                                part.TakeDamage(Damage);
                            }
                        }
                    };
                }
            }
            else
            {
                transform.DOMove(transform.position + dir * data.MaxDistance, time).onComplete += Hit;
            }

            trail.time = time;
        }
        
        
        protected override void Hit()
        {
            if (!isStopped)
            {
                base.Hit();
                autoDestroy.Kill();
                transform.DOKill();
                DOVirtual.DelayedCall(trail.time, InvokeBulletEnd);
                isStopped = true;
            }
        }
    }
}
