using Photon.Pun;
using UnityEngine;

namespace Com.TimCorporation.Multiplayer
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField] private float directionDampTime = 0.25f;

        #endregion

        #region MonoBehaviour Callbacks

        private Animator animator;
        
        private AnimatorStateInfo stateInfo;

        public static PlayerAnimatorManager Instance { get; private set; }


        void Start()
        {
            Instance = this;
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }


        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            
            if (!animator)
            {
                return;
            }

            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0)
            {
                v = Mathf.Abs(0);
            }

            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion

        public void Jump()
        {
            if (stateInfo.IsName("Base Layer.Run"))
            {
                animator.SetTrigger("Jump");
            }
        }

    }
}