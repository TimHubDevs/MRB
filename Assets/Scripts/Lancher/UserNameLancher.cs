using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.TimCorporation.Multiplayer
{
    [RequireComponent(typeof(InputField))]
    public class UserNameLancher : MonoBehaviour
    {
        private InputField inputField;
        #region Private Constants

        private const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            string defaultName = string.Empty;
            inputField = this.GetComponent<InputField>();
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
            value = inputField.text;
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