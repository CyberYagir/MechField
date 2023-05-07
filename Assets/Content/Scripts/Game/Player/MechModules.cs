using Content.Scripts.Game.Mechs;
using Content.Scripts.Global;
using Content.Scripts.Weapons;

namespace Content.Scripts.Game.Player
{
    public class MechModules : PlayerUtility
    {
        private GameDataObject gameData;
        
        public MechModules(GameDataObject gameData, MechBuilder player)
        {
            this.gameData = gameData;
            ChechAllModules(player);
        }

        
        public override void CheckModule(Slot data)
        {
            base.CheckModule(data);
            if (TryAddArmor(data)) return;
            
        }

        private bool TryAddArmor(Slot slot)
        {
            var moduleObject = ConvertSlotToModule(gameData, slot);
            var part = slot.GetComponentInParent<MechPart>();
            if (moduleObject != null && moduleObject.Have<ModuleArmorAdder>())
            {
                var armor = moduleObject.CreateModule<ModuleArmorAdder>(slot.transform.position, slot.transform.rotation, slot.transform);
                part.AddValues(armor);
                return true;
            }
            else
            {
                part.AddValues(null);
            }

            return false;
        }
    }
}