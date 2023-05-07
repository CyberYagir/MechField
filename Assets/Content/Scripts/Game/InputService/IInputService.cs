using System;
using UnityEngine;

namespace Content.Scripts.Game.InputService
{
    public interface IInputService
    {
        event Action<Vector2> OnMove;
        event Action<Vector2> OnMouse;
        event Action<Vector2> OnMousePosChange;
        event Action<int> OnFire;
        event Action<int> OnFireStart;
        event Action<int> OnFireEnd;


        event Action OnJump;
        event Action OnJumpStart;
        event Action OnJumpEnd;
        
        event Action OnEscape;
    }
}