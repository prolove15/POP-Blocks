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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI
{
    /// <summary>
    /// Class for any day reward
    /// </summary>
    public class DayReward : MonoBehaviour
    {
        public Image check;
        public Image image;
        public Sprite CurrentDay;
        public Sprite AheadDay;
        public Sprite PassedDay;
        public TextMeshProUGUI count;
        public TextMeshProUGUI textday;
        public Color colorCurrent;
        public Color colorPassed;
        public Color colorAhead;
        public int day;

        private void OnEnable()
        {
            // textday.text = "DAY";
        }

        public void SetDayAhead()
        {
            image.sprite = AheadDay;
            count.color = colorAhead;
            check.enabled = false;
        }

        public void SetCurrentDay()
        {
            image.sprite = CurrentDay;
            count.color = colorCurrent;
            check.enabled = false;
        }

        public void SetPassedDay()
        {
            image.sprite = PassedDay;
            count.color = colorPassed;
            check.enabled = true;
        }
    }
}