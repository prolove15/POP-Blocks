// // ©2015 - 2023 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace POPBlocks.Scripts.Integrations.Ads
{
    public class AdsManager : MonoBehaviour, IAdsListener
    {
        private AdsSettings adsSettings;
        public List<AdEvents> adsEvents = new List<AdEvents>();
        public static AdsManager THIS;
        private IAdsHandler adsHandler;
        public delegate void RewEvent();
        public static event RewEvent OnRewardedShownEvent;
        public static event RewEvent OnRewardedFailEvent;
        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
            adsSettings = Resources.Load<AdsSettings>("Settings/AdsSettings");
            var adsSettingsAd = adsSettings.ads.Where(i=>i.enable).First(i=>i.enable);
            adsHandler = null;
            var adNetworkNetwork = adsSettingsAd.adNetwork.network;
            if (adNetworkNetwork == AdNetworks.UnityAds)
            {
                adsHandler = null;
                #if UNITY_ADS
                    adsHandler = new UnityAdsHandler(GetPlatformID(adsSettingsAd));
                #endif
            }
            else if (adNetworkNetwork == AdNetworks.Admob)
            {
                var admobController = gameObject.AddComponent<AdmobController>();
                admobController.adsSettingsAd = adsSettingsAd;
                adsHandler = new AdmobAdsHandler(GetPlatformID(adsSettingsAd), admobController);
            }
            else if (adNetworkNetwork == AdNetworks.Ironsource)
            {
                #if IRONSOURCE
                adsHandler = new IronsourceAdsHandler(GetPlatformID(adsSettingsAd));
                #endif
            }
            if (adsSettingsAd.showRewardedAds)
            {
                if (adsHandler != null)
                {
                    adsHandler.SetListener(this);
                }
            }

            foreach (var adTrigger in adsSettingsAd.adTriggers)
            {
                var adEvents = new AdEvents {gameEvent = adTrigger.trigger, adType = adTrigger.type, everyLevel = adTrigger.frequency};
                adEvents.adsHandler = adsHandler;
                adsEvents.Add(adEvents);
            }
        }

        private void OnInitialized()
        {
            adsHandler.LoadInterstitial();
            adsHandler.LoadRewardedAds();
        }

        private void OnRewardedFail()
        {
            OnRewardedFailEvent?.Invoke();
        }

        private void OnEnable()
        {
            LevelManager.OnGamestateChanged += CheckAdsEvents;
        }

        
        private void OnDisable()
        {
            LevelManager.OnGamestateChanged -= CheckAdsEvents;
        }

        private void OnRewardedShown()
        {
            adsHandler.LoadRewardedAds();
            OnRewardedShownEvent?.Invoke();
        }

        private void CheckAdsEvents(GameState state)
        {
            // Debug.Log("Check ads " + state);
            foreach (var item in adsEvents)
            {
                if (item.gameEvent == state )
                {
                    item.calls++;
                    if (item.adsHandler != null && item.calls % item.everyLevel == 0 && item.adsHandler.IsAvailable())
                    {
                        item.adsHandler.ShowAdsInterstitial();
                    }
                }
            }
        }

        public void ShowInterstitial()
        {
            adsHandler.ShowAdsInterstitial();
        }

        public void ShowRewarded()
        {
            adsHandler.ShowRewardedAds();
        }

        public bool IsRewardedAvailable()
        {
            return false;
        }
        
        private string GetPlatformID(AdItem adsSettingsAd)
        {
            
            #if UNITY_ADS || IRONSOURCE
            #if UNITY_EDITOR
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.Android).id;
                case BuildTarget.iOS:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.iOS).id;
            }
            #else
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                     return adsSettingsAd.adsId.First(i => i.platform == Platforms.Android).id;
                case RuntimePlatform.IPhonePlayer:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.iOS).id;
            }
            #endif
            #elif ADMOB
            
#if UNITY_EDITOR
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.Android && i.type == AdType.Interstitial).id;
                case BuildTarget.iOS:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.iOS && i.type == AdType.Interstitial).id;

            }
#else
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                     return adsSettingsAd.adsId.First(i => i.platform == Platforms.Android && i.type == AdType.Interstitial).id;
                case RuntimePlatform.IPhonePlayer:
                    return adsSettingsAd.adsId.First(i => i.platform == Platforms.iOS && i.type == AdType.Interstitial).id;
            }

#endif
            #endif
            return String.Empty;
        }

        #region IAdsListener implementation
        public void OnAdsInitialized()
        {
            OnInitialized();
        }

        public void OnAdsRewarded()
        {
            OnRewardedShown();
        }

        public void OnAdsRewardedFail()
        {
            OnRewardedFail();
        }

        public void OnAdsInterstitialShown()
        {
            adsHandler.LoadInterstitial();
        }

        #endregion
    }

    /// <summary>
    /// Ad event
    /// </summary>
    [Serializable]
    public class AdEvents
    {
        public GameState gameEvent;
        public AdType adType;
        public int everyLevel;
        public int calls;
        public IAdsHandler adsHandler;
    }
}