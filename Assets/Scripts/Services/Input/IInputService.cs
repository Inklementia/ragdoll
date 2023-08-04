using UnityEngine;

namespace Services.Input
{
    public interface IInputService : IService
    {
        bool IsMouseButtonDown();
        
        bool IsMouseButtonUp();
    }
}