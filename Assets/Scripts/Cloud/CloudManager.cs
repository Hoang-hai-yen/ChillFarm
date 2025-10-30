public class CloudManager
{
    private static CloudManager instance;
    public static CloudManager Instance
    {
       
        get
        {
            if(instance == null)
            {
                instance = new CloudManager();
                ApiConfig apiConfig = ApiConfig.Instance;
                instance.Auth = new CloudAuthService(apiConfig);
                instance.Database = new CloudDatabaseService(apiConfig); 
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public CloudAuthService Auth { get; private set; } 
    public CloudDatabaseService Database { get; private set; }

    public enum FirebaseUrl
    {
        
    }

    private CloudManager() { }
    
}