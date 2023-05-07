using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Content.Scripts.Menu.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizeTextAuto : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<LocalizeStringEvent>()?.OnUpdateString.AddListener(delegate(string text)
            {
                GetComponent<TMP_Text>().text = text;
            });
        }
    }
}
