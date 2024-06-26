using System;
using System.Collections.Generic;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMsg
{
    public string name;
    public string text;
}

public class ChatChannel
{
    public bool isChannel;
    public uint id;
    public List<ChatMsg> items = new List<ChatMsg>();
    public GameObject item;
    public UIText textCount;
    public int count;
}

public class PanelChat : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject prefabUser;
    [SerializeField] private Transform rootUsers;
    [SerializeField] private ScrollRect chatScroll;
    [SerializeField] private GameObject prefabText;
    [SerializeField] private Transform rootText;
    [SerializeField] private RectTransform verticalLayoutGroup;

    [SerializeField] private List<UIGroupItem> chatGroupItems;
    [SerializeField] private TextMeshProUGUI textUserCount;

    private List<ChatChannel> channels = new List<ChatChannel>();

    private ChatChannel curChannel = null;

    private List<GameObject> items = new List<GameObject>();

    private bool isChange = false;
    private int changeCount = 0;

    private List<UIText> channelTexts = new List<UIText>();

    public void Init()
    {
        channels.Add(new ChatChannel(){id = 0, isChannel = true, textCount = chatGroupItems[0].GetComponent<UIText>()});
        channels.Add(new ChatChannel(){id = 1, isChannel = true, textCount = chatGroupItems[1].GetComponent<UIText>() });
        channels.Add(new ChatChannel(){id = 2, isChannel = true, textCount = chatGroupItems[2].GetComponent<UIText>() });
        OnSelectChannel(0);
    }

    void Start()
    {
        prefabUser.SetActive(false);
        prefabText.SetActive(false);
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMsg();
        }

        if (isChange )
        {
            changeCount++;
            if (changeCount > 2)
            {
                isChange = false;
                changeCount = 0;
                LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup);
                chatScroll.verticalNormalizedPosition = 0.0f;
            }
        }
    }

    public void SendMsg()
    {
        string text = inputField.text;
        if (!string.IsNullOrEmpty(text))
        {
            Data.Instance.SendMsg(new MsgClient(MsgClintType.chat, new MsgClientChat(curChannel.isChannel, curChannel.id, inputField.text)));
            inputField.text = String.Empty;

            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    public void OnSelectChannel(int id)
    {
        var c = channels.Find(f => f.isChannel && f.id == id);
        if(c == null)
            return;

        if(c == curChannel)
            return;

        chatGroupItems[id].OnSelect();

        curChannel = c;
        DrawChannel();
    }

    private void DrawChannel()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();

        for (int i = 0; i < curChannel.items.Count; i++)
        {
            AddText(curChannel.items[i].name, curChannel.items[i].text);
        }
        chatScroll.verticalNormalizedPosition = 0f;

        curChannel.count = 0;
        curChannel.textCount.Init(String.Empty);
    }

    public bool IsFocus()
    {
        return inputField.isFocused;
    }

    public void UpdateUserList(List<uint> users)
    {
        //удаляем лишние
        for (int i = 0; i < channels.Count; i++)
        {
            if (!channels[i].isChannel && users.IndexOf(channels[i].id) == -1)
            {
                Destroy(channels[i].item);
                channels.RemoveAt(i);
                i--;
            }
        }

        //добавляем новые

        for (int i = 0; i < users.Count; i++)
        {
            if (channels.Find(f => !f.isChannel && f.id == users[i]) == null)
            {
                ChatChannel channel = new ChatChannel() {id = users[i], isChannel = false};
                GameObject obj = Instantiate(prefabUser, rootUsers);
                channel.item = obj;
                channel.textCount = obj.GetComponent<UIText>();
                UIChatUser chatUser = obj.GetComponent<UIChatUser>();
                chatUser.Init(users[i]);
                channels.Add(channel);
                obj.SetActive(true);
            }
        }

        textUserCount.text = (channels.Count - 3).ToString();
    }

    public void AddText(bool isChannel, uint id, string name, string text)
    {
        var c = channels.Find(f => f.isChannel == isChannel && f.id == id);

        if (c == null)
            return;

        c.items.Add(new ChatMsg(){name = name, text = text});
        if (curChannel == c)
        {
            chatScroll.verticalNormalizedPosition = 0f;
            AddText(name, text);
        }
        else
        {
            c.count++;
            c.textCount.Init(c.count.ToString());
        }
    }

    private void AddText(string name, string text)
    {
        GameObject obj = Instantiate(prefabText, rootText);

        string m = name + ": " + text;

        obj.GetComponent<UIChatText>().Init(m, text);

        items.Add(obj);
        obj.SetActive(true);

        isChange = true;
        changeCount = 0;
    }

    public void SelectUserChat(uint userId)
    {
        var c = channels.Find(f => !f.isChannel && f.id == userId);
        if (c == null)
            return;

        if (c == curChannel)
            return;

        curChannel = c;
        curChannel.item.GetComponent<UIGroupItem>().OnSelect();
        DrawChannel();
    }
}
