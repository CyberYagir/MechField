using System.Linq;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Menu.PlayerLoader
{
    public class PlayerLoaderService : MonoBehaviour, IPlayerLoaderService
    {
        [SerializeField] private PlayerDataObject playerData;

        public PlayerDataObject PlayerData => playerData;

        [Inject]
        public void Constructor(GameDataObject gameDataObject)
        {
            playerData = ScriptableObject.CreateInstance<PlayerDataObject>();

            if (PlayerData.HaveSave())
            {
                PlayerData.LoadFile();
                if (playerData.GetMechID() == -1)
                {
                    CreateSave(gameDataObject);
                }
            }
            else
            {
                CreateSave(gameDataObject);
            }

            PlayerData.UpdateMissions(gameDataObject);
        }

        private void CreateSave(GameDataObject gameDataObject)
        {
            var firstMech = gameDataObject.GetListMeches().First().MechObject;
            playerData.AngarAddMech(firstMech);
            PlayerData.SetMech(firstMech);
            PlayerData.SaveFile();
        }
    }
}
