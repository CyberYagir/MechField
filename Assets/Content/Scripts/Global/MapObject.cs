using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/Map Object", fileName = "Map Object", order = 0)]
    public class MapObject : ScriptableObject
    {
        [System.Serializable]
        public class WorldParameters
        {
            [SerializeField] private float gravity = -9.81f;
            [SerializeField] private float coolingModifier = 5f;

            public float CoolingModifier => coolingModifier;

            public void SetGravity()
            {
                Physics.gravity = new Vector3(0, gravity, 0);
            }
        }
        
        [SerializeField] private string mapName;
        [SerializeField] private LocalizedString mapNameLocalized;
        [SerializeField] private int mapID;
        [SerializeField] private LocalizedString mapDescription;
        [SerializeField] private Sprite mapImage;
        [SerializeField] private AssetReference mapPrefab;
        [SerializeField] private WorldParameters parameters;
        public int MapID => mapID;
        public Sprite MapImage => mapImage;
        public string MapDescription => mapDescription.GetLocalizedString();
        public string MapName => mapNameLocalized != null ? mapNameLocalized.GetLocalizedString() : mapName;
        public AssetReference MapPrefab => mapPrefab;

        public WorldParameters Parameters => parameters;
    }
}
