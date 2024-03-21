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
using UnityEngine;

namespace POPBlocks.Scripts.Integrations.Ads
{
#if UNITY_ADS
    using UnityEngine.Advertisements;

    [Serializable]
    public class UnityAdsHandler : IAdsHandler, IUnityAdsInitializationListener, IUnityAdsLoadListener,
    IUnityAdsShowListener
    {
        private bool testMode;
        private bool adLoaded;
        private IAdsListener _listener;

        public UnityAdsHandler(string _id)
        {
            if (Advertisement.isSupported)
            {
                DebugLog(Application.platform + " supported by Advertisement");
            }
            Advertisement.Initialize(_id, testMode, this);
            
        }

        #region IAdsHandler Implementations

        public void ShowAdsInterstitial()
        {
            Advertisement.Show("video", this);
        }

        public bool IsAvailable()
        {
            return adLoaded;
        }

        public void ShowRewardedAds()
        {
            if (IsAvailable())
                Advertisement.Show("rewardedVideo", this);
        }

        public void LoadInterstitial()
        {
            Advertisement.Load("video", this);
        }

        public void LoadRewardedAds()
        {
            Advertisement.Load("rewardedVideo", this);
        }   
        #endregion
        
        #region Interface Implementations
        public void OnInitializationComplete()
        {
            DebugLog("Init Success");
            _listener.OnAdsInitialized();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            DebugLog($"Init Failed: [{error}]: {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            DebugLog($"Load Success: {placementId}");
            adLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            DebugLog($"Load Failed: [{error}:{placementId}] {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            _listener.OnAdsRewardedFail();
            DebugLog($"OnUnityAdsShowFailure: [{error}]: {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            DebugLog($"OnUnityAdsShowStart: {placementId}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            DebugLog($"OnUnityAdsShowClick: {placementId}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            adLoaded = false;
            if (placementId == "rewardedVideo" && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                _listener.OnAdsRewarded();
            }
            else if (placementId == "video")
            {
                _listener.OnAdsInterstitialShown();
            }
        }
        
        //wrapper around debug.log to allow broadcasting log strings to the UI
        void DebugLog(string msg)
        {
            Debug.Log(msg);
        }

        public void SetListener(IAdsListener listener)
        {
            _listener = listener;
        }
        #endregion
    }
    #endif
}