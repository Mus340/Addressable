using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private static AdsManager _ins;   
    public static AdsManager Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<AdsManager>();
            }
            return _ins;
        }
    }

    #if Test
    private string _adUnitId = "ca-app-pub-7661158316568075/5218603828";
    #else
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111"; //TestId
    #endif
    
    private InterstitialAd _interstitialAd;
    
    private float _adCooldown = 180f; // 3분
    private float _lastAdTime = -999f;
    private float _appStartTime;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _appStartTime = Time.time; 
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log($"ADMOB : {initStatus}");
            
            LoadInterstitialAd();
        });
    }

    public void TryShowAdOnGameOver()
    {
        float timeSinceAppStart = Time.time - _appStartTime;

        if (_interstitialAd != null &&
            _interstitialAd.CanShowAd() &&
            timeSinceAppStart >= _adCooldown &&
            Time.time - _lastAdTime >= _adCooldown)
        {
            Debug.Log("게임오버 → 전면 광고 표시");
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log($"StartTime : {timeSinceAppStart}");
            Debug.Log($"RemainTime : {Time.time - _lastAdTime}");
            Debug.Log("게임오버 → 광고 조건 미충족 (앱 시작 후 3분 경과해야 가능)");
        }
    }

    
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
        
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                    return;
                }
                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;
                RegisterReloadHandler(_interstitialAd);
            });
    }
    

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += ()=>
        {
            Debug.Log("Interstitial Ad full screen content closed.");
            _lastAdTime = Time.time;
            LoadInterstitialAd();
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            LoadInterstitialAd();
        };
    }
    
    private void OnDestroy()
    {
        _interstitialAd?.Destroy();
    }
}
