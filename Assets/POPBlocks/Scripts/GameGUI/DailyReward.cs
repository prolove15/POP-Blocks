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
using POPBlocks.Scripts.GameGUI.Popups;
using POPBlocks.Scripts.Popups;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    /// <summary>
    /// Daily reward popup
    /// </summary>
    public class DailyReward : Popup
    {
        public DayReward[] days;
        public TextMeshProUGUI description;
        int currentDay;
        void OnEnable()
        {
            if (ServerTime.THIS.dateReceived)
                CheckDaily();
            else 
                ServerTime.OnDateReceived += CheckDaily;
            var count = int.Parse(days[currentDay].count.text);
            description.text = "You got " + count + " coins";
        }

        private void CheckDaily()
        {
            var previousDay = PlayerPrefs.GetInt("LatestDay", -1);
            var DateReward = PlayerPrefs.GetString("DateReward", ServerTime.THIS.serverTime.ToString());
            var timePassedDaily = (int) DateTime.Parse(DateReward).DayOfWeek;
            
            /*if (timePassedDaily > ((int) ServerTime.THIS.serverTime.DayOfWeek + 1) % 7 || previousDay == 6)
            {
                previousDay = -1;
            }*/
            
            if (previousDay == 6)
            {
                previousDay = -1;
            }

            for (var day = 0; day < days.Length; day++)
            {
                if (day <= previousDay)
                    days[day].SetPassedDay();
                if (day == previousDay + 1)
                {
                    days[day].SetCurrentDay();
                    currentDay = day;
                }

                if (day > previousDay + 1)
                    days[day].SetDayAhead();
            }
        }

        public void Ok()
        {
            PlayerPrefs.SetInt("LatestDay", currentDay);
            PlayerPrefs.SetString("DateReward", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.Save();
            var count = int.Parse(days[currentDay].count.text);
            GameManager.Instance.coins.IncrementValue(count);
            description.text = "You got " + count + " coins";
            Hide();
        }

        private void OnDisable()
        {        
            ServerTime.OnDateReceived -= CheckDaily;

        }
    }
}
