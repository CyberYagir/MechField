using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/GameSettings", fileName = "Game Settings", order = 0)]
    public class GameSettingsObject : SavedObject
    {
        [SerializeField] private Vector2 mouseSensitivity = new Vector2(1, 5);
        [SerializeField] private int qualityLevel = 0;
        [SerializeField] private int localization = 0;


        public Vector2 MouseSensitivity => mouseSensitivity;

        public int QualityLevel => qualityLevel;

        public int Localization => localization;

        protected override string GetFilePath() => GetPathFolder() + @"\options.dat";


        public override void LoadFile()
        {
            base.LoadFile();

            SetQuality(QualityLevel);
            SetLocalization(Localization);
        }


        public void SetMouseX(float value)
        {
            mouseSensitivity.x = value;
        }

        public void SetMouseY(float value)
        {
            mouseSensitivity.y = value;
        }

        public void SetQuality(int value)
        {
            QualitySettings.SetQualityLevel(value);
            qualityLevel = value;
        }

        public async void SetLocalization(int value)
        {
            localization = value;
            if (LocalizationSettings.InitializationOperation.IsDone)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
            }
            else
            {
                LocalizationSettings.InitializationOperation.Completed += delegate { LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value]; };
            }
        }
    }
}
