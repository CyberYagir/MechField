using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechPartsItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText, healthText, armorText;
        
        [SerializeField] private Image back;
        [SerializeField] private Color unactiveColor;
        [SerializeField] private Color lowDamageColor;
        [SerializeField] private Color middleDamageColor;
        [SerializeField] private Color highDamageColor;
        private MechPart part;

        public void Init(MechPart part, GameDataObject gameData)
        {
            this.part = part;
            
            var find = gameData.MechPartsNames.Find(x => x.PartType == part.Type);
            if (find != null)
            {
                nameText.text = find.LocalizedString.GetLocalizedString();
            }
            else
            {
                nameText.text = part.Type.ToString().ToSpaceCapLetters();
            }
            
            
            part.OnTakeDamage += PartOnOnTakeDamage;
        }

        private void PartOnOnTakeDamage(float health)
        {
            if (part.IsWorking && health > 0)
            {
                healthText.text = (part.HealthPercent * 100).ToString("F0") + "%";
                armorText.text = (part.ArmorPercent * 100).ToString("F0") + "%";
            }
            else
            {
                healthText.text = "0%";
                armorText.text = "0%";
                part.OnTakeDamage -= PartOnOnTakeDamage;
                back.DOColor(unactiveColor, 0.2f);
                return;
            }
            
            
            if (part.HealthPercent < 0.35f)
            {
                back.DOColor(highDamageColor, 0.2f);
                return;
            }
            if (part.HealthPercent < 0.7f)
            {
                back.DOColor(middleDamageColor, 0.2f);
                return;
            }
            if (part.HealthPercent < 1f)
            {
                back.DOColor(lowDamageColor, 0.2f);
                return;
            }
        }
    }
}
