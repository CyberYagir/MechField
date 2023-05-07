using System.Collections.Generic;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Game.SettingsService;
using Content.Scripts.Global;
using Content.Scripts.Menu.PlayerLoader;
using Content.Scripts.Weapons;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Content.Scripts.Game.Player
{
    public class PlayerBuilder : MechBuilderBase
    {
        
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float playerDamage = 1.5f;

        private PlayerController controller;
        private PlayerAnimation animate;
        private PlayerWeapons weapons;
        private PlayerCamerasShaker shaker;
        private MechModules modules;
        private MechData mechData;
        private IStateService stateService;

        public Camera MainCamera => mainCamera;
        public PlayerController Controller => controller;


        public List<IWeapon> Weapons => weapons.Weapons;
        public bool IsFly => animate.IsFly;

        public MechData Data => mechData;

        public void Init(
            IInputService inputService, 
            ISettingsService settingsService, 
            IPoolService pool,
            IPlayerLoaderService loader,
            IStateService stateService,
            GameDataObject gameData, 
            MapObject.WorldParameters parameters
            )
        {
            this.stateService = stateService;
            this.gameData = gameData;
            
            mechObject = gameData.GetMechByID(loader.PlayerData.GetMechID());
            mechInstance = Instantiate(mechObject.Prefab, transform);
            mechInstance.LoadModulesAndOverrides(loader.PlayerData.GetCurrentModules(), mechObject, gameData);
            
            InitDeath();
            
            ConfigureRb();
            ConfigureOverlaysCamera();
            ConfigureCameras();
            
            if (Controller != null) Controller.Disable();

            mechData = new MechData(mechObject, parameters, playerDamage, stateService);
            controller = new PlayerController(inputService,settingsService, rb, this, Data, stateService);
            animate = new PlayerAnimation(mechInstance.LegsAnimator, rb, this, inputService);
            weapons = new PlayerWeapons(gameData, pool, inputService, this, stateService);
            modules = new MechModules(gameData, MechBuilder);
            shaker = new PlayerCamerasShaker(mechInstance);
            
            mechData.OnOverheat += OnOverheat;
            animate.OnLand += OnLand;
            
            ConfigureThrusters();
            
            
            mechInstance.MechUI.Init(this, inputService, base.gameData);
        }

        private void OnLand()
        {
            ShakeCamera(new PlayerCamerasShaker.ShakeOptions(0.1f, 0.5f, 5, 90));
        }

        public override void OnOverheat(bool state)
        {
            base.OnOverheat(state);
            if (!state)
            {
                mechInstance.PartsData.Torso.DOLocalRotate(new Vector3(controller.YLookAngle * 2f, 0, 0), 0.5f).onComplete += () => controller.SetEnabled(true);
            }
            else
            {
                mechInstance.PartsList.Find(x=>x.Type == MechPart.PartType.Torso).TakeDamageDirect(MechObject.MechValues.WorldParameters.ReactorDamageOverheat);
                controller.SetEnabled(false);
            }

        }

        public override void Death()
        {
            if (IsDead) return;

            animate.Disable();
            
            mechInstance.MechUI.gameObject.Destroy();
            controller.Disable();
            foreach (var ik in mechInstance.GetComponentsInChildren<MechIK>())
            {
                ik.enabled = false;
            }

            var rotator = new GameObject("Rotator")
                .With(x => x.transform.position = MechBuilder.PartsData.Plevis.position)
                .With(x => x.transform.rotation = MechBuilder.PartsData.Plevis.rotation)
                .With(x => x.transform.DORotate(Vector3.up * 360, 20).SetLoops(-1, LoopType.Restart));

            var camera = new GameObject("Camera")
                .With(x => x.AddComponent<Camera>())
                .With(x => x.transform.parent = rotator.transform)
                .With(x => x.transform.localPosition = new Vector3(0, 5, -10))
                .With(x => x.transform.LookAt(rotator.transform));


            DOVirtual.DelayedCall(2, ExplodeMech);

            isDead = true;
        }

        
        
        private void ConfigureThrusters()
        {
            foreach (var thruster in mechInstance.PartsData.Thrusters)
            {
                thruster.Init(controller);
            }
        }

        private void ConfigureRb() => mechObject.MechValues.WorldParameters.Configure(rb);

        private void ConfigureCameras()
        {
            MainCamera.transform.parent = mechInstance.CabineCamera.transform.parent;
            MainCamera.transform.position = mechInstance.CabineCamera.transform.position;
        }
        private void ConfigureOverlaysCamera()
        {
            var additionalData = MainCamera.GetComponent<UniversalAdditionalCameraData>();
            additionalData.cameraStack.Clear();
            additionalData.cameraStack.AddRange(mechInstance.Cameras);
        }

        private void Update()
        {
            if (!IsDead && stateService.IsCanPlayerPlay)
            {
                controller.Update();
                animate.Update();
                weapons.Update();
                mechData.Update();
            }
        }

        public void ShakeCamera(PlayerCamerasShaker.ShakeOptions shake)
        {
            shaker.ShakeCamera(shake);
        }
        
        private void OnDrawGizmos()
        {
            if (animate != null)
                animate.Gizmo();
        }
    }
}
