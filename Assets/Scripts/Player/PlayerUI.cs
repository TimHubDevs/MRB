using UnityEngine;
using UnityEngine.UI;

namespace Com.TimCorporation.Multiplayer
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [SerializeField]
        private Text playerNameText;

        [SerializeField]
        private Slider playerHealthSlider;

        PlayerManager playerManager;
        
        #endregion


        #region MonoBehaviour Callbacks

        void Update()
        {
            if (playerManager == null)
            {
                Destroy(this.gameObject);
                return;
            }


            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = playerManager.Health;
            }
        }

        #endregion


        #region Public Methods

        public void SetTarget(PlayerManager playerManager)
        {
            if (playerManager == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.",
                    this);
                return;
            }
            
            this.playerManager = playerManager;

            if (playerNameText != null)
            {
                playerNameText.text = this.playerManager.photonView.Owner.NickName;
            }
        }

        #endregion
    }
}