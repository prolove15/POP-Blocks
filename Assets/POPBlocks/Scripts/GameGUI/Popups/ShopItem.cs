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

#if UNITY_INAPP
using POPBlocks.Scripts.Integrations;
#endif
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class ShopItem : MonoBehaviour
    {
        public string productID;
        public GUICounter coinsCounter;
        public TextMeshProUGUI prices;

        public void Buy()
        {
#if UNITY_INAPP
            UnityInAppsIntegration.THIS.BuyProductID(productID);
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.cash);
#endif
        }

        private void Update()
        {
            #if UNITY_INAPP
            if (UnityInAppsIntegration.THIS.m_StoreController.products.WithID(productID).metadata.localizedPrice > new decimal(0.01))
                prices.text = "" + UnityInAppsIntegration.THIS.m_StoreController.products.WithID(productID).metadata.localizedPriceString;
            #endif
        }
    }
}