using System.Diagnostics;
using Content.Scripts.Menu.UIService;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Content.Scripts.Menu.UI
{
    public class VersionText : UIController<MenuUIService>, IPointerClickHandler
    {
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            GetComponent<TMP_Text>().text = Application.version;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            print("click");
            
            string fullPath = @"C:\Windows\System32\dxdiag.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "";

            Process.Start(startInfo);
        }
    }
}
