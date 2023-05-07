using System;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class MechData: IUpdateData
    {
        private float heat;
        private float fuel;
        
        private float maxHeat; 
        private float maxFuel;

        private float coolingModifier;
        private float coolingValue;

        private bool isOverheated;

        private float shootReloadHeat = 0.8f;
        
        private bool enabled = true;

        private float attackModify = 1;


        private IStateService stateService;

        public float Fuel => fuel;
        public float MaxFuel => maxFuel;
        public float Heat => heat;
        public float MaxHeat => maxHeat;
        public bool IsOverheated => isOverheated;
        
        public bool Enabled => enabled;

        public float AttackModify => attackModify;

        public IStateService StateService => stateService;

        public event Action<bool> OnOverheat;
        
        public MechData(MechObject mechObject, MapObject.WorldParameters worldData, float damageModify, IStateService stateService)
        {
            this.stateService = stateService;
            maxFuel = mechObject.MechValues.WorldParameters.NitroValue;
            fuel = maxFuel;
            
            maxHeat = mechObject.MechValues.WorldParameters.HeatValue;
            heat = 0;

            coolingModifier = worldData.CoolingModifier;

            coolingValue = coolingModifier;

            attackModify *= damageModify;
            
            isOverheated = false;
        }

        public void AddFuel(float value)
        {
            fuel += value;
        }
        
        public void AddHeat(float value)
        {
            heat = Mathf.Clamp(Heat + value, 0, maxHeat+1);
            if (heat >= maxHeat)
            {
                if (isOverheated == false)
                {
                    OnOverheat?.Invoke(true);
                }

                isOverheated = true;
                
            }

            if (IsOverheated)
            {
                if (heat <= maxHeat * shootReloadHeat)
                {
                    isOverheated = false;
                    OnOverheat?.Invoke(IsOverheated);
                }
            }

            if (coolingValue <= 0)
            {
                coolingValue = 0;
            }
        }



        public void SetEnabled(bool state) => enabled = state;

        public void Update()
        {
            if (!enabled) return;
            AddHeat(-coolingValue);
            coolingValue = Mathf.Lerp(coolingValue, coolingModifier, Time.deltaTime * 0.5f);
        }
    }
}