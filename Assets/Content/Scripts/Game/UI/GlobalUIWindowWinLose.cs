using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Content.Scripts.Game.UI
{
    public class GlobalUIWindowWinLose : GlobalUIWindow
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private GlobalUIWindowDropItem item;
        [SerializeField] private LocalizedString winLocalized, loseLocalized;
        private bool init;
        public override void Init(GlobalUIManager manager)
        {
            base.Init(manager);
            
            manager.OnGameEndState += CalculatePlayerWinLose;
        }

        private void CalculatePlayerWinLose(bool state)
        {
            if (!this.State)
            {
                SetState(true);

                header.text = state ? winLocalized.GetLocalizedString() : loseLocalized.GetLocalizedString();

                List<float> weights = new List<float>(20);
                var modulesList = Manager.GameData.GetListModules();
                foreach (var module in modulesList)
                {
                    weights.Add(module.DropWeight);
                }

                for (int i = 0; i < (state ? Manager.GameData.RewardDropCount.max : Manager.GameData.RewardDropCount.min); i++)
                {
                    var id = weights.ChooseRandomIndexFromWeights();
                    Instantiate(item, item.transform.parent).Init(modulesList[id]);
                    Manager.LoaderService.PlayerData.AddItem(modulesList[id].ID, false);
                }

                if (state)
                {
                    Manager.LoaderService.PlayerData.CompleteCurrentMission();
                }
                
                Manager.LoaderService.PlayerData.SaveFile();

                item.gameObject.SetActive(false);
            }
        }
    }
}