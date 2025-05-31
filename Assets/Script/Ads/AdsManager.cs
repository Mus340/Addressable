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
    
    private string adUnitId = "ca-app-pub-3940256099942544/6300978111";
    private BannerView _bannerView;
    
    
    public async Task InitializeAdmobAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob 초기화 완료");
            tcs.SetResult(true);
        });
        await tcs.Task;
    }

    private void RequestBanner()
    {
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        _bannerView.LoadAd(request);
    }

    private void OnDestroy()
    {
        _bannerView?.Destroy();
    }
}
