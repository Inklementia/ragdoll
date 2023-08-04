
namespace Services.Input
{
    public class InputService : IInputService
    {
        public bool IsMouseButtonDown() => UnityEngine.Input.GetMouseButtonDown(0);
        
        public bool IsMouseButtonUp() => UnityEngine.Input.GetMouseButtonUp(0);
    }
}

