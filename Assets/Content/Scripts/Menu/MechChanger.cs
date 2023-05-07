using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Global;
using Content.Scripts.Menu.PlayerLoader;
using Content.Scripts.Menu.UIService;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Menu
{
    
    public sealed class MechChanger : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private CameraRotator rotator;
        
        private MechObject mech;
        private MechBuilder spawnedMech;
        private IPlayerLoaderService playerLoaderService;
        private GameDataObject gameData;
        private MeshWeapons meshWeapons;
        public MechBuilder SpawnedMech => spawnedMech;


        [Inject]
        public void Constructor(IPlayerLoaderService playerLoaderService, GameDataObject gameDataObject, IInputService input, MenuUIService uiService)
        {
            this.playerLoaderService = playerLoaderService;
            this.gameData = gameDataObject;
            
            playerLoaderService.PlayerData.OnChageMech += ChangeMech;
            ChangeMech(gameDataObject.GetMechByID(playerLoaderService.PlayerData.GetMechID()));
            rotator.Init(input, uiService);


            playerLoaderService.PlayerData.OnStorageUpdate += UpdateWeapons;
        }

        private void UpdateWeapons()
        {
            if (meshWeapons != null) 
                meshWeapons.Weapons.ForEach(x=>x.Transform.gameObject.Destroy());
            
            meshWeapons = new MeshWeapons(gameData, spawnedMech);
            ScaleWeapons();
        }


        private void ChangeMech(MechObject newMech)
        {
            if (newMech != mech)
            {
                if (SpawnedMech)
                {
                    meshWeapons.Weapons.ForEach(x=>x.Transform.gameObject.Destroy());
                    Destroy(SpawnedMech.gameObject);
                }

                var builder = Instantiate(newMech.Prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                builder.Torso.transform.position = builder.PartsData.PlevisPoint.transform.position;
                builder.LegsAnimator.enabled = false;

                foreach (var ik in builder.GetComponentsInChildren<MechIK>())
                {
                    ik.enabled = false;
                }
                
                spawnedMech = builder;
                mech = newMech;
                spawnedMech.MechUI.gameObject.SetActive(false);
                
                spawnedMech.LoadModulesAndOverrides(playerLoaderService.PlayerData.GetCurrentModules(), newMech, gameData);
                meshWeapons = new MeshWeapons(gameData, spawnedMech);
                ScaleWeapons();
            }
        }

        public void ScaleWeapons()
        {
            foreach (var x in meshWeapons.Weapons)
                x.Transform.localScale *= transform.localScale.x;
        }
    }
}
