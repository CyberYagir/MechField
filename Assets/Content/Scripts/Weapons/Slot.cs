using UnityEngine;
using static Content.Scripts.Global.ModuleObject;

namespace Content.Scripts.Weapons
{
    public abstract class Slot : MonoBehaviour
    {
        [SerializeField] private int slotID;
        [SerializeField] private int moduleID = -1;
        [SerializeField] private ModuleType type = ModuleType.Any;
        public int SlotID => slotID;
        public int ModuleID => moduleID;

        public ModuleType Type => type;


        public void SetID(int i)
        {
            slotID = i;
        }

        public bool SetModuleID(int moduleID, ModuleType moduleType)
        {
            if (CanPlaceItem(moduleType))
            {
                this.moduleID = moduleID;
                return true;
            }

            return false;
        }

        public bool CanPlaceItem(ModuleType moduleType)
        {
            if (moduleType == ModuleType.Any || Type == ModuleType.Any)
            {
                return true;
            }
            if (moduleType == Type)
            {
                return true;
            }

            return false;
        }

        public void SetType(ModuleType type)
        {
            this.type = type;
        }


        public static string TypeToString(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.Any:
                    return "-";
                case ModuleType.Weapon:
                    return "W";
                case ModuleType.Armor:
                    return "A";
                default:
                    return "?";
            }
        }
    }
}