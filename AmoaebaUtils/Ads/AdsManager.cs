using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using AmoaebaUtils;
using System;

namespace AmoaebaAds
{

using OnAdCompletion = System.Action<string, AdsResult>;

public enum AdsResult
    {
        Failed = ShowResult.Failed,
        Skipped = ShowResult.Skipped,
        Finished = ShowResult.Finished,
        TimedOut ,

    }

public class AdsManager : BootScriptableObject, IUnityAdsListener 
{   
    [SerializeField]
    private string gameId = "3651551";

    [SerializeField]
#if DEBUG
    bool testMode = true;
#else
    bool testMode = false;
#endif

    private Dictionary<string, List<OnAdCompletion>> awaitingCallbacks = new Dictionary<string, List<OnAdCompletion>>();
    private HashSet<string> awaitingToShow = new HashSet<string>();

    private IEnumerator timeOutRoutine = null;
    private CoroutineRunner runner = null;

    private bool initialized = false;
    public override void OnBoot()
    {       
        OnStop();

        if(runner == null)
        {
            runner = CoroutineRunner.Instantiate(this.name);

            
            //AdsManagerRunner manager = runner.gameObject.AddComponent<AdsManagerRunner>();
            //manager.Setup(gameId, testMode, this); 

            // For some reason Advertisement needs to be initialized from the main thread
            UnityMainThreadDispatcher.Instance().EnqueueAsync(()=> 
            {
                Advertisement.Initialize (gameId, testMode);
                Advertisement.AddListener (this);
            });
            
                    
        }

        initialized = true;
    }

    private void Init() 
    {
        OnStop();

        if(runner == null)
        {
            runner = CoroutineRunner.Instantiate(this.name);

            
            //AdsManagerRunner manager = runner.gameObject.AddComponent<AdsManagerRunner>();
            //manager.Setup(gameId, testMode, this); 

            // For some reason Advertisement needs to be initialized from the main thread
            UnityMainThreadDispatcher.Instance().EnqueueAsync(()=> 
            {
                Advertisement.Initialize (gameId, testMode);
                Advertisement.AddListener (this);
            });
            
                    
        }

        initialized = true;
    }


    public override void OnStop()
    {
            
        if(Application.isPlaying && Advertisement.isInitialized)
        {
            Advertisement.RemoveListener(this);
        }

        StopTimeOut();

       if(runner != null)
       {
           Destroy(runner.gameObject);
           runner = null;
       }
    }

    private IEnumerator TimeOut(string placementId, float timeOut)
    {
        yield return new WaitForSeconds(timeOut);

        if(awaitingCallbacks.ContainsKey(placementId))
        {
            Debug.Log("Ad TimedOut for placement - " + placementId);

            List<OnAdCompletion> placementCallbacks = awaitingCallbacks[placementId];
            foreach(OnAdCompletion callback in placementCallbacks)
            {
                if(callback != null)
                {
                    callback.Invoke(placementId, AdsResult.TimedOut);
                }
            }
            awaitingCallbacks[placementId].Clear();
        }
        awaitingToShow.Remove(placementId);
    }

    private void StopTimeOut()
    {
        
        if(timeOutRoutine != null && runner != null)
        {
            runner.StopCoroutine(timeOutRoutine);
        }
        timeOutRoutine = null;
    }

    public bool ShowAd(string placementId,  Action<string, AdsResult> onFinish, bool showWhenAvailable = false, float timeOut = 5.0f)
    {     
        if(Advertisement.isShowing || !Advertisement.isSupported || !initialized)
        {
            return false;
        }

        if(!Advertisement.isInitialized)
        {
            if(!showWhenAvailable)
            {
                return false;
            }
        }

        StopTimeOut();

        if(!awaitingCallbacks.ContainsKey(placementId))
        {
            awaitingCallbacks.Add(placementId, new List<OnAdCompletion>());
        }
        awaitingCallbacks[placementId].Add(onFinish);

        return ShowAd(placementId, showWhenAvailable, timeOut);
    }
    private bool ShowAd(string placementId, bool showWhenAvailable, float timeOut = 5.0f)
    {
        if(Advertisement.IsReady(placementId))
        {
            StopTimeOut();
            Advertisement.Show(placementId);
            return true;
        }

        if(showWhenAvailable)
        {
            
           timeOutRoutine = TimeOut(placementId, timeOut);
           runner.StartCoroutine(timeOutRoutine);
           
           awaitingToShow.Add(placementId);
           return true;
        }

        return false;
    }
    
    private AdsResult ResultFromShownResult(ShowResult result)
    {
        switch(result)
        {
            case ShowResult.Failed:
                return AdsResult.Failed;
            case ShowResult.Skipped:
                return AdsResult.Skipped;
            case ShowResult.Finished:
                return AdsResult.Finished;
        }
        Debug.LogError("Unexpected type for ShowResult in AdsManager");
        return AdsResult.Failed;
    }


    public bool HasAdsAvaliable(string placementId)
    {
        return Advertisement.IsReady(placementId); 
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) 
    {
        Debug.Log("Ad Finished on Unity Ads: " + placementId + " : " + showResult);

        if(awaitingCallbacks.ContainsKey(placementId))
        {
            List<OnAdCompletion> placementCallbacks = awaitingCallbacks[placementId];
            foreach(OnAdCompletion callback in placementCallbacks)
            {
                if(callback != null)
                {
                    callback.Invoke(placementId, ResultFromShownResult(showResult));
                }
            }
            awaitingCallbacks[placementId].Clear();
        }

        awaitingToShow.Remove(placementId);
    }

    public void OnUnityAdsReady (string placementId) 
    {
        if(awaitingToShow.Contains(placementId))
        {
            awaitingToShow.Remove(placementId);
            ShowAd(placementId, true);
        }
        
        Debug.Log("Ad Ready on Unity Ads: " + placementId);
    }

    public void OnUnityAdsDidError (string message) {
        Debug.Log("Error on Unity Ads: " + message);
    }

    public void OnUnityAdsDidStart (string placementId) 
    {
        Debug.Log("Ad Started  on Unity Ads: " + placementId);
    } 
}
}