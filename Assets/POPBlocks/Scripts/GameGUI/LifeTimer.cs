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
using POPBlocks.Scripts.Scriptables;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    /// <summary>
    /// Life time counter on the map
    /// </summary>
    public class LifeTimer : MonoBehaviour
    {
        TextMeshProUGUI text;
        static float TimeLeft;
        float TotalTimeForRestLife = 15f * 60;  //8 minutes for restore life
        bool startTimer;
        DateTime templateTime;

        private GameSettings gameSettings;
        public string value;

        // Use this for initialization
        void Start()
        {
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            text = GetComponent<TextMeshProUGUI>();
            TotalTimeForRestLife = gameSettings.TotalTimeForRestLifeHours * 60 * 60 + gameSettings.TotalTimeForRestLifeMin * 60 + gameSettings.TotalTimeForRestLifeSec;
        }

        bool CheckPassedTime()
        {
            //print(GameManager.DateOfExit);
            if (GameManager.DateOfExit == "" || GameManager.DateOfExit == default(DateTime).ToString())
                GameManager.DateOfExit = ServerTime.THIS.serverTime.ToString();

            var dateOfExit = DateTime.Parse(GameManager.DateOfExit);
            if (ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds > TotalTimeForRestLife * (gameSettings.CapOfLife - GameManager.Instance.lives.GetValue()))
            {
                //Debug.Log(dateOfExit + " " + GameManager.today);
                GameManager.Instance.lives.RestoreDefault();
                GameManager.Instance.RestLifeTimer = 0;
                return false;    ///we dont need lifes
            }

            TimeCount((float)ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds);
            //Debug.Log((float)ServerTime.THIS.serverTime.Subtract(dateOfExit).TotalSeconds + " " + dateOfExit + " " + ServerTime.THIS.serverTime);
            return true;     ///we need lifes
        }

        void TimeCount(float tick)
        {
            if (GameManager.Instance.RestLifeTimer <= 0)
                ResetTimer();

            GameManager.Instance.RestLifeTimer -= tick;
            if (GameManager.Instance.RestLifeTimer <= 1 && GameManager.Instance.lives.GetValue() < gameSettings.CapOfLife)
            {
                GameManager.Instance.lives.IncrementValue(1);
                ResetTimer();
            }
            //		}
        }

        void ResetTimer()
        {
            GameManager.Instance.RestLifeTimer = TotalTimeForRestLife;
        }

        // Update is called once per frame
        void Update()
        {
            if (!startTimer && ServerTime.THIS.dateReceived && ServerTime.THIS.serverTime.Subtract(ServerTime.THIS.serverTime).Days == 0)
            {
                if (GameManager.Instance.lives.GetValue() < gameSettings.CapOfLife)
                {
                    if (CheckPassedTime())
                        startTimer = true;
                    //	StartCoroutine(TimeCount());
                }
            }

            if (startTimer)
                TimeCount(Time.deltaTime);

            // if (gameObject.activeSelf)
            {
                if (GameManager.Instance.lives.GetValue() < gameSettings.CapOfLife)
                {
                    if (gameSettings.TotalTimeForRestLifeHours > 0)
                    {
                        var hours = Mathf.FloorToInt(GameManager.Instance.RestLifeTimer / 3600);
                        var minutes = Mathf.FloorToInt((GameManager.Instance.RestLifeTimer - hours * 3600) / 60);
                        var seconds = Mathf.FloorToInt((GameManager.Instance.RestLifeTimer - hours * 3600) - minutes * 60);

                        text.enabled = true;
                        value = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                    }
                    else
                    {
                        var minutes = Mathf.FloorToInt(GameManager.Instance.RestLifeTimer / 60F);
                        var seconds = Mathf.FloorToInt(GameManager.Instance.RestLifeTimer - minutes * 60);

                        // text.enabled = true;
                        value = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

                    }

                    //				//	text.text = "+1 in \n " + Mathf.FloorToInt( MainMenu.RestLifeTimer/60f) + ":" + Mathf.RoundToInt( (MainMenu.RestLifeTimer/60f - Mathf.FloorToInt( MainMenu.RestLifeTimer/60f))*60f);
                }
                else
                {
                    value = "Full";
                }
                // text.text = value;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                GameManager.DateOfExit = ServerTime.THIS.serverTime.ToString();
                PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
                PlayerPrefs.SetFloat("RestLifeTimer", GameManager.Instance.RestLifeTimer);

                PlayerPrefs.Save();
            }
            else
            {
                startTimer = false;
                // GameManager.today = ServerTime.THIS.serverTime;
                //		MainMenu.DateOfExit = PlayerPrefs.GetString("DateOfExit");
            }
        }
        

        void OnEnable()
        {
            startTimer = false;
        }

        private void OnDisable()
        {
            GameManager.DateOfExit = ServerTime.THIS.serverTime.ToString();
        }

        void OnApplicationQuit()  //1.4  
        {
            GameManager.DateOfExit = ServerTime.THIS.serverTime.ToString();
            PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.SetFloat("RestLifeTimer", GameManager.Instance.RestLifeTimer);

            PlayerPrefs.Save();
        }
    }
}
