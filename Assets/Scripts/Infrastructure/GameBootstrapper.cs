using Infrastructure.States;
using Scene;
using UnityEngine;

namespace Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        public LoadingCurtain LoadingCurtainPrefab;
        private Game _game;

        private void Awake()
        {
            _game = new Game(this, Instantiate(LoadingCurtainPrefab));
            _game.GameStateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }
    }
}