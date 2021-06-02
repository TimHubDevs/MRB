using Com.TimCorporation.Multiplayer.Services;

namespace Com.TimCorporation.Multiplayer
{
    public class ServiceManagerImpl : ServiceManager
    {
        private PlayerControlService _playerControlService;
        public ServiceManagerImpl(ModuleManager moduleManager)
        {
            _playerControlService = new PlayerControlServiceImpl();
        }

        public PlayerControlService getPlayerControlService()
        {
            return _playerControlService;
        }
    }
}