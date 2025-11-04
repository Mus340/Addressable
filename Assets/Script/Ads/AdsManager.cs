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
    
    public async Task InitializeAdmobAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob 초기화 완료");
            tcs.SetResult(true);
            
            LoadInterstitialAd();
        });
        await tcs.Task;
    }

    public void TryShowAdOnGameOver()
    {
        if (_interstitialAd == null)
        {
            Debug.LogError("Iniailized is Null");
            return;
        }
        else if (_interstitialAd.CanShowAd() == false)
        {
            Debug.LogError("Ads Cant show");
            return;
        }
        _interstitialAd.Show();
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
