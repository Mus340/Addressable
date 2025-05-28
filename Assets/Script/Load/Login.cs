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
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    private AsyncSubject<Unit> _onLoadComplete = new AsyncSubject<Unit>();

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

#if UNITY_IOS
    await AuthenticateGameCenterAsync();
#elif UNITY_ANDROID
    await AuthenticateGooglePlayAsync();
#endif
        await SignInAnonymouslyAsync();
        _onLoadComplete.OnNext(Unit.Default);
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            var userCredential = await _auth.SignInAnonymouslyAsync();
            if (userCredential != null && userCredential.User != null)
            {
                Main.Ins.MainUser.SetUid(userCredential.User.UserId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"익명 로그인 실패: {e}");
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
    private async Task AuthenticateGooglePlayAsync()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    
        var tcs = new TaskCompletionSource<bool>();
        Social.localUser.Authenticate(success =>
        {
            Debug.Log(success ? "Google Play Games 로그인 성공" : "Google Play Games 로그인 실패");
            tcs.SetResult(success);
        });
        await tcs.Task;
    }
    #endif
}
