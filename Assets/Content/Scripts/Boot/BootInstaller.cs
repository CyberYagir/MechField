using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Boot
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private Loader loader;

        [Inject]
        public void Constructor(GameDataObject gameData)
        {
            loader.Load(gameData);
        }

        public override void InstallBindings()
        {
            // base.InstallBindings();
        }
    }
}
