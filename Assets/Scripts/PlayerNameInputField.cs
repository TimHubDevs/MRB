using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace Com.TimCorporation.Multiplayer
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        private const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            string defaultName = string.Empty;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;
            
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}