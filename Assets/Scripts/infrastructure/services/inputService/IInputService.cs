using System;
using UnityEngine;

namespace infrastructure.services.inputService
{
    public interface IInputService
    {
        event Action OnSpacePressed;
        event Action OnPPressed;
        Vector2 MousePosition();
        Vector2 GetMouseDelta();
        Vector2 GetMouseScroll();
        bool IsMouseButtonHeld();
    }
}
