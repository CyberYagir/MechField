using System.Collections.Generic;
using Content.Scripts.Global;
using PathCreation;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Content.Scripts.Game.Bot
{
    public partial class BotBuilder
    {
        [System.Serializable]
        public class BotData
        {
            [SerializeField] private float seeDistance;
            [SerializeField] private float seeAngle;
            [SerializeField] private float cabineRotateSpeed = 5;
            [SerializeField] private float mainCameraRotateSpeed = 20;
            [SerializeField] private float heightOffcet = 2.5f;
            [SerializeField] private float pathLerpSpeed = 5;
            [SerializeField] private float fullBraveHealth = 0.8f;
            [SerializeField] private float shootDuration = 5f;
            [SerializeField] private float missTargetTime = 2f;
            [SerializeField] private float minAgrDistance = 10;
            [SerializeField] private float minCoolingToShoot = 0.8f;
            [SerializeField] private float damageModify = 0.2f;
            
            
            [SerializeField] private List<PathCreator> attackPaths;
            [SerializeField] private AssetReference namesList;
            [SerializeField] private LayerMask weaponsMask;
            public float HeightOffcet => heightOffcet;
            public float CabineRotateSpeed => cabineRotateSpeed;
            public float SeeAngle => seeAngle;
            public float SeeDistance => seeDistance;
            public float PathLerpSpeed => pathLerpSpeed;
            public float FullBraveHealth => fullBraveHealth;
            public float ShootDuration => shootDuration;
            public float MissTargetTime => missTargetTime;
            public float MinAgrDistance => minAgrDistance;
            public LayerMask WeaponsLayerMask => weaponsMask;

            public float MainCameraRotateSpeed => mainCameraRotateSpeed;

            public float MinCoolingToShoot => minCoolingToShoot;

            public float DamageModify => damageModify;


            public List<PathCreator> SpawnPaths(Transform parent)
            {
                List<PathCreator> list = new List<PathCreator>(attackPaths.Count);
                foreach (var p in attackPaths)
                {
                    list.Add(
                        Instantiate(p, parent)
                            .With(x => x.gameObject.SetActive(false)
                            )
                    );
                }
                return list;
            }

            public string GetName(GameDataObject gameData) => (gameData.TempData.Get(namesList) as NamesListObject)?.GetRandomName();
        }
    }
}