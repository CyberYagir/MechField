using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UI
{
    public class ExitButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                Application.Quit();
            });
        }
    }
}
