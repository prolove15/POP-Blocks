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

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace POPBlocks.Scripts.Integrations.Ads
{
    public class ShowAdsByButton : MonoBehaviour
    {
        public UnityEvent OnRewaredeShown;
        public UnityEvent OnRewardedFailed;
        public bool checkRewardedAvailable;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                if (checkRewardedAvailable && GetComponent<Button>().onClick.GetPersistentMethodName(0) == "ShowRewardedAd" && !AdsManager.THIS.IsRewardedAvailable())
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                    StartCoroutine(WaitForAds());
                }
            }
        }

        private IEnumerator WaitForAds()
        {
            yield return new WaitUntil(()=>AdsManager.THIS.IsRewardedAvailable());
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

        private void OnDisable()
        {

        }

        private void OnRewardedShown()
        {
            OnRewaredeShown?.Invoke();
            AdsManager.OnRewardedShownEvent -= OnRewardedShown;
        }

        public void ShowInterstitial(string placement)
        {
            if (PlayerPrefs.GetInt("tutorialShown", 0) == 0) return;
            AdsManager.THIS.ShowInterstitial();
        }

        public void ShowRewardedAd(string placement)
        {
            #if !UNITY_ANDROID && !UNITY_IPHONE
            Debug.LogError("Use a mobile platform to test ads");
            #endif
            AdsManager.OnRewardedShownEvent += OnRewardedShown;
            AdsManager.OnRewardedFailEvent += OnRewardedFail;
            AdsManager.THIS.ShowRewarded();
        }

        private void OnRewardedFail()
        {
            OnRewardedFailed?.Invoke();
            AdsManager.OnRewardedFailEvent -= OnRewardedFail;

        }
    }
}