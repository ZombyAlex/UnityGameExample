using Lidgren.Network;
using SWFServer.Data;
using SWFServer.Data.Net;
using UnityEngine;


public class ClientNetMsg
{
    public MsgServer data;
    public NetIncomingMessage msg;
}

public class ClientNet
{
	private NetClient client;
	private float timeConnect = 0;

    private bool isReconnect = false;
    private int gamePort;

    private string ip;
    private int serverPort;

    //private bool isConnection = false;

    private UIMenu uiMenu;
    private ModeSystem modeSystem;
    private NetMsgSystem netMsgSystem;

    public void Init(UIMenu uiMenu, ModeSystem modeSystem, NetMsgSystem netMsgSystem)
    {
        this.uiMenu = uiMenu;
		this.modeSystem = modeSystem;
		this.netMsgSystem = netMsgSystem;
    }

    public ClientNet(string ip, int port)
    {
        this.ip = ip;
        this.serverPort = port;
    }
    /*
    public void Run()
    {
        XTimer timer = new XTimer();
        while (!isTerminate)
        {
            float dt = (float)timer.DeltaTime();
            Update(dt);
            Thread.Sleep(20);
        }

        Disconnect();
    }
    */

    public void Reconnect(int port)
    {
        Debug.Log("Reconnect port=" + port);
        isReconnect = true;
        gamePort = port;
        Disconnect();
        Data.Instance.connected = false;
        timeConnect = 0;
    }
	
	public void Update(float dt)
	{
		if (!Data.Instance.connected)
		{
            if (modeSystem.IsGame)
            {
                GameManager.Instance.LostConnect();
            }

            timeConnect -= dt;
			if (timeConnect <= 0)
			{
                Debug.Log("connected...");
				timeConnect = 10.0f;
                if (!isReconnect)
                    Init(ip, serverPort, GameConst.serverName);
                else
                    Init(ip, gamePort, GameConst.serverName);
            }
		}
		UpdateNet();
	}

	private void Init(string inHost, int inPort, string inNameServer)
	{
		NetPeerConfiguration config = new NetPeerConfiguration(inNameServer);
		config.PingInterval = 10;
		//Debug.Log("TIMEOUT " + config.ConnectionTimeout);
		//Debug.Log("PING " + config.PingInterval);
		client = new NetClient(config);

		//client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 

		client.Start();

		NetOutgoingMessage hail = client.CreateMessage();
		hail.Write("Hi");
		var aRes = client.Connect(inHost, inPort, hail);
		Debug.Log("CONNECT RESULT = " + aRes);
        
    }

	private void UpdateNet()
    {
        if (client == null)
            return;
		NetIncomingMessage aMsg;
		while ((aMsg = client.ReadMessage()) != null)
		{
			// handle incoming message
			//Debug.Log("m = " + aMsg.MessageType);
			switch (aMsg.MessageType)
			{
				case NetIncomingMessageType.DebugMessage:
				case NetIncomingMessageType.ErrorMessage:
				case NetIncomingMessageType.WarningMessage:
				case NetIncomingMessageType.VerboseDebugMessage:
					string text = aMsg.ReadString();
					Debug.Log(text);
					//Output(text););
					break;
				case NetIncomingMessageType.StatusChanged:
					NetConnectionStatus status = (NetConnectionStatus) aMsg.ReadByte();

					if (status == NetConnectionStatus.Connected)
					{
						Debug.Log("Connected true");
                        Data.Instance.connected = true;
                        timeConnect = 0;
					}
					else
					{
						Debug.Log("Connected false");
                        Data.Instance.connected = false;
                        timeConnect = 0;
					}
					string reason = aMsg.ReadString();
					if (reason == "Wrong application identifier!")
					{
						//Data.errorVersion = true;
                        uiMenu.ShowWrongVersionClient();
                    }
					//Debug.Log(status.ToString() + ": " + reason);

					break;
				case NetIncomingMessageType.Data:
					ProcessingIncomingMsg(aMsg);
					break;
				default:
					Debug.Log("Unhandled type: " + aMsg.MessageType + " " + aMsg.LengthBytes + " bytes");
					break;
			}
		}
		
		if (Data.Instance.connected)
		{
			if (Data.Instance.netMsg.ToWorkFast())
			{
			    var list = Data.Instance.netMsg.GetWork();

				for (int i = 0; i < list.Count; i++)
				{
					SendMsg((MsgClient) list[i]);
				}
			}
		}
    }

    private void SendMsg(MsgClient msg)
    {
        //Debug.Log("Send msg = " + msg.Type);
		NetOutgoingMessage om = client.CreateMessage();
		msg.Write(om);
		client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
	}

	private void ProcessingIncomingMsg(NetIncomingMessage msg)
	{
        MsgServer m = new MsgServer();
		m.Read(msg);
        //Debug.Log("incoming msg = " + data.Type);

        netMsgSystem.AddMsg(m);
    }

    

    public void Disconnect()
	{
		client.Disconnect("disconnect");
		client.Shutdown("Bye");
        client = null;
    }

}
