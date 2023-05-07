using Content.Scripts.Game.Player;
using UnityEngine;

namespace Content.Scripts.Weapons.Laser
{
    public class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData data;
        [SerializeField] protected Transform meshRotator;
        protected MechData playerData;
        protected new  Camera camera;

        public bool IsCantShoot => !data.Part.IsWorking || playerData.IsOverheated;
        
        public WeaponData Data => data;


        public virtual void OnUpdateMesh()
        {
            
        }

        protected void UpdateMesh()
        {
            if (meshRotator != null && IsCantShoot == false)
            {
                OnUpdateMesh();
                var customRay = ShootRaycasts();
                meshRotator.LookAt(customRay.NextPoint);
            }
        }

        protected virtual void OnRayNotHit()
        {
            
        }

        protected RaysData ShootRaycasts()
        {
            var nextPos = Vector3.zero;
            bool isHitted = false;

            RaycastHit secondHit;
            var rayData = new RaysData();
            
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, data.MaxDistance, data.Mask))
            {
                nextPos = SecondRaycast(hit.point, out isHitted, out secondHit);
                rayData.AvailableHit = secondHit;
            }
            else
            {
                nextPos = camera.transform.position + camera.transform.forward * data.MaxDistance;
                
                var secondRay = SecondRaycast(nextPos, out isHitted, out secondHit);
                rayData.AvailableHit = secondHit;

                if (isHitted)
                {
                    nextPos = secondRay;
                }
                else
                {
                    OnRayNotHit();
                }
            }

            rayData.NextPoint = nextPos;
            
            return rayData;
        }

        private Vector3 SecondRaycast(Vector3 targetPoint, out bool isHitted, out RaycastHit hit)
        {
            isHitted = Physics.Raycast(transform.position, targetPoint - transform.position, out RaycastHit handHit, data.MaxDistance, data.Mask);
            hit = handHit;
            return handHit.point;
        }
    }
}