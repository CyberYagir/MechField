using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Menu.UIService
{
    public class UIController<T>:MonoBehaviour
    {
        [SerializeField] private List<UIController<T>> controls = new List<UIController<T>>();

        private T controller;

        public T Controller => controller;

        public virtual void Init(T controller)
        {
            this.controller = controller;
            foreach (var control in controls)
            {
                control.Init(controller);
            }
        }
    }
}