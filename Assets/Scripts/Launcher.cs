using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Com.TimCorporation.Multiplayer
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        [SerializeField] private byte maxPlayersPerRoom = 4;

        [SerializeField] private GameObject namePanel;
        
        [SerializeField] private GameObject load;

        [SerializeField] private Text feedbackText;
        
        #endregion

        #region Private Fields

        private string gameVersion = "1";
        private bool isConnecting;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            namePanel.SetActive(true);
            load.SetActive(false);
            feedbackText.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "";

            isConnecting = true;

            namePanel.SetActive(false);
            load.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                feedbackText.text = "Joining Room...";
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                feedbackText.text = "Connecting...";

                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                feedbackText.text = "OnConnectedToMaster: Next -> try to Join Random Room";
                Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n " +
                          "Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
                PhotonNetwork.JoinRandomRoom();
                // isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            feedbackText.text = "<Color=Red>OnDisconnected</Color> "+cause;
            namePanel.SetActive(true);
            load.SetActive(false);
            isConnecting = false;
            Debug.LogError("OnDisconnected() was called by PUN with reason \n" + cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            feedbackText.text = "<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room";
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\n" +
                      "Calling: PhotonNetwork.CreateRoom");
            
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
            // Debug.Log("Room Created: Waiting for Another Players");
        }

         public override void OnJoinedRoom()
         {
             feedbackText.text = "<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)";
             Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
             Debug.Log("Now this client is in a room.");
             if (PhotonNetwork.CurrentRoom.PlayerCount <= 4)
             {
                 Debug.Log("We load the Game Room");
                 PhotonNetwork.LoadLevel("GameRPG");
             }
         }

         #endregion
    }
}