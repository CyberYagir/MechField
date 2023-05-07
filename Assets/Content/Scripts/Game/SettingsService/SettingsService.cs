using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Game.SettingsService
{
    public class SettingsService : MonoBehaviour, ISettingsService
    {
        [SerializeField] private GameSettingsObject gameSettingsObject;
        public GameSettingsObject SettingsObject => gameSettingsObject;

        [Inject]
        public void Constructor()
        {
            if (gameSettingsObject != null)
            {
                LoadFile();
            }
            else
            {
                gameSettingsObject = ScriptableObject.CreateInstance<GameSettingsObject>();
                Debug.LogError("Game Settings Empty!!!");
            }
        }
        
        public void LoadFile()
        {
            gameSettingsObject.LoadFile();
        }
        
        public void SaveFile()
        {
            gameSettingsObject.SaveFile();
        }
    }
}
