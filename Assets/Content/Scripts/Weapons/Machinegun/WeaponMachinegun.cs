using System.Collections;
using Content.Scripts.Weapons.Energy;
using UnityEngine;

namespace Content.Scripts.Weapons.Machinegun
{
    public class WeaponMachinegun : WeaponEnergy
    {
        private float rotateSpeed;
        private float maxRotateSpeed = 720;
        private float gravity = 300;
        private float upSpeed => maxRotateSpeed * 2f + gravity;
        
        private void Awake()
        {
            StartCoroutine(UpdateRotation());
        }

        public override void OnShoot()
        {
            data.RemoveBullet();
        }

        public override bool HaveBullets()
        {
            return data.BulletsCount > 0;
        }

        public override void UpdateVisuals()
        {
            rotateSpeed += upSpeed * Time.deltaTime;
        }


        IEnumerator UpdateRotation()
        {
            while (true)
            {
                rotateSpeed -= gravity * Time.deltaTime;

                rotateSpeed = Mathf.Clamp(rotateSpeed, 0, maxRotateSpeed);
                
                cannonMesh.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime, Space.Self);
                yield return null;
            }
        }
    }
}
