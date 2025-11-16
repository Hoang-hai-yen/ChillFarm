public class FirebaseManager
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
       
        get
        {
            if(instance == null)
            {
                instance = new FirebaseManager();
                instance.Auth = new FirebaseAuthService(); 
                instance.Database = new FirebaseDatabaseService(ApiConfig.Instance);
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public FirebaseAuthService Auth { get; private set; } 
    public FirebaseDatabaseService Database { get; private set; }
    public enum FirebaseUrl
    {
        
    }

    private FirebaseManager() { }
    
}