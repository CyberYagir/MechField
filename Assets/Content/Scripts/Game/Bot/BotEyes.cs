using Content.Scripts.Game.Player;
using UnityEngine;

namespace Content.Scripts.Game.Bot
{
    public class BotEyes
    {
        private float distance; 
        private float angle;
        private LayerMask mask;
        private Transform torso;

        public BotEyes(float distance, float angle, Transform torso)
        {
            this.distance = distance;
            this.angle = angle;
            this.torso = torso;
            this.mask = LayerMask.GetMask("PlayerMechCollider", "Default");
        }

        public Vector3 DirFromAngle(float angle, bool isGlobal)
        {
            if (!isGlobal)
            {
                angle += torso.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public bool IsVisible(Transform transform)
        {
            var targetPos = transform.position;
            targetPos.y = torso.position.y;
            if (torso.position.ToDistance(targetPos) >= distance) return false;
                
            return Vector3.Angle(torso.forward, (targetPos - torso.position).normalized) < angle/2f;
        }

        public bool IsPhysicsVisible(Transform transform)
        {
            if (Physics.Raycast(torso.position, (transform.position - torso.position), out RaycastHit hit, distance, mask))
            {
                if (hit.transform.GetComponent<PlayerBuilder>()) return true;
            }

            return false;
        }
    }
}