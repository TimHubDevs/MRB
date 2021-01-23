using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    // set in inspector. Up to a certain degree, previously sent messages can be fetched for context
    public int HistoryLengthToFetch;
    public Text UserIdText; // set in inspector
    public InputField InputFieldChat; // set in inspector
    /**/public Toggle ChannelToggleToInstantiate; // set in inspector
    /**/public Text CurrentChannelText;     // set in inspector



    private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

    public string UserName { get; set; }
    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];
    
    
    private ChatClient chatClient;
    private string nameRoom = "ChatRoom";

    private string selectedChannelName; // mainly used for GUI/input


    [SerializeField] private string userId;
    [SerializeField] private GameObject joinUI;
    [SerializeField] private GameObject chatUI;


    #region MonoBehaviour

    void Start()
    {
        this.UserIdText.text = "";
        this.UserIdText.gameObject.SetActive(true);

        chatUI.SetActive(false);

        if (string.IsNullOrEmpty(this.UserName))
        {
            this.UserName = "user" + Environment.TickCount % 99; //made-up username
            Debug.Log("user is " + UserName);
        }

        chatClient = new ChatClient(this);

        // chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userId));
    }

    void Update()
    {
        chatClient.Service();
    }

    #endregion

    #region MyOwn Methods

    public void Connect()
    {
        this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
        Debug.Log("Connecting as: " + this.UserName);
    }

    private void SendChatMessage(string inputLine)
    {
        
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }

        if ("test".Equals(inputLine))
        {
            if (this.TestLength != this.testBytes.Length)
            {
            	this.testBytes = new byte[this.TestLength];
            }
            
            this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
        }


        bool doingPrivateChat = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
        string privateChatTarget = string.Empty;
        if (doingPrivateChat)
        {
            // the channel name for a private conversation is (on the client!!) always composed of both user's IDs: "this:remote"
            // so the remote ID is simple to figure out

            string[] splitNames = this.selectedChannelName.Split(new char[] {':'});
            privateChatTarget = splitNames[1];
        }

        
        Debug.Log("selectedChannelName: " + selectedChannelName + " doingPrivateChat: " + doingPrivateChat +
                              " privateChatTarget: " + privateChatTarget);


        if (doingPrivateChat)
        {
            this.chatClient.SendPrivateMessage(privateChatTarget, inputLine);
        }
        else
        {
            this.chatClient.PublishMessage(this.selectedChannelName, inputLine);
        }
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            this.SendChatMessage(this.InputFieldChat.text);
            this.InputFieldChat.text = "";
        }
    }

    //??dont use
    // public void OnClickSend()
    // {
    //     if (this.InputFieldChat != null)
    //     {
    //         this.SendChatMessage(this.InputFieldChat.text);
    //         this.InputFieldChat.text = "";
    //     }
    // }
    
    private void InstantiateChannelButton(string channelName)
    {
        if (this.channelToggles.ContainsKey(channelName))
        {
            Debug.Log("Skipping creation for an existing channel toggle.");
            return;
        }

        Toggle cbtn = (Toggle)Instantiate(this.ChannelToggleToInstantiate);
        cbtn.gameObject.SetActive(true);
        cbtn.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
        cbtn.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);

        this.channelToggles.Add(channelName, cbtn);
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        this.selectedChannelName = channelName;
        this.CurrentChannelText.text = channel.ToStringMessages();
        Debug.Log("ShowChannel: " + this.selectedChannelName);

        foreach (KeyValuePair<string, Toggle> pair in this.channelToggles)
        {
            pair.Value.isOn = pair.Key == channelName ? true : false;
        }
    }
    
    #endregion

    #region IChatClientListener Region

    public void DebugReturn(DebugLevel level, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        this.chatClient.Subscribe(new[] {nameRoom});

        this.UserIdText.text = "Connected as " + this.UserName;
    }

    public void OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
       
        //this.InstantiateChannelButton(channelName);

        byte[] msgBytes = message as byte[];
        if (msgBytes != null)
        {
            Debug.Log("Message with byte[].Length: "+ msgBytes.Length);
        }
        if (this.selectedChannelName.Equals(channelName))
        {
            this.ShowChannel(channelName);
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (string channel in channels)
        {
            this.chatClient.PublishMessage(channel, "says 'hi'."); // you don't HAVE to send a msg on join but you could.

            if (this.ChannelToggleToInstantiate != null)
            {
                this.InstantiateChannelButton(channel);

            }
        }

        Debug.Log("OnSubscribed: " + string.Join(", ", channels));
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (string channelName in channels)
        {
            if (this.channelToggles.ContainsKey(channelName))
            {
                Toggle t = this.channelToggles[channelName];
                Destroy(t.gameObject);

                this.channelToggles.Remove(channelName);

                Debug.Log("Unsubscribed from channel '" + channelName + "'.");

                // Showing another channel if the active channel is the one we unsubscribed from before
                if (channelName == this.selectedChannelName && this.channelToggles.Count > 0)
                {
                    IEnumerator<KeyValuePair<string, Toggle>> firstEntry = this.channelToggles.GetEnumerator();
                    firstEntry.MoveNext();

                    this.ShowChannel(firstEntry.Current.Key);

                    firstEntry.Current.Value.isOn = true;
                }
            }
            else
            {
                Debug.Log("Can't unsubscribe from channel '" + channelName + "' because you are currently not subscribed to it.");
            }
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}