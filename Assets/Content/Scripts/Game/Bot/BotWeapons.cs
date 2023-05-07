using System.Collections.Generic;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Global;
using Content.Scripts.Weapons;
using UnityEngine;

namespace Content.Scripts.Game.Bot
{
    public class BotWeapons : MeshWeapons, IUpdateData
    {
        public enum BotShootType
        {
            Minimum = 3, Normal = 2, Hard = 1
        }

        private BotShootType type;
        private Transform target;
        private BotBuilder botBuilder;

        private bool enabled = true;
        private bool isShoot;

        private List<int> shootedWeapons = new List<int>(5);

        private List<IWeapon> activeWeapons;


        public bool Enabled => enabled;

        public BotWeapons(GameDataObject gameData, BotBuilder mech, IPoolService pool, IStateService stateService) : base(gameData, mech, pool, stateService)
        {
            botBuilder = mech;
            botBuilder.OnChangeTarget += SetTarget;


            activeWeapons = new(Weapons);
            
            SetBotShootType();
            
            
            foreach (var wp in Weapons)
            {
                wp.Data.SetLayerMask(botBuilder.BotOptions.WeaponsLayerMask);
                wp.Transform.ChangeLayerWithChilds(LayerMask.NameToLayer("Enemy"));
                wp.Data.Part.OnBreak += OnPartBreak;
            }
        }

        private void OnPartBreak(MechPart part)
        {
            var weapon = Weapons.Find(x => x.Data.Part == part);
            weapon.ShootEnd();
        }

        private void SetBotShootType()
        {
            activeWeapons.RemoveAll(x => !x.Transform.GetComponentInParent<MechPart>().IsWorking);
            
            if (activeWeapons.Count >= 3)
            {
                type = Extensions.GetRandomEnum<BotShootType>();
            }
            else
            {
                type = BotShootType.Hard;
            }
        }

        private void SetTarget(Transform target)
        {
            this.target = target;
        }

        protected override void WeaponInit(IWeapon weapon, WeaponSlot weaponSlot)
        {
            var bot = (mech as BotBuilder);
            weapon.Init(bot.MechBuilder.CabineCamera, pool, bot.Data, false, weaponSlot.ModuleID);
            weaponSlot.SetWeapon(weapon);
        }
        
        
        public void AllWeaponsShoot()
        {
            for (int i = 0; i < Mathf.RoundToInt(activeWeapons.Count/(float)type); i++)
            {
                if (!shootedWeapons.Contains(i))
                {
                    activeWeapons[i].ShootStart();
                    shootedWeapons.Add(i);
                }
                Weapons[i].Shoot();
            }
            isShoot = true;
        }

        public void AllWeaponsStop()
        {
            for (int i = 0; i < activeWeapons.Count; i++)
            {
                activeWeapons[i].ShootEnd();
            }
            shootedWeapons.Clear();
            isShoot = false;
        }

        public void RandomWeaponsState()
        {
            SetBotShootType();
        }

        public void SetEnabled(bool state) => enabled = state;

        public void Update()
        {
            if (isShoot)
            {
                for (int i = 0; i < activeWeapons.Count; i++)
                {
                    activeWeapons[i].UpdateState();
                }
            }
        }
    }
}