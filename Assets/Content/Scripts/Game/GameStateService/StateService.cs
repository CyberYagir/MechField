using System;
using UnityEngine;

namespace Content.Scripts.Game.GameStateService
{
    public enum GameState
    {
        Game, Pause, GameEnd
    }

    public interface IStateService
    {
        public GameState State { get; }
        public bool IsCanPlay { get; }
        public bool IsCanPlayerPlay { get; }
        
        event Action<GameState> OnChangeState;
        void ChangeState(GameState state);
    }

    public class StateService : MonoBehaviour, IStateService
    {
        [SerializeField] private GameState state;

        public bool IsCanPlay => state is GameState.Game or GameState.GameEnd;
        public bool IsCanPlayerPlay => state is GameState.Game;
        
        public event Action<GameState> OnChangeState;

        public GameState State => state;

        public void ChangeState(GameState state)
        {
            print($"Change global state to: <color=red>{state.ToString()}</color>");
            this.state = state;
            OnChangeState?.Invoke(state);
        }
    }
}
