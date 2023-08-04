using Scene;
using UnityEngine;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
       
        private const string InitialPoint = "InitialPoint";
        
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
          _loadingCurtain.Hide();
        }

        private void OnLoaded()
        {
            _gameStateMachine.Enter<GameLoopState>();
        }

      
    }
}