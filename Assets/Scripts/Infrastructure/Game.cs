using Scene;
using Services;
using Services.Input;

namespace Infrastructure
{
    public class Game
    {
        public static IInputService InputService;
        public readonly GameStateMachine GameStateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain)
        {
            GameStateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), loadingCurtain, AllServices.Container);
        }
    }
}