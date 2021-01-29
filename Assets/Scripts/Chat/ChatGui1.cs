using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;

#endif

public class ChatGui1 : MonoBehaviour, IChatClientListener
{
    [SerializeField] private GameObject connect;
    
    public string UserName { get; set; }
    
    public ChatClient chatClient;

#if !PHOTON_UNITY_NETWORKING
    [SerializeField]
#endif
    protected internal ChatAppSettings chatAppSettings;
    
    public GameObject UserIdFormPanel;
    
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
        this.connect.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(this.UserName))
        {
            this.UserName = "user" + Environment.TickCount % 99; //made-up username
        }

#if PHOTON_UNITY_NETWORKING
        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
#endif

        bool appIdPresent = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);

        this.UserIdFormPanel.gameObject.SetActive(appIdPresent);

        if (!appIdPresent)
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
        }
    }

    public void Connect()
    {
        this.UserIdFormPanel.gameObject.SetActive(false);
        this.connect.gameObject.SetActive(true);

        this.chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        this.chatClient.UseBackgroundWorkerForSending = true;
#endif

        this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
        Debug.Log("Connecting as: " + this.UserName);

        this.chatClient.ConnectUsingSettings(this.chatAppSettings);
        Debug.Log("Connecting...");
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    public void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient
                .Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }


    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];

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

        if (inputLine[0].Equals('\\'))
        {
            string[] tokens = inputLine.Split(new char[] {' '}, 2);

            if (tokens[0].Equals("\\state"))
            {
                int newState = 0;


                List<string> messages = new List<string>();
                messages.Add("i am state " + newState);
                string[] subtokens = tokens[1].Split(new char[] {' ', ','});

                if (subtokens.Length > 0)
                {
                    newState = int.Parse(subtokens[0]);
                }

                if (subtokens.Length > 1)
                {
                    messages.Add(subtokens[1]);
                }

                this.chatClient.SetOnlineStatus(newState,
                    messages.ToArray()); // this is how you set your own state and (any) message
            }
            else if ((tokens[0].Equals("\\subscribe") || tokens[0].Equals("\\s")) && !string.IsNullOrEmpty(tokens[1]))
            {
                this.chatClient.Subscribe(tokens[1].Split(new char[] {' ', ','}));
            }
            else if ((tokens[0].Equals("\\unsubscribe") || tokens[0].Equals("\\u")) && !string.IsNullOrEmpty(tokens[1]))
            {
                this.chatClient.Unsubscribe(tokens[1].Split(new char[] {' ', ','}));
            }
            else if (tokens[0].Equals("\\clear"))
            {
            }
            else if (tokens[0].Equals("\\msg") && !string.IsNullOrEmpty(tokens[1]))
            {
                string[] subtokens = tokens[1].Split(new char[] {' ', ','}, 2);
                if (subtokens.Length < 2) return;

                string targetUser = subtokens[0];
                string message = subtokens[1];
                this.chatClient.SendPrivateMessage(targetUser, message);
            }
            else if ((tokens[0].Equals("\\join") || tokens[0].Equals("\\j")) && !string.IsNullOrEmpty(tokens[1]))
            {
                string[] subtokens = tokens[1].Split(new char[] {' ', ','}, 2);
            }
            else
            {
                Debug.Log("The command '" + tokens[0] + "' is invalid.");
            }
        }
    }


    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnConnected()
    {
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a message).
    }

    public void OnDisconnected()
    {
    }

    public void OnChatStateChange(ChatState state)
    {
        // use OnConnected() and OnDisconnected()
        // this method might become more useful in the future, when more complex states are being used.
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // in this demo, we simply send a message into each channel. This is NOT a must have!
        foreach (string channel in channels)
        {
            this.chatClient.PublishMessage(channel,
                "says 'hi'."); // you don't HAVE to send a msg on join but you could.
        }

        Debug.Log("OnSubscribed: " + string.Join(", ", channels));

        // Switch to the first newly created channel
        this.ShowChannel(channels[0]);
    }

    /// <inheritdoc />
    public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
    {
        Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", channel, users.Length,
            properties.ToStringFull());
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }
    
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
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
    }
}