using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Global;
using Content.Scripts.Weapons;

namespace Content.Scripts.Game.Player
{
    public class PlayerWeapons : MeshWeapons, IUpdateData
    {
        private bool enabled = true;
        
        public bool Enabled => enabled;

        public PlayerWeapons(GameDataObject gameData, IPoolService pool, IInputService inputService, PlayerBuilder player, IStateService stateService) : base(gameData,player, pool, stateService)
        {
            inputService.OnFireStart += ShootStart;
            inputService.OnFireEnd += ShootEnd;
            inputService.OnFire += ShootProcess;
        }
        
        protected override void WeaponInit(IWeapon weapon, WeaponSlot weaponSlot)
        {
            var player = mech as PlayerBuilder;
            weapon.Init(player.MainCamera, pool, player.Data, true, weaponSlot.ModuleID);
            weaponSlot.SetWeapon(weapon);
        }



       
        public void SetEnabled(bool state) => enabled = state;

        public void Update()
        {
            if (!enabled) return;
            
            foreach (var weapon in Weapons)
            {
                weapon?.UpdateState();
            }
        }
        
    }
}