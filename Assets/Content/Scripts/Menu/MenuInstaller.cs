using Content.Scripts.Boot;
using Content.Scripts.Game;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.SettingsService;
using Content.Scripts.Menu.PlayerLoader;
using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Menu
{
    public class MenuInstaller : MonoBinder
    {
        [SerializeField] private MechChanger mechChanger;
        [SerializeField] private Loader loader;
        public override void InstallBindings()
        {
            
            BindService<ISettingsService>();
            BindService<IInputService>();
            BindService<IPlayerLoaderService>();
            BindService<Loader>();
            BindService<MenuUIService>();
            Container.Bind<MechChanger>().FromInstance(mechChanger);
        }

        public override void Start()
        {
            base.Start();
            Application.targetFrameRate = 60;
        }
    }
}
