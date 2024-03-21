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
using UnityEngine.Events;
#if ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif

namespace POPBlocks.Scripts.Integrations.Ads
{
    public class AdmobController : MonoBehaviour
    {
        public AdItem adsSettingsAd;
        #if ADMOB

        private AppOpenAd appOpenAd;
        private BannerView bannerView;
        public InterstitialAd interstitialAd;
        public RewardedAd rewardedAd;
        private RewardedInterstitialAd rewardedInterstitialAd;

        public Action OnAdLoadedEvent;
        public Action OnAdFailedToLoadEvent;
        public Action OnAdOpeningEvent;
        public Action OnAdFailedToShowEvent;
        public Action OnInterstitialShownEvent;
        public Action OnUserEarnedRewardEvent;
        public Action OnAdClosedEvent;
        public Action OnAdLeavingApplicationEvent;

        #region UNITY MONOBEHAVIOR METHODS

        public void Start()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

            // Add some test device IDs (replace with your own device IDs).
            #if UNITY_IPHONE
            deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
            #elif UNITY_ANDROID
            deviceIds.Add("BD7AD3D5D206E54FF4E0F382C3ECB01B");
            #endif

            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration = new RequestConfiguration();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // Debug.Log("Initialization complete");
                //RequestBannerAd();
            });
        }

        #endregion

        #region HELPER METHODS

        private AdRequest CreateAdRequest()
        {
            return new AdRequest();
                // .AddKeyword("unity-admob-sample")
                // .AddExtra("color_bg", "9B30FF")
        }

        #endregion

        #region BANNER ADS

        public void RequestBannerAd()
        {
            var adUnitId = adsSettingsAd.adsId.First(i => i.type == AdType.Banner).id;


            // Clean up banner before reusing
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            // Create a 320x50 banner at top of the screen
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

            // Add Event Handlers
            bannerView.OnBannerAdLoaded += () =>
            {
                PrintStatus("Banner ad loaded.");
                OnAdLoadedEvent?.Invoke();
            };
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
                OnAdFailedToLoadEvent?.Invoke();
            };
            bannerView.OnAdImpressionRecorded += () => { PrintStatus("Banner ad recorded an impression."); };
            bannerView.OnAdClicked += () => { PrintStatus("Banner ad recorded a click."); };
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                PrintStatus("Banner ad opening.");
                OnAdOpeningEvent?.Invoke();
            };
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Banner ad closed.");
                OnAdClosedEvent?.Invoke();
            };
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                string msg = string.Format("{0} (currency: {1}, value: {2}",
                    "Banner ad received a paid event.",
                    adValue.CurrencyCode,
                    adValue.Value);
                PrintStatus(msg);
            };

            // Load a banner ad
            bannerView.LoadAd(CreateAdRequest());
        }

        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
        }

        #endregion

        #region INTERSTITIAL ADS

        public void RequestAndLoadInterstitialAd()
        {
            var adUnitId = adsSettingsAd.adsId.First(i => i.type == AdType.Interstitial).id;
            // Clean up the old ad before loading a new one.
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            InterstitialAd.Load(adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Interstitial ad failed to load with error: " +
                                    loadError.GetMessage());
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Interstitial ad failed to load.");
                        return;
                    }

                    PrintStatus("Interstitial ad loaded.");
                    interstitialAd = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Interstitial ad opening.");
                        OnAdOpeningEvent?.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Interstitial ad closed.");
                        OnInterstitialShownEvent?.Invoke();
                    };
                    ad.OnAdImpressionRecorded += () => { PrintStatus("Interstitial ad recorded an impression."); };
                    ad.OnAdClicked += () => { PrintStatus("Interstitial ad recorded a click."); };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Interstitial ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                            "Interstitial ad received a paid event.",
                            adValue.CurrencyCode,
                            adValue.Value);
                        PrintStatus(msg);
                    };
                });
        }

        public void ShowInterstitialAd()
        {
            interstitialAd.Show();
        }

        private void RegisterEventHandlers(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () => { Debug.Log("Interstitial ad recorded an impression."); };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () => { Debug.Log("Interstitial ad full screen content opened."); };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () => { Debug.Log("Interstitial ad full screen content closed."); };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);
            };
        }

        public void DestroyInterstitialAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
        }

        #endregion

        #region REWARDED ADS

        public void RequestAndLoadRewardedAd()
        {
            // Debug.Log("Requesting Rewarded Ad.");

            var adUnitId = adsSettingsAd.adsId.First(i => i.type == AdType.RewardedAd).id;

            // create new rewarded ad instance
            RewardedAd.Load(adUnitId, CreateAdRequest(),
                (RewardedAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Rewarded ad failed to load with error: " +
                                    loadError.GetMessage());
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Rewarded ad failed to load.");
                        return;
                    }

                    PrintStatus("Rewarded ad loaded.");
                    rewardedAd = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Rewarded ad opening.");
                        OnAdOpeningEvent?.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Rewarded ad closed.");
                        OnAdClosedEvent?.Invoke();
                    };
                    ad.OnAdImpressionRecorded += () => { PrintStatus("Rewarded ad recorded an impression."); };
                    ad.OnAdClicked += () => { PrintStatus("Rewarded ad recorded a click."); };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Rewarded ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                            "Rewarded ad received a paid event.",
                            adValue.CurrencyCode,
                            adValue.Value);
                        PrintStatus(msg);
                    };
                });
        }

        public void ShowRewardedAd()
        {
            if (rewardedAd != null)
            {
                rewardedAd.Show((Reward reward) =>
                {
                    OnUserEarnedRewardEvent?.Invoke();
                    PrintStatus("Rewarded ad granted a reward: " + reward.Amount);
                });
            }
            else
            {
                PrintStatus("Rewarded ad is not ready yet.");
            }
        }


        public void RequestAndLoadRewardedInterstitialAd()
        {
            var adUnitId = adsSettingsAd.adsId.First(i => i.type == AdType.RewardedAd).id;

            // Create a rewarded interstitial.
            RewardedInterstitialAd.Load(adUnitId, CreateAdRequest(),
                (RewardedInterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Rewarded interstitial ad failed to load with error: " +
                                    loadError.GetMessage());
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Rewarded interstitial ad failed to load.");
                        return;
                    }

                    PrintStatus("Rewarded interstitial ad loaded.");
                    rewardedInterstitialAd = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Rewarded interstitial ad opening.");
                        OnAdOpeningEvent?.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Rewarded interstitial ad closed.");
                        OnAdClosedEvent?.Invoke();
                    };
                    ad.OnAdImpressionRecorded += () => { PrintStatus("Rewarded interstitial ad recorded an impression."); };
                    ad.OnAdClicked += () => { PrintStatus("Rewarded interstitial ad recorded a click."); };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Rewarded interstitial ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                            "Rewarded interstitial ad received a paid event.",
                            adValue.CurrencyCode,
                            adValue.Value);
                        PrintStatus(msg);
                    };
                });
        }

        public void ShowRewardedInterstitialAd()
        {
            if (rewardedInterstitialAd != null)
            {
                rewardedInterstitialAd.Show((Reward reward) => { PrintStatus("Rewarded interstitial granded a reward: " + reward.Amount); });
            }
            else
            {
                PrintStatus("Rewarded interstitial ad is not ready yet.");
            }
        }

        #endregion

        ///<summary>
        /// Log the message and update the status text on the main thread.
        ///<summary>
        private void PrintStatus(string message)
        {
            Debug.Log(message);
        }

        #endif
    }
}