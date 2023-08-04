using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Infrastructure;
using Services;
using Infrastructure.States;

public class SceneSwitcher : MonoBehaviour
{
    
    private IGameStateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = AllServices.Container.Single<IGameStateMachine>();
    }

    public void RestartScene()
    {
        
        Destroy( GameObject.FindObjectOfType<GameBootstrapper>());
        SceneManager.LoadScene("Initial");
    }

}
