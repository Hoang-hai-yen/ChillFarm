
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
                instance.Auth = new FirebaseAuthServie();
                instance.Database = new FirebaseDatabaseService();
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