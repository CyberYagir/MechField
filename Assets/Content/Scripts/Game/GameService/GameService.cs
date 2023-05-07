using System;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Game.GameService
{
    public class GameService : MonoBehaviour, IGameService
    {
        public event Action<GameStates> OnChangeState;
        public GameStates CurrentGameState => gameState;

        private GameStates gameState = GameStates.Game;

        [Inject]
        public void Constructor()
        {
            ChangeState(GameStates.Game);
        }
        
        
        public void ChangeState(GameStates newState)
        {
            gameState = newState;
            OnChangeState?.Invoke(newState);
        }
    }
}
