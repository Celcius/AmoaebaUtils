using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Advertisements;
using AmoaebaUtils;

namespace AmoaebaAds
{
[RequireComponent(typeof(Menu))]
public class MenuAdHelper : MonoBehaviour
{
    [SerializeField]
    private AdsManager manager;

    private Menu menu;

    private void Awake()
    {
        menu = GetComponent<Menu>();
        Assert.IsNotNull(manager, "No Ads Manager allocated to " + this.gameObject.name);
    }

    public bool WatchAds(string placementId, Action<string, AdsResult> menuCallback, bool showOnAvail = true)
    {
        Action<string, AdsResult> callback = (string placement, AdsResult result) => 
        { 
            menuCallback(placement, result);
            menu.EnableButtons();
            Debug.Log("Ad On Menu finished with " + result);
        };

        menu.DisableButtons();

        bool willShow = manager.ShowAd(placementId, callback, showOnAvail);
        if(!willShow)
        {
            menu.EnableButtons();
        }
        Debug.Log("Ad will show " + willShow);
        return willShow;
    }
}
}