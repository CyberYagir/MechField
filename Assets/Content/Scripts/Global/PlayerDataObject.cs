using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Weapons;
using UnityEngine;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/PlayerDataObject", fileName = "PlayerData", order = 0)]
    public class PlayerDataObject : SavedObject
    {
        [Serializable]
        public class PlayerMech
        {
            [Serializable]
            public class ModuleData
            {
                [SerializeField] private int uid;
                [SerializeField] private int moduleID;

                public ModuleData(int uid, int moduleID)
                {
                    this.uid = uid;
                    this.moduleID = moduleID;
                }

                public int ModuleID => moduleID;
                public int SlotID => uid;

                public void SetModule(int moduleID)
                {
                    this.moduleID = moduleID;
                }
            }

            [SerializeField] private int mechID;
            [SerializeField] private List<ModuleData> modules = new List<ModuleData>();

            public int MechID => mechID;

            public List<ModuleData> Modules => modules;

            public PlayerMech(MechObject mech)
            {
                mechID = mech.ID;
            }

            public PlayerMech()
            {

            }

        }


        [Serializable]
        public class PlayerAngar
        {
            [SerializeField] private List<PlayerMech> meches = new List<PlayerMech>(5);
            [SerializeField] private int currentMechID = -1;
            public event Action<int> OnAddMech;

            public int CurrentMechID => currentMechID;


            public void AddMechToAngar(MechObject mech)
            {
                if (!HaveMech(mech.ID))
                {
                    meches.Add(new PlayerMech(mech));
                    OnAddMech?.Invoke(mech.ID);
                }
            }

            public bool HaveMech(int id)
            {
                return meches.Find(x => x.MechID == id) != null;
            }

            public void SetPlayerMech(int mechID)
            {
                currentMechID = mechID;
            }

            public List<PlayerMech.ModuleData> GetCurrentModules()
            {
                return meches.Find(x => x.MechID == CurrentMechID).Modules;
            }

            public void SetModule(Slot newSlot)
            {
                var mech = meches.Find(x=>x.MechID == currentMechID);
                var slot = mech.Modules.Find(x => x.SlotID == newSlot.SlotID);
                if (slot == null)
                {
                    slot = new PlayerMech.ModuleData(newSlot.SlotID, newSlot.ModuleID);
                    mech.Modules.Add(slot);
                    return;
                }

                slot.SetModule(newSlot.ModuleID);
            }
        }

        [Serializable]
        public class PlayerStorage
        {
            [Serializable]
            public class StorageModule
            {
                [SerializeField] private int guid;
                [SerializeField] private int moduleId;

                public int ModuleId => moduleId;
                public int GUID => guid;

                public StorageModule(int moduleId)
                {
                    guid = Guid.NewGuid().GetHashCode();
                    this.moduleId = moduleId;
                }

            }

            [SerializeField] private List<StorageModule> modules = new List<StorageModule>();

            public List<StorageModule> GetStorage() => new List<StorageModule>(modules);

            public StorageModule AddToStorage(int id)
            {
                var it = new StorageModule(id);
                modules.Add(it);
                return it;
            }

            public bool HaveItem(int id) => modules.Find(x => x.ModuleId == id) != null;
            public StorageModule GetItem(int GUID) => modules.Find(x => x.GUID == GUID);
            public bool RemoveItem(int GUID) => modules.RemoveAll(x => x.GUID == GUID) != 0;

            public int ItemsCountOfId(int moduleUid) => modules.Count(x => x.ModuleId == moduleUid);

            public StorageModule RemoveItemByID(int moduleID)
            {
                var module = modules.Find(x => x.ModuleId == moduleID);

                if (module != null)
                {
                    modules.Remove(module);
                }

                return module;
            }
        }
        
        [Serializable]
        public class PlayerMissions
        {
            [Serializable]
            public class Mission
            {
                public enum Difficulty
                {
                    Easy, Normal, Hard
                }
                
                [SerializeField] private int guid;
                [SerializeField] private Difficulty type;
                [Space]
                [SerializeField] private int iconID;
                [SerializeField] private int mapID;
                [Space]
                [SerializeField] private bool completed;

                public bool IsCompleted => completed;

                public int MapID => mapID;

                public int IconID => iconID;

                public int GUID => guid;

                public Mission(GameDataObject gameData, bool isFirst, int lastMap)
                {
                    var locData = gameData.GetLocationData();
                    guid = Guid.NewGuid().GetHashCode();
                    type = !isFirst ? Extensions.GetRandomEnum<Difficulty>() : Difficulty.Easy;
                    iconID = locData.GetRandomIcon();
                    do
                    {
                        mapID = locData.GetRandomMap();
                    } while (mapID == lastMap);
                    
                }

                public void Complete(bool state)
                {
                    completed = state;
                }
            }

            [SerializeField] private List<Mission> missions = new List<Mission>(10);
            [Space]
            [SerializeField] private int currentMission;

            public List<Mission> Missions => missions;


            public bool CalculateMissions(GameDataObject gameData)
            {
                if (missions.Find(x => !x.IsCompleted) == null)
                {
                    missions.Clear();;
                }
                
                
                var lastMap = -1;                

                if (missions.Count == 0)
                {
                    for (int i = 0; i < gameData.PlayerSaveData.GetMissionsCount(); i++)
                    {
                        var mission = new Mission(gameData, i == 0, lastMap);
                        lastMap = mission.MapID;
                        missions.Add(mission);
                    }

                    ValidateMissions(gameData);
                    return true;
                }
                
                return ValidateMissions(gameData);
            }

            public bool ValidateMissions(GameDataObject gameData)
            {
                List<Mission> corrupted = new List<Mission>();
                for (int i = 0; i < missions.Count; i++)
                {
                    var (icon, map) = gameData.GetMapData(missions[i].IconID, missions[i].MapID);
                    if (icon == null || map == null)
                    {
                        corrupted.Add(missions[i]);
                    }
                }

                if (corrupted.Count == 0) return false;

                foreach (var mission in corrupted)
                {
                    missions.Remove(mission);
                }

                if (missions.Count == 0)
                {
                    CalculateMissions(gameData);
                }

                return true;
            }

            public Mission GetActiveMission()
            {
                return missions.Find(x => x.GUID == currentMission);
            }

            public void SetActiveMission(int selectedItemGuid)
            {
                currentMission = selectedItemGuid;
            }

            public void CompleteCurrentMission()
            {
                var curr = missions.Find(x => x.GUID == currentMission);
                if (curr != null)
                {
                    curr.Complete(true);
                }
            }
        }
        

        [SerializeField] private string pilotName = "Player";
        [SerializeField] private int credits = 0;
        [SerializeField] private PlayerAngar playerAngar = new PlayerAngar();
        [SerializeField] private PlayerStorage playerStorage = new PlayerStorage();
        [SerializeField] private PlayerMissions playerMissions = new PlayerMissions();
        
        
        public event Action<MechObject> OnChageMech;
        public event Action<string> OnChangeName;
        public event Action<int> OnIncreaseMoney;
        public event Action OnStorageUpdate;

        protected override string GetFilePath() => GetPathFolder() + @"\save.dat";

        public bool HaveSave()
        {
            return File.Exists(GetFilePath());
        }


        public void SetMech(MechObject mech, bool preview = false)
        {
            if (preview)
            {
                OnChageMech?.Invoke(mech);
            }
            else
            {
                if (AngarHaveMech(mech.ID))
                {
                    playerAngar.SetPlayerMech(mech.ID);
                    OnChageMech?.Invoke(mech);
                    SaveFile();
                }
            }
        }

        public void SetPlayerName(string newName)
        {
            pilotName = newName;
            OnChangeName?.Invoke(newName);
        }

        public void IncreaseMoney(int count)
        {
            credits += count;
            OnIncreaseMoney?.Invoke(credits);
        }

        public int GetMechID() => playerAngar.CurrentMechID;

        public int GetMoney() => credits;

        public bool BuyMech(GameDataObject.MechHolder selectedMech)
        {
            if (credits >= selectedMech.Cost)
            {
                IncreaseMoney(-selectedMech.Cost);
                playerAngar.AddMechToAngar(selectedMech.MechObject);
                SaveFile();
                return true;
            }

            return false;
        }

        public void AngarAddMech(MechObject mech) => playerAngar.AddMechToAngar(mech);

        public bool AngarHaveMech(int id) => playerAngar.HaveMech(id);

        public List<PlayerMech.ModuleData> GetCurrentModules() => playerAngar.GetCurrentModules();

        public List<PlayerStorage.StorageModule> GetStorageItems() => playerStorage.GetStorage();
        public void SetModule(Slot newSlot)
        {
            playerAngar.SetModule(newSlot);
            SaveFile();
        }

        public bool RemoveItem(int guid)
        {
            if (playerStorage.RemoveItem(guid))
            {
                OnStorageUpdate?.Invoke();
                SaveFile();
                return true;
            }
            return false;
        }

        public PlayerStorage.StorageModule AddItem(int moduleID, bool callEvent = true)
        {
            var item = playerStorage.AddToStorage(moduleID);
            if (callEvent)
            {
                OnStorageUpdate?.Invoke();
            }

            SaveFile();

            return item;
        }

        public void UpdateMissions(GameDataObject gameDataObject)
        {
            if (playerMissions.CalculateMissions(gameDataObject))
            {
                SaveFile();
            }
        }

        public void ChangeInventory() => OnStorageUpdate?.Invoke();

        public List<PlayerMissions.Mission> GetMissions() => new(playerMissions.Missions);

        public int GetActiveMissionMap() => playerMissions.GetActiveMission().MapID;

        public void SetActiveMission(int selectedItemGuid)
        {
            playerMissions.SetActiveMission(selectedItemGuid);
            SaveFile();
        }

        public int GetStorageItemsCount(int moduleUid) => playerStorage.ItemsCountOfId(moduleUid);

        public bool BuyItem(GameDataObject.ModuleHolder item)
        {
            if (credits >= item.Module.ModuleCost)
            {
                AddItem(item.ID);
                IncreaseMoney(-item.Module.ModuleCost);
                SaveFile();
                return true;
            }

            return false;
        }

        public PlayerStorage.StorageModule SellItem(GameDataObject.ModuleHolder moduleHolder, GameDataObject gameData)
        {
            var module = playerStorage.RemoveItemByID(moduleHolder.ID);

            if (module != null)
            {
                IncreaseMoney((int)(moduleHolder.Module.ModuleCost * gameData.SellCostMultiplier));
                SaveFile();
            }
            return module;
        }

        public void CompleteCurrentMission() => playerMissions.CompleteCurrentMission();
    }
}
