using System;
using System.Collections;
using Content.Scripts.Game.Mechs;
using UnityEngine;

namespace Content.Scripts.Game.Maps
{
    public class DeathTrigger : MonoBehaviour
    {
        [SerializeField] private float damage = 60;
        private MechBuilder builder;
        private void OnTriggerEnter(Collider other)
        {
            if (builder == null)
            {
                builder = other.GetComponentInParent<MechBuilder>();
                if (builder != null)
                {
                    print("enter");
                    StopAllCoroutines();
                    StartCoroutine(DamageLoop());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var exitBuilder = other.GetComponentInParent<MechBuilder>();
            if (builder == exitBuilder)
            {
                builder = null;
            }
        }

        IEnumerator DamageLoop()
        {
            while (builder)
            {
                foreach (var part in builder.PartsList)
                {
                    if (part.IsWorking)
                    {
                        part.TakeDamage(damage);
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
