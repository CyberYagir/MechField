using System.IO;
using System.Text;
using UnityEngine;

namespace Content.Scripts.Global
{
    public class SavedObject : ScriptableObject
    {
        protected string GetPathFolder() => Directory.GetParent(Application.dataPath)?.FullName;
        protected virtual string GetFilePath() => GetPathFolder() + @"\options.dat";
        
        public void SaveFile()
        {
            Debug.LogError("Save");
            if (!string.IsNullOrEmpty(GetPathFolder()))
            {
                var json = JsonUtility.ToJson(this);
                File.WriteAllText(GetFilePath(), json, Encoding.Unicode);
            }
            else
            {
                Debug.LogError("Path error!");
            }
        }

        public virtual void LoadFile()
        {
            var file = GetFilePath();
            if (File.Exists(file))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(file), this);
            }
            else
            {
                SaveFile();
            }
        }
        
        public void DeleteFile()
        {
            var file = GetFilePath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}