using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Game.SettingsService;
using Content.Scripts.Game.UI;
using Content.Scripts.Global;
using Content.Scripts.Menu.PlayerLoader;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Game.PlayerService
{
    public class CreateService : MonoInstaller, ICreateService
    {
        public PlayerBuilder PlayerBuilder => localPlayer;
        public MapObject CurrentMap => mapObject;
        public MapData CurrentMapInstance => mapInstance;


        [SerializeField] private GlobalUIManager globalUI;
        [SerializeField] private PlayerBuilder prefab;
        [SerializeField] private MapObject mapObject;
        [SerializeField] private MapData mapInstance;

        private PlayerBuilder localPlayer;

        [Inject]
        public void Constructor(
            IInputService inputService,
            ISettingsService settingsService,
            ICreateService createService,
            IPoolService pool,
            IPlayerLoaderService loader,
            IStateService stateService,
            GameDataObject gameData
        )
        {

            mapObject = gameData.GetMapData(loader.PlayerData.GetActiveMissionMap());

            mapInstance = Container.InstantiatePrefab(gameData.TempData.Get(mapObject.MapPrefab))
                    .Get<MapData>()
                    .With(x => x.Init())
                    .With(x => x.SetLightning())
                    .With(x => x.SetGravity(mapObject))
                    .With(x => x.BuildNavMesh())
                ;


            var point =
                    mapInstance.GetRandomEmptyPoint()
                        .With(x => x.SetFull())
                ;

            localPlayer =
                Instantiate(prefab, point.Point.position, point.Point.rotation)
                    .With(x =>
                        x.Init(inputService, settingsService, pool, loader, stateService, gameData, mapObject.Parameters))
                ;

            mapInstance.SpawnRandomBots(localPlayer, pool, stateService);

            globalUI.Init(inputService, stateService, createService, loader, localPlayer, mapInstance, gameData);
        }

        public override void InstallBindings()
        {
            // base.InstallBindings();
        }
    }
}
