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
using System.Collections.Generic;
using POPBlocks.MapScripts;
using POPBlocks.Scripts.Boosts;
using POPBlocks.Scripts.GameGUI;
using POPBlocks.Scripts.GameGUI.Popups;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Popups;
using POPBlocks.Scripts.Scriptables;
using POPBlocks.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace POPBlocks.Scripts
{
    /// <summary>
    /// basic game functions and variables called from main and game scenes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        //coins GUI element
        public GUICounter coins;
        //lives GUI element
        public GUICounter lives;
        //instance
        public static GameManager Instance;
        //boosts using in main scene and game scene
        public List<BoostType> boostTypes = new List<BoostType>();
        public List<BoostType> boostTypesGame = new List<BoostType>();
        //date of exit from the game for cheat protection purpose
        public static string DateOfExit { get; set; }
        //time is rest for refill life
        public float RestLifeTimer;
        //level num should be restarted
        public int restartedLevel;
        public bool win;
        public bool openNextLevel;
        private int totalLevels;
        public PlayerPrefsMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager ();
        [HideInInspector]public GameSettings gameSettings;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(this);
            if(PlayerPrefs.GetInt("SessionCounter") <= 1 && SceneManager.GetActiveScene().name=="main") FirstTimePlay();
            DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
            RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
            if (DateOfExit == "" || DateOfExit == default(DateTime).ToString())
                DateOfExit = ServerTime.THIS.serverTime.ToString();
            if (totalLevels == 0)
                totalLevels = Resources.LoadAll<Level>("Levels").Length;
            if (SceneManager.GetActiveScene().name == "main")
            {
                PlayerPrefs.SetInt("OpenLevelTest", 0);
                PlayerPrefs.Save();
            }
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

        }

        /// <summary>
        /// Function called on the first game session
        /// </summary>
        private void FirstTimePlay()
        {
            var boosts= Resources.Load<BoostSettings>("Settings/BoostSettings").boosts;
            foreach (var boost in boosts)
            {
                PlayerPrefs.SetInt(boost.boostType.ToString(), boost.startCount);
            }
            PlayerPrefs.SetInt("NotFirstTime",1);
            PlayerPrefs.Save();
        }
        
        private void OnEnable()
        {
            Levels.LevelSelected += OnLevelClicked;
            LevelsMap.OnLevelReached += OnLevelReached;
        }

        private void OnDisable()
        {
            Levels.LevelSelected -= OnLevelClicked;
            LevelsMap.OnLevelReached -= OnLevelReached;
        }


        // Event handler for when a level is clicked on the map
        private void OnLevelClicked(object sender, LevelReachedEventArgs e)
        {
            if (CanvasExtensions.IsPointerOverUIObject() && SceneManager.GetActiveScene().name != "grid")
                return;
            if (!PopupManager.Instance.IsAnyPopupOpen())
            {
                SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
                OpenMenuPlay(e.Number);
            }
        }

        // Opens the menu for the selected level
        private void OpenMenuPlay(int number)
        {
            GameManager.Instance._mapProgressManager.CurrentLevel = number;
            openNextLevel = false;
            PopupManager.Instance.play1.Show();
        }

        // Event handler for when a level is reached
        void OnLevelReached()
        {
            var num = _mapProgressManager.GetLastLevel();
            if (openNextLevel && totalLevels >= num && gameSettings.openMenuPlay)
            {
                OpenMenuPlay(num);
            }
        }

        /// <summary>
        /// Purchasing initiated
        /// </summary>
        /// <param name="cost">coins to spend</param>
        /// <returns>true if coins is enough</returns>
        public bool Purchasing(int cost)
        {
            if (GetCoins() >= cost)
            {
                coins.DecrementValue(cost);
                return true;
            }
            PopupManager.Instance.coins_shop.Show();
            return false;
        }

        /// <summary>
        /// Consuming life
        /// </summary>
        /// <param name="lifeCost">life to consume</param>
        /// <param name="checkLife"></param>
        /// <returns>true if lives is enough</returns>
        public bool SpendLife(int lifeCost, bool checkLife)
        {
            if (GetLife() >= lifeCost)
            {
                lives.DecrementValue(lifeCost);
                return true;
            }
            if (!checkLife) return true;
            PopupManager.Instance.life_shop.Show();
            return false;
        }
        
        /// <summary>
        /// Check lives is more than 0
        /// </summary>
        /// <returns>true if lives > 0 </returns>
        public bool CheckLife()
        {
            if (GetLife() > 0)
                return true;
            PopupManager.Instance.life_shop.Show();
            return false;
        }

        /// <summary>
        /// Getting coins amount from variable or from PlayerPrefs
        /// </summary>
        /// <returns>amount of coins</returns>
        private int GetCoins()
        {
            return coins != null ? coins.GetValue() : PlayerPrefs.GetInt("coins");
        }
        
        /// <summary>
        /// Getting lives amount from variable or from PlayerPrefs
        /// </summary>
        /// <returns>amount of lives</returns>
        private int GetLife()
        {
            return lives != null ? lives.GetValue() : PlayerPrefs.GetInt("life");
        }
        
        /// <summary>
        /// Called on hiding the game on a device
        /// </summary>
        /// <param name="pauseStatus">true if the game was hide</param>
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (RestLifeTimer > 0)
                {
                    PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
                }

                PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
                PlayerPrefs.Save();
            }
        }
        
        /// <summary>
        /// Called on game quit
        /// </summary>
        void OnApplicationQuit()
        {
            //1.4  added 
            if (RestLifeTimer > 0)
            {
                PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            }

            PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.Save();
            //print(RestLifeTimer)
        }

        public bool IsLifeFull()
        {
            return GameManager.Instance.lives.GetValue() == gameSettings.CapOfLife;
        }

    }
}