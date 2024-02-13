using System;
using UnityEngine;

namespace infrastructure.services.inputService
{
    public interface IInputService
    {
        event Action OnSpacePressed;
        Vector2 MousePosition();
    }
}
