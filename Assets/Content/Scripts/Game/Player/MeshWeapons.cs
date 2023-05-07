using System.Collections.Generic;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Global;
using Content.Scripts.Weapons;

namespace Content.Scripts.Game.Player
{
    public class MeshWeapons : PlayerUtility
    {
        private List<IWeapon> weapons = new List<IWeapon>(5);
        private GameDataObject gameData;
        protected IPoolService pool;
        protected MechBuilderBase mech;
        private IStateService stateService;

        public List<IWeapon> Weapons => weapons;
        
        public MeshWeapons(GameDataObject gameData, MechBuilderBase mech, IPoolService pool, IStateService stateService)
        {
            this.stateService = stateService;
            this.pool = pool;
            this.gameData = gameData;
            this.mech = mech;
            
            ChechAllModules(mech.MechBuilder);
        }

        public MeshWeapons(GameDataObject gameData, MechBuilder builder)
        {
            this.gameData = gameData;
            ChechAllModules(builder);
        }


        public override void CheckModule(Slot data)
        {
            base.CheckModule(data);
            
            if (TryAddWeapon(data)) return;
        }


        protected bool TryAddWeapon(Slot slot)
        {
            var moduleObject = ConvertSlotToModule(gameData, slot);
            if (moduleObject != null && moduleObject.Have<IWeapon>())
            {
                var weapon = moduleObject.CreateModule<IWeapon>(slot.transform.position, slot.transform.rotation, slot.transform);
                var weaponSlot = (slot as WeaponSlot);
                if (weaponSlot != null)
                {
                    WeaponInit(weapon, weaponSlot);
                    weapons.Add(weapon);
                    return true;
                }
            }

            return false;
        }
        
        protected virtual void WeaponInit(IWeapon weapon, WeaponSlot weaponSlot)
        {
            
        }
        
        
        public void ShootProcess(int weaponID)
        {
            if (!stateService.IsCanPlayerPlay)
            {
                ShootEnd(weaponID);
                return;
            }
            for (var i = 0; i < Weapons.Count; i++)
            {
                if (i == weaponID)
                {
                    Weapons[i]?.Shoot();
                }
            }
        }

        public void ShootStart(int weaponID)
        {
            if (!stateService.IsCanPlayerPlay) return;
            for (var i = 0; i < Weapons.Count; i++)
            {
                if (i == weaponID)
                {
                    Weapons[i]?.ShootStart();
                }
            }
        }

        public void ShootEnd(int weaponID)
        {
            for (var i = 0; i < Weapons.Count; i++)
            {
                if (i == weaponID)
                {
                    Weapons[i]?.ShootEnd();
                }
            }
        }
    }
}