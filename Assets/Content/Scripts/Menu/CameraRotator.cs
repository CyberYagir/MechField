using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Menu
{
    public class CameraRotator : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform rotator;
        [SerializeField] private float rotateSpeed;


        private MenuUIService menuUIService;
        private Vector3 mousePos;
        private bool moved;
        private float cameraAngle = 0;
        private bool isDown;

        public void Init(IInputService input, MenuUIService menuUIService)
        {
            this.menuUIService = menuUIService;
            
            input.OnFireStart += RotateStart;
            input.OnFireEnd += RotateEnd;
            input.OnMouse += MoveCamera;
            input.OnMousePosChange += ChangeMousePos;
            
            
        }

        private void ChangeMousePos(Vector2 pos)
        {
            mousePos = pos;
        }

        private void MoveCamera(Vector2 delta)
        {
            if (isDown && menuUIService.CanMoveCamera)
            {
                cameraAngle += delta.x * rotateSpeed * Time.deltaTime;
                cameraAngle = Mathf.Clamp(cameraAngle, 0, 20);
                rotator.localEulerAngles = new Vector3(0, cameraAngle, 0);
            }
        }
        private void RotateEnd(int keyID)
        {
            if (keyID == 0)
            {
                isDown = false;
            }
        }

        private void RotateStart(int keyID)
        {
            if (keyID == 0)
            {
                
                Ray ray = camera.ScreenPointToRay(mousePos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponentInParent<MechBuilder>())
                    {
                        isDown = true;
                    }
                }
            }
        }

    }
}
