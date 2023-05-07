using System.Collections.Generic;
using Content.Scripts.Boot;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.SettingsService;
using Content.Scripts.Global;
using Content.Scripts.Menu.PlayerLoader;
using Content.Scripts.Menu.UI;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Menu.UIService
{
    public class MenuUIService : MonoBehaviour
    {
        [SerializeField] private List<UIController<MenuUIService>> items;
        [SerializeField] private WindowsSwitcher windowManager;
        
        private IPlayerLoaderService playerLoaderService;
        private ISettingsService settingsService;
        private GameDataObject gameDataObject;
        private MechChanger mechChanger;
        private IInputService inputService;
        private Loader _globalLoader;


        public IPlayerLoaderService LoaderService => playerLoaderService;
        public ISettingsService SettingsService => settingsService;
        public IInputService InputService => inputService;
        public GameDataObject GameData => gameDataObject;
        public MechChanger MechHolder => mechChanger;

        public Loader GlobalLoader => _globalLoader;
        
        public bool CanMoveCamera => !windowManager.IsFullScreenWindow();


        [Inject]
        public void Contructor(
            IPlayerLoaderService playerLoaderService, 
            IInputService inputService,
            ISettingsService settingsService,
            GameDataObject gameDataObject, 
            MechChanger mechChanger,
            Loader mapLoader
            )
        {
            this.settingsService = settingsService;
            this.playerLoaderService = playerLoaderService;
            this.gameDataObject = gameDataObject;
            this.inputService = inputService;
            this.mechChanger = mechChanger;
            this._globalLoader = mapLoader;
            
            foreach (var item in items)
            {
                item.Init(this);
            }
        }
    }
}
