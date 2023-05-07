using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PlayerService;
using Content.Scripts.Global;
using Content.Scripts.Menu.PlayerLoader;
using UnityEngine;

namespace Content.Scripts.Game.UI
{
    public class GlobalUIManager : MonoBehaviour
    {
        [SerializeField] private List<GlobalUIWindow> windows;


        private Dictionary<WindowType, GlobalUIWindow> windowDictionary = new Dictionary<WindowType, GlobalUIWindow>(5);
        private IStateService stateService;
        private MapData mapData;
        private PlayerBuilder player;
        private ICreateService createService;
        private GameDataObject gameData;
        private IPlayerLoaderService loaderService;
        public event Action<bool> OnGameEndState;

        public PlayerBuilder Player => player;

        public MapData Map => mapData;

        public GameDataObject GameData => gameData;
        public ICreateService CreateService => createService;

        public IPlayerLoaderService LoaderService => loaderService;


        public void Init(
            IInputService inputService, 
            IStateService stateService, 
            ICreateService createService, 
            IPlayerLoaderService loaderService,
            PlayerBuilder player, 
            MapData mapData,
            GameDataObject gameData
            )
        {
            this.loaderService = loaderService;
            this.gameData = gameData;
            this.createService = createService;
            this.player = player;
            this.stateService = stateService;
            this.mapData = mapData;
            
            foreach (var globalUIWindow in windows)
            {
                globalUIWindow.Init(this);
                globalUIWindow.SetState(false);
                windowDictionary.Add(globalUIWindow.Type, globalUIWindow);
            }
            
            
            player.OnDeath += Lose;
            foreach (var bot in mapData.Bots)
            {
                bot.OnDeath += OnBotDeath;
            }
            
            inputService.OnEscape += OnEscape;

            StartCoroutine(Loop());
        }


        private IEnumerator Loop()
        {
            while (true)
            {
                yield return null;

                foreach (var w in windows)
                {
                    if (w.State)
                    {
                        w.UpdateWindow();
                    }
                }
            }
        }

        public void OnEscape()
        {
            if (stateService.State != GameState.GameEnd)
            {
                ToggleWindow(WindowType.Escape);
                stateService.ChangeState(StateWindow(WindowType.Escape) ? GameState.Pause : GameState.Game);
            }
        }

        private bool StateWindow(WindowType type)
        {
            return windowDictionary[type].State;
        }

        private void OnBotDeath(MechBuilderBase mech)
        {
            if (Map.Bots.FindAll(x => !x.IsDead).Count == 0)
            {
                Win();
            }
        }

        private void Win()
        {
            StopGame();
            OnGameEndState?.Invoke(true);
        }

        private void Lose(MechBuilderBase mechAttacker)
        {
            StopGame();
            OnGameEndState?.Invoke(false);
        }

        private void StopGame()
        {
            windowDictionary[WindowType.Escape].SetState(false);
            stateService.ChangeState(GameState.GameEnd);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        private void ToggleWindow(WindowType type)
        {
            windowDictionary[type].Toggle();
        }
    }
}
