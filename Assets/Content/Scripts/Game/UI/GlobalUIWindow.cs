    using UnityEngine;

namespace Content.Scripts.Game.UI
{
    public enum WindowType
    {
        WinLose,
        Escape,
        Overheating
    }
    public class GlobalUIWindow: MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private WindowType type;
        private GlobalUIManager manager;

        public GlobalUIManager Manager => manager;

        public WindowType Type => type;
        public bool State => canvas.enabled;

        public virtual void Init(GlobalUIManager manager)
        {
            this.manager = manager;
            
            
        }

        public virtual void Toggle()
        {
            SetState(!canvas.enabled);
        }

        public virtual void SetState(bool state)
        {
            canvas.enabled = state;
        }

        public virtual void UpdateWindow()
        {
            
        }
    }
}