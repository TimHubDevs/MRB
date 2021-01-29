using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;

#endif
public class ChatGame : MonoBehaviour, IChatClientListener
{
    [SerializeField] private RectTransform ChatPanel;
    [SerializeField] private InputField InputFieldChat;
    [SerializeField] private Text CurrentChannelText;
    [SerializeField] private GameObject showButtonChat;
    [SerializeField] private int HistoryLengthToFetch;
    private string[] ChannelsToJoinOnConnect;
    
#if !PHOTON_UNITY_NETWORKING
    [SerializeField]
#endif
    protected internal ChatAppSettings chatAppSettings;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);


#if PHOTON_UNITY_NETWORKING
        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
#endif

        this.ChatPanel.gameObject.SetActive(false);
        this.showButtonChat.gameObject.SetActive(false);
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            // this.SendChatMessage(this.InputFieldChat.text);
            this.InputFieldChat.text = "";
        }
    }

    public void OnClickSend()
    {
        if (this.InputFieldChat != null)
        {
            // this.SendChatMessage(this.InputFieldChat.text);
            this.InputFieldChat.text = "";
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new NotImplementedException();
    }

    public void OnConnected()
    {
        this.ChatPanel.gameObject.SetActive(true);
        this.showButtonChat.gameObject.SetActive(true);

        List<string> nameRoom = new List<string>();
        nameRoom.Add("chat");
        ChannelsToJoinOnConnect = nameRoom.ToArray();
        if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
        {
            //this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        throw new NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        throw new NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }
}