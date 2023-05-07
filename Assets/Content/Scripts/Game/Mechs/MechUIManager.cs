using System.Collections.Generic;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Player;
using Content.Scripts.Global;
using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs
{
    public class MechUIManager : MonoBehaviour
    {
        [SerializeField] private List<UIController<MechUIManager>> items;
        private PlayerBuilder builder;
        private IInputService inputService;
        private GameDataObject gameData;

        public IInputService InputService => inputService;

        public MechBuilder MechBuilder => Player.MechBuilder;

        public PlayerBuilder Player => builder;
        public GameDataObject GameData => gameData;

        public void Init(PlayerBuilder builder, IInputService inputService, GameDataObject gameDataObject)
        {
            gameData = gameDataObject;
            this.inputService = inputService;
            this.builder = builder;
            foreach (var it in items)
            {
                it.Init(this);
            }
        }
    }
}
