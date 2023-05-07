using System;
using System.Collections.Generic;
using Content.Scripts.Game.Bot;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Menu.UIService;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using Zenject;
using Object = UnityEngine.Object;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/GameData", fileName = "Game Data", order = 0)]
    public class GameDataObject : ScriptableObjectInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<GameDataObject>().FromInstance(this).AsSingle();

#if UNITY_EDITOR
            foreach (var module in modules)
            {
                if (module.Module.UID == 0)
                {
                    module.Module.CreateUID();
                    EditorUtility.SetDirty(module.Module);
                }
            }
#endif
            
        }
        
        [Serializable]
        public class ModuleHolder
        {
            [SerializeField] private int id;
            [SerializeField] private ModuleObject module;
            [SerializeField] private float dropWeight;

            public int ID => id;

            public ModuleObject Module => module;

            public float DropWeight => dropWeight;
        }
        [Serializable]
        public class MechHolder
        {
            [SerializeField] private MechObject mech;
            [SerializeField] private int cost;
            [SerializeField] private LocalizedString mechDesc; 
            
            private RenderTexture mechPreview;
            
            public event Action<RenderTexture> OnSetPreview;
            
            public MechObject MechObject => mech;

            public RenderTexture MechPreview => mechPreview;

            public string MechDesc => mechDesc.GetLocalizedString();

            public int Cost => cost;


            public void SetPreview(RenderTexture rn)
            {
                mechPreview = rn;
                OnSetPreview?.Invoke(rn);
            } 
        }
        [Serializable]
        public class LocationsData
        {
            [SerializeField] private List<Sprite> icons = new List<Sprite>();
            [SerializeField] private List<MapObject> locations;

            public int GetRandomIcon() => icons.GetRandomIndex();

            public int GetRandomMap() => locations.GetRandomItem<MapObject>().MapID;

            public MapObject GetMapByID(int dataMapID)
            {
                var item = locations.Find(x => x.MapID == dataMapID);
                return item;
            }

            public Sprite GetIconByID(int iconID)
            {
                if (iconID >= icons.Count || iconID < 0 || icons.Count == 0) return null;
                return icons[iconID];
            }
        }

        [Serializable]
        public class SaveData
        {
            [SerializeField] private Range missionsCount;

            public int GetMissionsCount() => (int)missionsCount.RandomWithin();
        }
        
        public class MemoryData
        {
            private Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();
            public bool IsLoaded(AssetReference asset)
            {
                return loadedAssets.ContainsKey(asset.AssetGUID);
            }

            public void AddObject(AssetReference asset, Object loadedResult)
            {
                if (!IsLoaded(asset))
                {
                    loadedAssets.Add(asset.AssetGUID, loadedResult);
                }
            }

            public Object Get(AssetReference activeMapMapPrefab)
            {
                if (IsLoaded(activeMapMapPrefab))
                {
                    foreach (var item in loadedAssets)
                    {
                        if (item.Key == activeMapMapPrefab.AssetGUID)
                        {
                            return item.Value;
                        }
                    }
                }

                return null;
            }

            public Dictionary<string, Object> GetLoadedList() => new(loadedAssets);

            public void Unload(AssetReference asset)
            {
                if (IsLoaded(asset))
                {
                    asset.ReleaseAsset();
                    loadedAssets.Remove(asset.AssetGUID);
                }
            }
        }
        
        
        
        [SerializeField] private List<MechHolder> meches;
        [SerializeField] private List<ModuleHolder> modules;
        [SerializeField] private LocationsData locationsData;
        [SerializeField] private SaveData saveData;
        
        [Space]
        [SerializeField] private BotBuilder botPrefab;
        [SerializeField] private ParticleSystem explosion;
        [SerializeField] private Range rewardDropCount;
        [SerializeField] private float sellCostMultiplier;
        [SerializeField] private List<UIMechPartItem.NameToLocalizedText> mechPartsNames;
        private MemoryData memoryData = new MemoryData();
        
        public SaveData PlayerSaveData => saveData;

        public MemoryData TempData => memoryData;

        public BotBuilder BotPrefab => botPrefab;

        public ParticleSystem ExplosionParticle => explosion;

        public Range RewardDropCount => rewardDropCount;

        public float SellCostMultiplier => sellCostMultiplier;

        public List<UIMechPartItem.NameToLocalizedText> MechPartsNames => mechPartsNames;

        public ModuleObject GetModuleByID(int id)
        {
            var module = modules.Find(x => x.ID == id);
            if (module == null) return null;
            return module.Module;
        }
        
        
        public MechObject GetMechByID(int id)=>meches.Find(x => x.MechObject.ID == id).MechObject;

        public List<MechHolder> GetListMeches() => new(meches);
        public List<ModuleHolder> GetListModules() => new(modules);
        public void SetMechIcon(RenderTexture rn, int i) => meches[i].SetPreview(rn);
        
        public LocationsData GetLocationData() => locationsData;

        public int GetModuleID(ModuleObject module)
        {
            var item = modules.Find(x => x.Module.UID == module.UID);
            if (item != null) return item.ID;
            return -1;
        }

        public (Sprite, MapObject) GetMapData(int iconID, int dataMapID) => (locationsData.GetIconByID(iconID), GetMapData(dataMapID));

        public MapObject GetMapData(int missionMapID) => locationsData.GetMapByID(missionMapID);
        
    }
}
