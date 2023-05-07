using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Content.Scripts.Global
{
    public abstract class ModuleObject : ScriptableObject
    {
        public enum ModuleType
        {
            Any, Weapon, Armor
        }

        [SerializeField] private int uid;
        [SerializeField] private string moduleName;
        [SerializeField] private LocalizedString localizedName;
        [SerializeField] private string moduleDesc = "Unknown";
        [SerializeField] private int moduleCost;
        [SerializeField] private GameObject modulePrefab;
        [SerializeField] private ModuleType type;

        public ModuleType Type => type;

        public int UID => uid;

        public string ModuleName => localizedName != null ? localizedName.GetLocalizedString() : moduleName;

        public string ModuleDesc => moduleDesc;

        public int ModuleCost => moduleCost;

        public virtual T CreateModule<T>(Vector3 pos, Quaternion rot, Transform parent)
        {
            if (modulePrefab == null) Debug.LogError(this.name + " modulePrefab is null");
            return Instantiate(modulePrefab, pos, rot, parent).GetComponent<T>();
        }

        public bool Have<T>()
        {
            if (modulePrefab == null)
            {
                return false;
            }
            
            return modulePrefab.GetComponent<T>() != null;
        }

        public void CreateUID()
        {
            uid = Guid.NewGuid().GetHashCode();
        }
    }
}
