using System;

namespace infrastructure.services.updateService
{
    public interface IUpdateService
    {
        event Action OnUpdate;
    }
}