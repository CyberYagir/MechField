using Content.Scripts.Global;
using Content.Scripts.Weapons;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechWeaponItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text idText, nameText, bulletsText;
        [SerializeField] private Image back;
        [SerializeField] private Color unactiveColor;
        
        private IWeapon weapon;

        public void Init(IWeapon weapon, int id, GameDataObject gameDataObject)
        {
            this.weapon = weapon;
            weapon.Data.OnChangeData += DataOnOnChangeData;

            idText.text = id.ToString();
            var module = gameDataObject.GetModuleByID(weapon.Data.ModuleID);
            if (module)
            {
                nameText.text =  module.ModuleName.ToSpaceCapLetters();
            }
            else
            {
                nameText.text = "<color=red>Error " + weapon.Data.ModuleID;
            }

            if (float.IsPositiveInfinity(weapon.Data.BulletsCount))
            {
                bulletsText.text = "∞";
            }

            DataOnOnChangeData();
        }

        private void DataOnOnChangeData()
        {
            if (!weapon.Data.Part.IsWorking)
            {
                idText.text = "X";
                idText.SetAlpha(0.5f);
                nameText.SetAlpha(0.5f);
                bulletsText.SetAlpha(0.5f);
                back.DOColor(unactiveColor, 0.2f);

                weapon.Data.OnChangeData -= DataOnOnChangeData;
                return;
            }
            
            if (!float.IsPositiveInfinity(weapon.Data.BulletsCount))
            {
                bulletsText.text = weapon.Data.BulletsCount.ToString();
            }
        }
    }
}
