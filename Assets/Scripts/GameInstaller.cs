using UnityEngine;

/*

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig config;
    //[SerializeField] private GameContent content;
    [SerializeField] private SceneData sceneData;

    public override void InstallBindings()
    {
       
        //Container.Bind<CustomUnitFactory>().AsSingle();

        Container.BindInstance(config).AsSingle();
        //Container.BindInstance(content).AsSingle();

        


        Container.BindInterfacesAndSelfTo<UnitSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<MapSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<MapPosSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<TaskTargetSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();
        Container.BindInterfacesAndSelfTo<BuildSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<NetMsgSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<ModeSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameCursorSystem>().AsSingle();


        
        

        Container.Bind<Transform>().WithId("UnitsParent").FromInstance(sceneData.unitsParentTransform);


        Container.BindFactory<string, UnitController, CustomUnitFactory>()
            .FromFactory<CustomUnitFactory>()
            .NonLazy(); // Использование AsSingle вместо AsCached

        Container.Bind<IPrefabProvider>().WithId("Unit").To<UnitPrefabProvider>().AsSingle();
        Container.Bind<IPrefabProvider>().WithId("Citizen").To<CitizenPrefabProvider>().AsSingle();


        Container.BindFactory<UIMapTarget, UIMapTarget.Factory>()
            .FromComponentInNewPrefab(config.PrefabMapTarget)
            .WithGameObjectName("MapTarget")
            .UnderTransform(sceneData.RootMapTarget);

        //Container.BindFactory<Enemy, Enemy.Factory>().FromComponentInNewPrefab(unitContentConfig.prefabUnit);

    }
}
*/