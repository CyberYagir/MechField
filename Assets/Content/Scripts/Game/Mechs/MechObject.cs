using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.Game.Mechs
{
    [CreateAssetMenu(menuName = "Scriptable/MechObject", fileName = "MechObject", order = 0)]
    public class MechObject : ScriptableObject
    {
        [System.Serializable]
        public class MechData
        {
            
            [System.Serializable]
            public class LegsData
            {
                [SerializeField] private float moveSpeed, rotateSpeed, animationMaxSpeed;
                
                
                public float RotateSpeed => rotateSpeed;
                public float MoveSpeed => moveSpeed;

                public float AnimationMaxSpeed => animationMaxSpeed;
            }
        
            [System.Serializable]
            public class WorldData
            {
                [SerializeField] private float mass, drag, angular;
                [SerializeField] private float nitroValue;
                [SerializeField] private float heatValue;
                [SerializeField] private float flyAcceleration;
                [SerializeField] private float reactorDamageOverheat;
                public float NitroValue => nitroValue;

                public float FlyAcceleration => flyAcceleration;

                public float HeatValue => heatValue;
                public float ReactorDamageOverheat => reactorDamageOverheat;

                public void Configure(Rigidbody rb)
                {
                    rb.angularDrag = angular;
                    rb.drag = drag;
                    rb.mass = mass;
                }
            }
            
            [System.Serializable]
            public class TorsoData
            {
                [SerializeField] private float horizontalSpeed;

                public float HorizontalSpeed => horizontalSpeed;
            }
            
            [SerializeField] private WorldData worldData = new WorldData();
            [SerializeField] private LegsData legsData = new LegsData();
            [SerializeField] private TorsoData torsoData = new TorsoData();
            
            
            public LegsData LegsParameters => legsData;
            public WorldData WorldParameters => worldData;
            public TorsoData TorsoParameters => torsoData;
        }
        
        
        [SerializeField] private string mechName;
        [SerializeField] private int uid;
        [Space]
        [SerializeField] private MechBuilder prefab;
        [SerializeField] private MechData mechData;
        [SerializeField] private List<GameDataObject.ModuleHolder> moduleHolder;



        public string MechName => mechName;
        public int ID => uid;
        public MechBuilder Prefab => prefab;
        public MechData MechValues => mechData;

        public List<GameDataObject.ModuleHolder> ModuleHolder => moduleHolder;
    }
}
