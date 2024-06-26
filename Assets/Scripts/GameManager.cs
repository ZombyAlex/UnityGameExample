using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static ClientNet net;
    private UIMenu uiMenu;
    private ModeSystem modeSystem;
    private NetMsgSystem netMsgSystem;

    [Inject]
    public void Construct(UIMenu uiMenu, ModeSystem modeSystem, NetMsgSystem netMsgSystem)
    {
        this.uiMenu = uiMenu;
        this.modeSystem = modeSystem;
        this.netMsgSystem = netMsgSystem;
    }

    void Awake()
    {
        Instance = this;
    }
    

    void Update()
    {
        if (net != null)
        {
            net.Update(Time.deltaTime);
        }

        Data.Instance.ServerTime += Time.deltaTime;
    }

    public void Connect(bool isLocal)
    {
        if (net != null)
            return;
        if (isLocal)
            //net = new ClientNet("109.111.178.203", 58888);
            //net = new ClientNet("192.168.0.104", 58888);
            net = new ClientNet("127.0.0.1", 58888);
        else
            net = new ClientNet("127.0.0.1", 58888);
        //net = new ClientNet("188.120.232.19", 58888);

        net.Init(uiMenu, modeSystem, netMsgSystem);
    }

    void OnApplicationQuit()
    {
        Data.Instance.SaveConfig();
        Disconnect();
    }

    private void Disconnect()
    {
        if (net != null)
        {
            Debug.Log("Disconnect");
            net.Disconnect();
            net = null;
        }
    }

    public void LostConnect()
    {
        modeSystem.SetGameMode(false);
        SceneManager.LoadScene("game");
    }

    public void StartLogin()
    {
        Invoke("CheckLoginDone", 10);
    }

    private void CheckLoginDone()
    {
        if (!modeSystem.IsGame)
        {
            Disconnect();
            LostConnect();
        }
    }
}
