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

using POPBlocks.Scripts.Boosts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI.BonusSpin
{
    /// <summary>
    /// Reward icon for the Reward popup
    /// </summary>
    public class RewardIcon : MonoBehaviour
    {
        public Sprite[] sprites;
        public Image icon;
        public Transform iconHolder;
        public TextMeshProUGUI text;
        public TextMeshProUGUI rewardName;

        /// <summary>
        /// Sets Wheel reward
        /// </summary>
        /// <param name="reward">reward object</param>
        public void SetWheelReward(RewardWheel reward)
        {
            foreach (Transform item in iconHolder)
            {
                Destroy(item.gameObject);
            }
            var g = Instantiate(reward.gameObject, Vector2.zero, Quaternion.identity, iconHolder);
            g.transform.localPosition = Vector3.zero;
            g.transform.localScale = Vector3.one * 2;
            icon = g.GetComponent<Image>();
            if (reward.type == BoostType.None)
            {
                text.text = "You got coins";
                rewardName.text = reward.GetDescription();
            }
            else
            {
                text.text = "You got the boost";
                rewardName.text = reward.GetDescription();
            }

        }

        /// <summary>
        /// Set icon
        /// </summary>
        /// <param name="i"></param>
        public void SetIconSprite(int i)
        {
            icon.sprite = sprites[i];
            if (i == 0)
            {
                text.text = "You got coins";
                rewardName.text = "Coins";
            }
            else if (i == 1)
            {
                text.text = "You got life";
                rewardName.text ="Life";
            }
        }
    }
}
