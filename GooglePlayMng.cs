using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
//using Boo.Lang.Environments;

public class GooglePlayMng : MonoBehaviour
{
    private static GooglePlayMng instance;

    public static GooglePlayMng Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<GooglePlayMng>();
            if (instance == null) instance = new GameObject("GooglePlayMng").AddComponent<GooglePlayMng>();
            return instance;
        }
    }
    public Text scoretText;
    public Text myLog;
    public RawImage myImage;

    private bool waitingForAuth = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        myLog.text = "Ready..";
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    // Start is called before the first frame update
    void Start()
    {
        doAutoLogin();
    }

    public void doAutoLogin()
    {
        myLog.text = "...";
        if (waitingForAuth) return;
        if (!Social.localUser.authenticated)
        {
            myLog.text = "Authenticating...";
            waitingForAuth = true;
            Social.localUser.Authenticate(AuthenticateCallBack);
        }else
        {
            myLog.text = "Login Failed";
        }
    }

    public void onBtnLoginClicked()
    {
        if (Social.localUser.authenticated)
        {
            Debug.Log(Social.localUser.userName);
            UIMng.instance.myId.text = "" + Social.localUser.userName + "\n";
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log(Social.localUser.userName);
                    UIMng.instance.myId.text = "" + Social.localUser.userName + "\n";
                }else
                {
                    Debug.Log("LogFailed");
                    UIMng.instance.myId.text = "LogIn Failed";
                }
            });
        }
    }
    public void OnBtnLogOutClicked()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        myLog.text = "LogOut..";
    }

    void AuthenticateCallBack(bool success)
    {
        myLog.text = "Loading..";

        if (success)
        {
            myLog.text = "Welcome" + Social.localUser.userName + "\n";
            StartCoroutine(UserPictureLoad());
        }else
        {
            myLog.text = "Login Failed \n";
        }
    }

    IEnumerator UserPictureLoad()
    {
        myLog.text = "img Loading...\n";
        Texture2D pic = Social.localUser.image;
        while(pic == null)
        {
            pic = Social.localUser.image;
            yield return null;
        }
        //UIMng.instance.success = true;
        myImage.texture = pic;
        myLog.text = "" + Social.localUser.userName + "\n";
    }
   
}
