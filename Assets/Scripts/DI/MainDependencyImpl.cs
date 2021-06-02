namespace Com.TimCorporation.Multiplayer
{
    public class MainDependencyImpl : MainDependencys
    {
        private static MainDependencys instance = new MainDependencyImpl();
        private ModuleManager moduleManager = new ModuleManagerImpl();
        private ServiceManager serviceManager;
        
        public MainDependencyImpl()
        {
            serviceManager = new ServiceManagerImpl(moduleManager);
        }
        
        
        public static MainDependencys getInstance()
        {
            return instance;
        }
        
        public ModuleManager GetModuleManager()
        {
            return moduleManager;
        }

        public ServiceManager GetServiceManager()
        {
            return serviceManager;
        }
    }
}