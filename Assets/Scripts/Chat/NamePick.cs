using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.TimCorporation.Multiplayer.Chat
{
    [RequireComponent(typeof (ChatGui1))]
    public class NamePick : MonoBehaviour
    {
        private const string UserNamePlayerPref = "NamePickUserName";

        public ChatGui1 chatNewComponent;

        public InputField idInput;

        public void Start()
        {
            this.chatNewComponent = FindObjectOfType<ChatGui1>();


            string prefsName = PlayerPrefs.GetString(NamePick.UserNamePlayerPref);
            if (!string.IsNullOrEmpty(prefsName))
            {
                this.idInput.text = prefsName;
            }
        }


        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
        public void EndEditOnEnter()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                this.StartChat();
            }
        }

        public void StartChat()
        {
            ChatGui1 chatNewComponent = FindObjectOfType<ChatGui1>();
            chatNewComponent.UserName = this.idInput.text.Trim();
            chatNewComponent.Connect();
            enabled = false;

            PlayerPrefs.SetString(NamePick.UserNamePlayerPref, chatNewComponent.UserName);
            PhotonNetwork.NickName = chatNewComponent.UserName;
        }
    }
}


