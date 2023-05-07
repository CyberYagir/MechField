using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/Names List", fileName = "NamesListObject", order = 0)]
    public class NamesListObject : ScriptableObject
    {
        [SerializeField] private List<string> names;
        [SerializeField] private TextAsset source;


        public void LoadFromFile()
        {
            names = source.text.Split("/").ToList();
        }

        public string GetRandomName()
        {
            return names.GetRandomItem();
        }
    }
}
