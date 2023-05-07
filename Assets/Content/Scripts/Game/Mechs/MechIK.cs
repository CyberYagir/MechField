using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Game.Mechs
{
    public class MechIK : MonoBehaviour
    {
        [System.Serializable]
        public class Bone
        {
            [SerializeField] private Transform bone;
            [SerializeField] private float addAngle;

            private float angle;
            
            public float AddAngle => addAngle;

            public Transform TargetBone => bone;

            public float Angle => angle;


            public void UpdateAngle(float percent)
            {
                angle = Mathf.Lerp(Angle, addAngle * percent, Time.deltaTime * 5f);
            }
            
        }
        
        [SerializeField] private float footLength, animatedLength, footIKLength;
        [SerializeField] private Transform raycastPoint;
        [SerializeField] private List<Bone> bones;
        [SerializeField] private Transform foot;

        private Quaternion lastFootRotation;
        private LayerMask mask;
        
        private void Awake()
        {
            mask = LayerMask.GetMask("Default", "Ground");
            lastFootRotation = foot.rotation;
        }

        private void LateUpdate()
        {

            Physics.Raycast(raycastPoint.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask);

            var dist = Mathf.Clamp(hit.distance, animatedLength, footLength);
            float percent = (dist - animatedLength) / (footLength - animatedLength);
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].UpdateAngle(percent);
                bones[i].TargetBone.SetXLocalEulerAngles(bones[i].TargetBone.localEulerAngles.x + bones[i].Angle);
            }

            Foot();
        }


        private void Foot()
        {
            foot.transform.rotation = Quaternion.Lerp(foot.transform.rotation, lastFootRotation, 15 * Time.deltaTime);
            
            if (Physics.Raycast(foot.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
            {
                if (hit.distance <= footIKLength)
                {
                    Vector3 crossVector = Vector3.Cross(foot.transform.right, hit.normal);

                    var oldRot = foot.transform.rotation;
                    
                    foot.transform.rotation = Quaternion.LookRotation(crossVector, hit.normal);
                    foot.transform.SetYLocalEulerAngles(0);
                    lastFootRotation = foot.transform.rotation;

                    foot.transform.rotation = oldRot;
                    return;
                }
            }
            lastFootRotation = foot.transform.rotation;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * footLength);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * animatedLength);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(foot.position, foot.position + Vector3.down * footIKLength);
        }
    }
}
