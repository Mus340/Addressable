using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UniRx;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class Login : MonoBehaviour
{    
    private static Login _ins;   
    public static Login Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<Login>();
            }
            return _ins;
        }
    }
    
    private FirebaseAuth _auth;
    public string UserID { get; private set; }
    public bool IsNewUser { get; private set; }
    public async Task LoginUser()
    {
        try
        {
            var dependencyResult = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyResult == DependencyStatus.Available)
            {
                await LoginUserAsync();
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyResult}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase 초기화 실패: {e}");
        }
    }

    private async Task LoginUserAsync()
    {
        _auth = FirebaseAuth.DefaultInstance;

#if UNITY_IOS && !UNITY_EDITOR
    await AuthenticateGameCenterAsync();
#elif UNITY_ANDROID && !UNITY_EDITOR
    await LoginGooglePlayGames();
#endif
        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        if (_auth.CurrentUser != null)
        {
            UserID = _auth.CurrentUser.UserId;
            IsNewUser = false;
            Debug.Log($"기존 익명 사용자로 로그인됨 : {UserID}");
            return;
        }
        var userCredential = await _auth.SignInAnonymouslyAsync();
        if (userCredential != null && userCredential.User != null)
        {
            UserID = userCredential.User.UserId;
            IsNewUser = true;
            Debug.Log($"새 익명 사용자로 로그인됨 : {UserID}");
        }
    }
 
#if UNITY_IOS
    private async Task AuthenticateGameCenterAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        Social.localUser.Authenticate(success =>
        {
            Debug.Log(success ? "Game Center 로그인 성공" : "Game Center 로그인 실패");
            tcs.SetResult(success);
        });
        await tcs.Task;
    }
    #endif
    
    #if UNITY_ANDROID
    private async Task LoginGooglePlayGames()
    {
        PlayGamesPlatform.Activate();
        var tcs = new TaskCompletionSource<bool>();
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");
                tcs.SetResult(true);
            }
            else
            {
                Debug.Log("Login Unsuccessful");
            }
        });
        await tcs.Task;
    }
#endif

}
