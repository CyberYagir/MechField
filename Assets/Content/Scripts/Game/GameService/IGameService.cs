using System;

namespace Content.Scripts.Game.GameService
{
    public interface IGameService
    {
        event Action<GameStates> OnChangeState;
        GameStates CurrentGameState { get; }
        void ChangeState(GameStates newState);
    }
}