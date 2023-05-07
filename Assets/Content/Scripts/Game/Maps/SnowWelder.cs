using System.Collections;
using Content.Scripts.Game.PlayerService;
using UnityEngine;

namespace Content.Scripts.Game.Maps
{
    public class SnowWelder : MapPart<MapData>
    {
        [SerializeField] private ParticleSystem particles;


        public override void Init(MapData data)
        {
            base.Init(data);

            StartCoroutine(Loop(data.CreatorService));
        }

        IEnumerator Loop(ICreateService createService)
        {
            while (true)
            {
                yield return null;
                particles.transform.position = createService.PlayerBuilder.transform.position + Vector3.up;
            }
        }
    }
}
