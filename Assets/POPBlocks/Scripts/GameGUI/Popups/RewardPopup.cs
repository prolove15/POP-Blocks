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
using System.Linq;
using POPBlocks.Scripts.GameGUI.BonusSpin;
using POPBlocks.Scripts.Utils;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class RewardPopup : Popup
    {
        public RewardDescription[] rewardIcons;
        public Transform iconPivot;
        public void SetReward(RewardTypes rewardTypes)
        {
            rewardIcons.ForEachY(i => i.gameObject.SetActive(false));
            var rewardDescription = rewardIcons.First(i => i.rewardType == rewardTypes);
            rewardDescription.gameObject.SetActive(true);
        }
        public void SetReward(RewardWheel rewardWheel)
        {
            rewardIcons.ForEachY(i => i.gameObject.SetActive(false));
            var icon = Instantiate(rewardWheel, iconPivot.position, Quaternion.identity, transform);
            icon.transform.localScale *= 3;
            var rewardDescription = rewardWheel.description;
        }
    }

    [Serializable]
    public class RewardDescription
    {
        public RewardTypes rewardType;
        public GameObject gameObject;
        public string description;
    }
}