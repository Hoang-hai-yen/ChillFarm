
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
                ApiConfig apiConfig = ApiConfig.Instance;
                instance.Auth = new FirebaseAuthServie(apiConfig);
                instance.Database = new FirebaseDatabaseService(apiConfig);
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public FirebaseAuthServie Auth { get; private set; }
    public FirebaseDatabaseService Database { get; private set; }

    public enum FirebaseUrl
    {
        
    }

    private FirebaseManager() { }
    
}