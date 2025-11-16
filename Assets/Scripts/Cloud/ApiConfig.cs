using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ApiConfig
{
    public string ApiKey;
    public string ProjectId;
    public string Register;
    public string Login;
    public string Refresh;
    public string SendPassResetEmail;
    public string VerifyPassResetCode;
    public string ConfirmPassReset;
    public string Database;

    private static ApiConfig instance;
    public static ApiConfig Instance
    {
        get
        {
            if (instance == null)
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("apiConfig");
                instance = JsonUtility.FromJson<ApiConfig>(jsonFile.text);
            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public string SendOobCode { get; internal set; }
}

