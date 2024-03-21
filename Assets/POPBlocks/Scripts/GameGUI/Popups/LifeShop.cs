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

using POPBlocks.Scripts.Scriptables;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    class LifeShop : Popup
    {
        private GameSettings gameSettings;
        public TextMeshProUGUI price;
        private RewardPopup instanceRewardPopup;

        private void OnEnable()
        {
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            price.text = gameSettings.refillLifeCost.ToString();
            if(GameManager.Instance.lives.GetValue() == gameSettings.CapOfLife) Hide();
        }

        public void BuyLife()
        {
            if (GameManager.Instance.Purchasing(gameSettings.refillLifeCost))
            {
                GameManager.Instance.lives.RestoreDefault();
                Hide();
            }
        }
        
        public void FailedToShowOrClosed()
        {
            if(instanceRewardPopup.GetComponent<CanvasGroup>().alpha==0)
                instanceRewardPopup?.Hide();  
        }
        
        public void StartRewarded()
        {
            instanceRewardPopup = (RewardPopup)PopupManager.Instance.rewardPopup.Show();
            instanceRewardPopup.GetComponent<CanvasGroup>().alpha = 0;
            instanceRewardPopup.SetReward(RewardTypes.Life);
        }

        public void OnRewarded()
        {
            GameManager.Instance.lives.RestoreDefault();
            instanceRewardPopup.GetComponent<CanvasGroup>().alpha = 1;
            Hide();
        }
    }
}