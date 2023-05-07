using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Content.Scripts.Game.InputService
{


    public class InputService : MonoBehaviour, IInputService
    {

        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnMouse;
        public event Action<Vector2> OnMousePosChange;

        public event Action<int> OnFire;
        public event Action<int> OnFireStart;
        public event Action<int> OnFireEnd;


        public event Action OnJump;
        public event Action OnJumpStart;
        public event Action OnJumpEnd;
        
        public event Action OnEscape;

        
        
        private bool isFly;
        private ControlsData data;
        private List<int> pressedKeys = new List<int>(5);
        private List<InputAction> fireKeys = new List<InputAction>(5);

        
        [Inject]
        public void Constructor()
        {
            data = new ControlsData();
            
            InitFireKeys();

            data.BasePC.Jump.started += (a) =>
            {
                OnJumpStart?.Invoke();
                isFly = true;
            };
            
            data.BasePC.Jump.canceled += (a) =>
            {
                OnJumpEnd?.Invoke();
                isFly = false;
            };
            
            
            data.BasePC.Pause.performed += (a) => OnEscape?.Invoke();
        }

        
        private void InitFireKeys()
        {
            fireKeys.Add(data.BasePC.Fire);
            fireKeys.Add(data.BasePC.Fire1);
            fireKeys.Add(data.BasePC.Fire2);
            fireKeys.Add(data.BasePC.Fire3);
            

            for (int i = 0; i < fireKeys.Count; i++)
            {
                var id = i;
                fireKeys[i].started += (a) =>
                {
                    pressedKeys.Add(id);
                    OnFireStart?.Invoke(id);
                };
                fireKeys[i].canceled += (a) =>
                {
                    pressedKeys.Remove(id);
                    OnFireEnd?.Invoke(id);
                };
            }
            
        }
        private void Update()
        {
            foreach (var number in pressedKeys)
            {
                OnFire?.Invoke(number);
            }

            if (isFly)
            {
                OnJump?.Invoke();
            }


            OnMousePosChange?.Invoke(data.BasePC.MousePoint.ReadValue<Vector2>());
            OnMove?.Invoke(data.BasePC.Moving.ReadValue<Vector2>());
            OnMouse?.Invoke(data.BasePC.Mouse.ReadValue<Vector2>());
        }
        private void OnEnable()
        {
            data?.Enable();
        }
        private void OnDisable()
        {
            data?.Disable();
        }
    }
}
