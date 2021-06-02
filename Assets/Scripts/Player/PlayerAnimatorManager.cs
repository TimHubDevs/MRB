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
        private float horizontal;
        private float vertical;

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
// #if UNITY_EDITOR
//             horizontal = Input.GetAxis("Horizontal");
//             vertical = Input.GetAxis("Vertical");
// #endif

            horizontal = FloatingJoystick.Instance.Direction.x;
            vertical = FloatingJoystick.Instance.Direction.y;

            if (vertical < 0)
            {
                vertical = Mathf.Abs(0);
            }

            animator.SetFloat("Speed", horizontal * horizontal + vertical * vertical);
            animator.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime);
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