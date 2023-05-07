using System.Collections.Generic;
using Content.Scripts.Game.Bot;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Maps;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PlayerService;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Global;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.Game
{
    public class MapData : MonoBehaviour
    {
        [System.Serializable]
        public class LightData
        {
            [SerializeField] private Material skybox;
            [SerializeField] private Range fogDistance;
            [SerializeField] private Color fogColor;
            [SerializeField] private bool haveFog;


            public void Set()
            {
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fog = haveFog;
                RenderSettings.skybox = skybox;
                RenderSettings.fogColor = fogColor;
                RenderSettings.fogStartDistance = fogDistance.min;
                RenderSettings.fogEndDistance = fogDistance.max;
            }
        }
        
        [System.Serializable]
        public class SpawnPoint
        {
            [SerializeField] private bool isEmpty = true;
            [SerializeField] private Transform point;

            public Transform Point => point;
            public bool IsEmpty => isEmpty;

            public SpawnPoint(Transform point)
            {
                this.point = point;
            }

            public void SetFull() => isEmpty = false;
        }
        
        [System.Serializable]
        public class Sectors
        {
            public class Sector
            {
                private Vector2 coords;
                private Vector3 worldPos;
                private Bounds bounds;

                public Bounds Bounds => bounds;

                public Vector2 Coords => coords;

                public Sector(Vector2 coords, Vector3 worldPos, Vector3 size)
                {
                    this.coords = coords;
                    this.worldPos = worldPos;

                    bounds = new Bounds(this.worldPos, size + Vector3.up * 100);


                }

                public bool IsContains(Vector3 pos) => bounds.Contains(pos);

                public Vector3 GetRandomPoint()
                {
                    var point = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        100,
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    
                    Physics.Raycast(point, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground"));

                    return hit.point;
                }
            }

            [SerializeField] private float sectorWidth, sectorDeep;
            [SerializeField] private float sectorOffsetX, sectorOffsetZ;
            [SerializeField] private float sectorsCount;
            
            private List<List<Sector>> sectors = new List<List<Sector>>(20);



            public void Init()
            {
                for (int x = 0; x < sectorsCount; x++)
                {
                    sectors.Add(new List<Sector>(10));
                    for (int z = 0; z < sectorsCount; z++)
                    {
                        var worldPos = new Vector3(sectorWidth * x, 0, sectorDeep * z) + new Vector3(sectorOffsetX, 0, sectorOffsetZ);
                        sectors[x].Add(new Sector(new Vector2(x, z), worldPos, new Vector3(sectorWidth, 20, sectorDeep)));
                    }
                }
            }

            public void Gizmo()
            {
                if (!Application.isPlaying)
                {
                    for (int x = 0; x < sectorsCount; x++)
                    {
                        for (int z = 0; z < sectorsCount; z++)
                        {
                            Gizmos.DrawWireCube(new Vector3(sectorWidth * x, 0, sectorDeep * z) + new Vector3(sectorOffsetX, 0, sectorOffsetZ), new Vector3(sectorWidth, 20, sectorDeep));
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < sectorsCount; x++)
                    {
                        for (int z = 0; z < sectorsCount; z++)
                        {
                            Gizmos.DrawWireCube(sectors[x][z].Bounds.center, sectors[x][z].Bounds.size);
                        }
                    }
                }
            }

            public Sector GetSectorByPos(Vector3 pos)
            {
                for (int x = 0; x < sectorsCount; x++)
                {
                    for (int z = 0; z < sectorsCount; z++)
                    {
                        if (sectors[x][z].IsContains(pos))
                        {
                            return sectors[x][z];
                        }
                    }
                }
                return null;
            }

            public Sector GetRandomSector()
            {
                return sectors.GetRandomItem().GetRandomItem();
            }
        }

        [System.Serializable]
        public class BotsHolder
        {
            [SerializeField] private List<MechObject> meches;

            public List<MechObject> Meches => meches;
        }

        [SerializeField] private int id;
        [SerializeField] private NavMeshSurface navMesh;
        [SerializeField] private GameObject dynamicObjects;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private LightData lightningData;
        [SerializeField] private List<MapPart<MapData>> mapParts;
        [SerializeField] private Sectors sectors;
        [SerializeField] private List<BotsHolder> botsLists;

        private List<BotBuilder> bots;
        private List<SpawnPoint> spawns = new List<SpawnPoint>(10);
        private ICreateService createService;
        private GameDataObject gameData;

        public ICreateService CreatorService => createService;

        public Sectors SectorsData => sectors;

        public List<BotBuilder> Bots => bots;

        [Inject]
        public void Constructor(ICreateService createService, GameDataObject gameData)
        {
            this.gameData = gameData;
            this.createService = createService;
        }

        public void Init()
        {
            foreach (var item in spawnPoints)
            {
                spawns.Add(new SpawnPoint(item));
            }

            foreach (var part in mapParts)
            {
                part.Init(this);
            }

            sectors.Init();
        }

        public SpawnPoint GetRandomEmptyPoint()
        {
            var empty = spawns.FindAll(x => x.IsEmpty);
            return empty.GetRandomItem();
        }
        public void SetLightning() => lightningData.Set();

        public void SetGravity(MapObject mapObject) => mapObject.Parameters.SetGravity();

        public void BuildNavMesh()
        {
            dynamicObjects.gameObject.SetActive(false);
            navMesh.BuildNavMesh();
            dynamicObjects.gameObject.SetActive(true);
        }

        public void SpawnRandomBots(PlayerBuilder playerBuilder, IPoolService pool, IStateService stateService)
        {
            var randomBots = botsLists.GetRandomItem();
            bots = new List<BotBuilder>(10);
            
            for (int i = 0; i < randomBots.Meches.Count; i++)
            {
                var point = GetRandomEmptyPoint();
                if (point != null)
                {
                    var bot = Instantiate(gameData.BotPrefab, point.Point.position, point.Point.rotation);
                    var id = i;
                    bot
                        .With(x => x.Init(randomBots.Meches[id], this, playerBuilder, gameData, pool, stateService, createService.CurrentMap.Parameters))
                        .With(x => x.CreateBot());
                    
                    this.bots.Add(bot);
                    
                    point.SetFull();
                    
                }
            }
        }

        public bool PointIsOnNavmesh(Vector3 point)
        {
            var hit = HitOnNavMesh(point);
            return hit.hit;
        }

        public NavMeshHit HitOnNavMesh(Vector3 point, float maxDistance = 4f)
        {
            NavMesh.SamplePosition(point, out NavMeshHit hit, maxDistance, 1 << NavMesh.GetAreaFromName("Walkable"));
            return hit;
        }

        private void OnDrawGizmosSelected()
        {
            sectors.Gizmo();
        }
    }
}
