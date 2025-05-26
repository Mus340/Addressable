using System;
using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;

public class UIRankingPopup : UIPopupPanel
{
    public DynamicVScrollView ScrollView;

    private void Awake()
    {
        ScrollView.totalItemCount = 100;
    }
    
}
