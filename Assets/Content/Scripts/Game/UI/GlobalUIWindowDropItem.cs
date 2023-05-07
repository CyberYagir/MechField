using Content.Scripts.Global;
using Content.Scripts.Weapons;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Game.UI
{
    public class GlobalUIWindowDropItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text nameText;

        public void Init(GameDataObject.ModuleHolder moduleHolder)
        {
            typeText.text = Slot.TypeToString(moduleHolder.Module.Type);
            nameText.text = moduleHolder.Module.ModuleName;
        }
    } 
}
