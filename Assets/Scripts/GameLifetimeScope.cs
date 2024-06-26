using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameConfig config;
    [SerializeField] private SceneData sceneData;
    [SerializeField] private Data data;
    [SerializeField] private UIMenu uiMenu;
    [SerializeField] private UIGame uiGame;
    [SerializeField] private PanelLogin panelLogin;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UIInfo uiInfo;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(uiMenu);
        builder.RegisterComponent(uiGame);
        builder.RegisterComponent(panelLogin);
        builder.RegisterComponent(uiManager);
        builder.RegisterComponent(uiInfo);


        builder.RegisterComponent(data);
        builder.RegisterInstance(config);
        builder.RegisterInstance(config.Content);
        builder.RegisterInstance(sceneData);
        
        builder.RegisterEntryPoint<MapSystem>().AsSelf();

        builder.Register<UnitFactory>(Lifetime.Singleton).AsSelf();

        builder.RegisterEntryPoint<UnitSystem>().AsSelf();
        
        builder.RegisterEntryPoint<MapPosSystem>().AsSelf();
        builder.RegisterEntryPoint<TaskTargetSystem>().AsSelf();
        builder.RegisterEntryPoint<PlayerController>().AsSelf();
        builder.RegisterEntryPoint<BuildSystem>().AsSelf();


        builder.RegisterEntryPoint<NetMsgSystem>().AsSelf();


        builder.RegisterEntryPoint<ModeSystem>().AsSelf();

        builder.RegisterEntryPoint<GameCursorSystem>().AsSelf();
    }

}
