using UnityEngine;

namespace Content.Scripts.Global
{

    public class ModuleArmorAdder : MonoBehaviour
    {
        [SerializeField] private float health, armor;

        public float Armor => armor;

        public float Health => health;
    }
}
