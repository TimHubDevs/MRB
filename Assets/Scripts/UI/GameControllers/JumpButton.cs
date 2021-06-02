using UnityEngine;

namespace Com.TimCorporation.Multiplayer
{
    public class JumpButton : MonoBehaviour
    {
        public void Jump()
        {
            PlayerAnimatorManager.Instance.Jump();
        }
    }
}