using System.Collections;
using UnityEngine;

namespace Content.Scripts.Menu.UIService
{
    public class UIMechRenderer : UIController<MenuUIService>
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private new Light light;
         public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            camera.cullingMask = LayerMask.GetMask("CustomRenderer");
            StartCoroutine(RenderImagesRuntime());
        }

        IEnumerator RenderImagesRuntime()
        {
            var list = Controller.GameData.GetListMeches();
            for (int i = 0; i < list.Count; i++)
            {
                var builder = Instantiate(list[i].MechObject.Prefab, Vector3.zero, Quaternion.identity);
                builder.LegsAnimator.enabled = false;
                ChangeChildLayer(LayerMask.NameToLayer("CustomRenderer"), builder.transform);

                yield return null;

                var rn = new RenderTexture(512, 512, 32);
                camera.targetTexture = rn;
                camera.transform.position = builder.PreviewPoint.position;
                camera.transform.rotation = builder.PreviewPoint.rotation;
                
                yield return null;
                light.enabled = true;
                camera.Render();
                light.enabled = false;
                Controller.GameData.SetMechIcon(rn, i);
                yield return null;
                Destroy(builder.gameObject);
                yield return null;
            }
        }

        public void ChangeChildLayer(int layer, Transform holder)
        {
            holder.gameObject.layer = layer;
            foreach (Transform t in holder)
            {
                ChangeChildLayer(layer, t);
            }
        }
    }
}
