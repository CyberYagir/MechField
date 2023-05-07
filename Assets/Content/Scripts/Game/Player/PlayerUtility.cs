using System.Linq;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Global;
using Content.Scripts.Weapons;

namespace Content.Scripts.Game.Player
{
    public class PlayerUtility
    {
        public void ChechAllModules(MechBuilder player)
        {
            var slots = player.ModuleSlots.OrderBy(x=>x.SlotID).Reverse().ToList();
            
            for (var i = 0; i < slots.Count; i++)
            {
                CheckModule(slots[i]);
            }
        }

        public ModuleObject ConvertSlotToModule(GameDataObject gameData, Slot slot)
        {
            if (slot.ModuleID != -1)
            {
                var moduleObject = gameData.GetModuleByID(slot.ModuleID);
                return moduleObject;
            }

            return null;
        }

        public virtual void CheckModule(Slot data)
        {
            
        }
    }
}