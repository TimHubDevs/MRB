using System;

namespace Com.TimCorporation.Multiplayer.Services
{
    public class PlayerControlServiceImpl : PlayerControlService
    {
        public void Jump(Action jump)
        {
            PlayerAnimatorManager.Instance.Jump();
        }
    }
}