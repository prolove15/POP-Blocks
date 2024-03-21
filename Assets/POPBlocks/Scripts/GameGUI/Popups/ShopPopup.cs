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

using System.Linq;
using POPBlocks.Scripts.Popups;
using POPBlocks.Scripts.Scriptables;
using UnityEngine;
#if UNITY_INAPP
using POPBlocks.Scripts.Integrations;
using UnityEngine.Purchasing;
#endif

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class ShopPopup : Popup
    {
        public ShopItem[] shopItems;
        private GameSettings gameSettings;
        private ShopSettings shopSettings;
        private RewardPopup instanceRewardPopup;

        private void Start()
        {
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            shopSettings = Resources.Load<ShopSettings>("Settings/ShopSettings");
            var list = shopSettings.shopItems.OrderByDescending(i => i.coins).ToArray();
            for (var i = 0; i < list.Count(); i++)
            {
                var shopSetting = list[i];
                shopItems[i].productID = shopSettings.shopItems.OrderByDescending(shopItemEditor => shopItemEditor.coins).ToArray()[i].productID;
                shopItems[i].coinsCounter.SetValue(shopSetting.coins);
            }
        }

        private void OnEnable()
        {
            #if UNITY_INAPP
            UnityInAppsIntegration.OnPurchaseSucceed += OnPurchased;
            #endif
        }

        private void OnDisable()
        {
#if UNITY_INAPP

            UnityInAppsIntegration.OnPurchaseSucceed -= OnPurchased;
#endif

        }

#if UNITY_INAPP
        void OnPurchased(PurchaseEventArgs args)
        {
            GameManager.Instance.coins.IncrementValue(shopSettings.shopItems.First(i => i.productID == args.purchasedProduct.definition.id).coins);
            Hide();
        }
#endif

        public void StartRewarded()
        {
            instanceRewardPopup = (RewardPopup)PopupManager.Instance.rewardPopup.Show();
            instanceRewardPopup.GetComponent<CanvasGroup>().alpha = 0;
            instanceRewardPopup.SetReward(RewardTypes.Coins);
        }
        
        public void FailedToShowOrClosed()
        {
            if(instanceRewardPopup.GetComponent<CanvasGroup>().alpha==0)
                instanceRewardPopup?.Hide();
        }

        public void OnRewarded()
        {
            GameManager.Instance.coins.IncrementValue(gameSettings.coinsReward);
            instanceRewardPopup.GetComponent<CanvasGroup>().alpha = 1;
            Hide();
        }
    }
}