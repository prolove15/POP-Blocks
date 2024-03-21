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
    public class IronsourceAdsHandler : IAdsHandler
    {
        private IAdsListener _listener;
        public IronsourceAdsHandler(string _id)
        {
            #if IRONSOURCE
            
            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(_id);
            
          
            #endif
        }
#if IRONSOURCE

        private void Rewardeded(IronSourcePlacement obj)
        {
            Debug.Log("Rewardeded");
            _listener?.OnAdsRewarded();
        }
        private void SdkInitializationCompletedEvent()
        {
            Debug.Log("Ironsource SdkInitializationCompletedEvent");
            _listener?.OnAdsInitialized();
        }

        private void InterstitialAdLoadFailedEvent(IronSourceError obj)
        {
            Debug.Log("InterstitialAdLoadFailedEvent " + obj.getCode() + " " + obj.getDescription());
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
        {
            Debug.Log("RewardedVideoAdShowFailedEvent " + obj.getCode() + " " + obj.getDescription());
            _listener?.OnAdsRewardedFail();
        }

#endif
        public void ShowAdsInterstitial()
        {
#if IRONSOURCE
            IronSource.Agent.showInterstitial();
#endif
        }

        public bool IsAvailable()
        {
#if IRONSOURCE
            return IronSource.Agent.isInterstitialReady();
#endif
            return false;
        }

        public void ShowRewardedAds()
        {
#if IRONSOURCE
            IronSource.Agent.showRewardedVideo();
#endif
        }

        public void LoadInterstitial()
        {
#if IRONSOURCE
            IronSource.Agent.loadInterstitial();
#endif
        }

        public void LoadRewardedAds()
        {
 #if IRONSOURCE
            IronSource.Agent.loadRewardedVideo();
 #endif
        }

        public void SetListener(IAdsListener listener)
        {
            _listener = listener;
            #if IRONSOURCE
            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdRewardedEvent += Rewardeded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            
            //Add Interstitial Events
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += listener.OnAdsInterstitialShown;
            #endif
        }
    }
}