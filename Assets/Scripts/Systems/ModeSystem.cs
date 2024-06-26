using VContainer.Unity;

public enum GameMode
{
    normal,
    battle
}

public class ModeSystem: IInitializable
{
    public bool IsGame { get; private set; } = false;
    public GameMode Mode { get; private set; } = GameMode.normal;


    private UIManager uiManager;

    public ModeSystem(UIManager uiManager)
    {
        this.uiManager = uiManager;
    }

    public void Initialize()
    {
        
    }

    public void SetGameMode(bool isGame)
    {
        IsGame = isGame;
        uiManager.SetGameMode(IsGame);
    }
}
