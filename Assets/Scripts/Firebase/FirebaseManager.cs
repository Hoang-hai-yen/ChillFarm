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
                instance.Auth = new FirebaseAuthService(); // <--- SỬA: Lỗi chính tả
                instance.Database = new FirebaseDatabaseService(ApiConfig.Instance); // <--- SỬA: Thêm (ApiConfig.Instance)
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    public FirebaseAuthService Auth { get; private set; } // <--- SỬA: Lỗi chính tả
    public FirebaseDatabaseService Database { get; private set; }

    public enum FirebaseUrl
    {
        
    }

    private FirebaseManager() { }
    
}