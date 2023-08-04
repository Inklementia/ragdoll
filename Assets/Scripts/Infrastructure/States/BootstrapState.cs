using Infrastructure.AssetManagement;
using Scene;
using Services;
using Services.Input;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, AllServices services )
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _services = services;
            RegisterServices();
        }

        public void Enter()
        {
         
          _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel()
        {
            _gameStateMachine.Enter<LoadLevelState, string>("Main");
        }

        public void Exit()
        {
        }

        private void RegisterServices()
        {
            _services.RegisterSingle<IInputService>(new InputService());
            _services.RegisterSingle<IAssets>(new AssetProvider());
         
        }
    }
    
}