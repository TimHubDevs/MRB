using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace Com.TimCorporation.Multiplayer
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        [SerializeField] private GameObject beams;

        [SerializeField] public GameObject gameCanvas;

        bool IsFiring;

        public static PlayerManager Instance { get; private set; }

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            Instance = this;

            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.

            // DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            ThirdCamera thirdCamera = gameObject.GetComponent<ThirdCamera>();

            if (thirdCamera == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            if (gameCanvas != null)
            {
                GameObject uiGo = Instantiate(gameCanvas);
                uiGo.GetComponentInChildren<PlayerUI>().SetTarget(this.GetComponent<PlayerManager>());
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Update()
        {
            // if (photonView.IsMine)
            // {
            //     this.ProcessInputs();
            //     if (Health <= 0f)
            //     {
            //         GameManager.Instance.LeaveRoom();
            //     }
            // }
            //
            // if (beams != null && IsFiring != beams.activeInHierarchy)
            // {
            //     beams.SetActive(IsFiring);
            // }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            Health -= 0.1f * Time.deltaTime;
        }

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            // Create the UI
            GameObject uiGo = Instantiate(this.gameCanvas);
            uiGo.GetComponentInChildren<PlayerUI>().SetTarget(this.GetComponent<PlayerManager>());
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Private Methods

        public async UniTask ProcessInputs()
        {
            await Fire();

            await UniTask.Delay(1500);

            await StopFire();
        }

        private async UniTask StopFire()
        {
            if (photonView.IsMine)
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
        }

        private async UniTask Fire()
        {
            if (photonView.IsMine)
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }

                if (Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool) stream.ReceiveNext();
                this.Health = (float) stream.ReceiveNext();
            }
        }

        #endregion
    }
}