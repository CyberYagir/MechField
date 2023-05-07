using Content.Scripts.Game.GameService;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.PlayerService;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Game.SettingsService;
using Content.Scripts.Menu.PlayerLoader;
using UnityEngine;

namespace Content.Scripts.Game
{
    public sealed class GameInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<ISettingsService>();
            BindService<IStateService>();
            BindService<IPlayerLoaderService>();
            BindService<IGameService>();
            BindService<IInputService>();
            BindService<IPoolService>();
            BindService<ICreateService>();
        }

        public override void Start()
        {
            base.Start();
            Application.targetFrameRate = 60;
        }
    }
}
