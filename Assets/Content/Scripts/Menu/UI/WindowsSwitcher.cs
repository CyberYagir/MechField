using System.Collections.Generic;
using Content.Scripts.Menu.UIService;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Scripts.Menu.UI
{
    public class WindowsSwitcher : UIController<MenuUIService>
    {
        [System.Serializable]
        public class Window
        {
            [SerializeField] private bool isOpened;
            [SerializeField] private bool isFullscreen;
            [SerializeField] private Canvas canvas;
            [SerializeField] private UnityEvent OnOpen, OnClose;

            public bool IsFullscreen => isFullscreen;


            public void OpenClose(bool state)
            {
                if (state != isOpened)
                {
                    if (state)
                    {
                        OnOpen.Invoke();
                    }
                    else
                    {
                        OnClose.Invoke();
                    }

                    isOpened = state;
                    canvas.enabled = isOpened;
                }
            }

            public void Disable()
            {
                canvas.enabled = false;
            }
        }

        [SerializeField] private List<Window> windows = new List<Window>();

        private Window currentWindow;

        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            foreach (var window in windows)
            {
                window.Disable();
            }

            controller.GlobalLoader.OnLoadStart += HideAllWindows;
            
            controller.InputService.OnEscape += OnEscape;
            OpenWindow(0);
        }

        private void HideAllWindows()
        {
            OpenWindow(-1);
        }

        private void OnEscape()
        {
            OpenWindow(0);
        }

        public void OpenWindow(int id)
        {
            if (currentWindow != null)
            {
                currentWindow.OpenClose(false);
            }
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].OpenClose(i == id);
                if (i == id) currentWindow = windows[i];
            }
        }

        public bool IsFullScreenWindow()
        {
            if (currentWindow == null) return false;
            
            return currentWindow.IsFullscreen;
        }
    }
}
