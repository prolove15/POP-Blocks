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
#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace POPBlocks.Scripts.Integrations.Ads
{
    public class AdmobAdsHandler: IAdsHandler
    {
        private readonly AdmobController admobController;

#if ADMOB
        private InterstitialAd interstitial;
        private RewardedAd rewardedAd;
        private AdRequest request;
#endif
        public AdmobAdsHandler(string _id, AdmobController controller)
        {
            admobController = controller;
            
        }

        public void ShowAdsInterstitial()
        {
            #if ADMOB
            admobController.ShowInterstitialAd();
            #endif
        }

        
        public bool IsAvailable()
        {
            #if ADMOB
            return admobController.interstitialAd.CanShowAd();
            #endif
            return false;
        }

        public void ShowRewardedAds()
        {
            #if ADMOB
            admobController.ShowRewardedAd();
            #endif
        }

        public void LoadInterstitial()
        {
            #if ADMOB
            admobController.RequestAndLoadInterstitialAd();
            #endif
        }

        public void LoadRewardedAds()
        {
            #if ADMOB
            admobController.RequestAndLoadRewardedAd();
            #endif
        }
        
        public void SetListener(IAdsListener listener)
        {
            #if ADMOB
            admobController.RequestAndLoadInterstitialAd();
            admobController.RequestAndLoadRewardedAd();
            admobController.OnUserEarnedRewardEvent += listener.OnAdsRewarded;
            admobController.OnAdFailedToLoadEvent += listener.OnAdsRewardedFail;
            admobController.OnAdClosedEvent += listener.OnAdsRewardedFail;
            admobController.OnAdFailedToShowEvent += listener.OnAdsRewardedFail;
            admobController.OnInterstitialShownEvent += listener.OnAdsInterstitialShown;
            #endif
        }
    }
}