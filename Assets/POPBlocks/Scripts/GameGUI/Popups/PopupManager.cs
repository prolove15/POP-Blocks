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
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance;
        public Popup win;
        public Popup lose;
        public Popup coins_shop;
        public Popup exit;
        public Popup info;
        public Popup leadboard_menu;
        public Popup life_shop;
        public Popup outOfMoves;
        public Popup play1;
        public Popup play2;
        public Popup boostShop;
        public Popup rewardPopup;
        public Popup mainMenu;
        public Popup preComplete;
        public Popup bonusSpin;
        public Popup dailyReward;
        public Popup noMatches;
        public Popup[] tutorial;
        public bool noFade;
        public Image fader;
        public RectTransform unmask;
        private string lastPopup;
        private DebugSettings debugSettings;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            debugSettings = Resources.Load<DebugSettings>("Settings/DebugSettings");
            ShowDailyReward();
        }
        private void OnEnable()
        {
            Popup.OnShowPopup += OnShowPopup;
            // Popup.OnHidePopup += OnHidePopup;
        }
        
        private void OnDisable()
        {
            Popup.OnShowPopup -= OnShowPopup;
            // Popup.OnHidePopup -= OnHidePopup;
        }

        private void OnHidePopup(string popupname)
        {
            
        }

        private void OnShowPopup(string popupname)
        {
            lastPopup = popupname;
        }

        public void FadeIn()
        {
            if (!noFade)
            {
                fader.gameObject.SetActive(true);
                fader.DOFade(0.9f, 1);
            }
        }

        public void FadeOut(string popupName = "", TweenCallback callback = null)
        {
            if (popupName == "") popupName = lastPopup;
            if (!noFade /*&& popupName == lastPopup*/)
                fader.DOFade(0f, 1).OnComplete(()=>
                {
                    callback?.Invoke();
                });
        }

        private void Update()
        {
            CheckMainMenu();
            if (Input.GetKeyUp(debugSettings.Back))
            {
                var find = transform.Find(lastPopup);
                if (!(find is null)) find.GetComponent<Popup>().BackButton();
                else if (LevelManager.Instance.GameState == GameState.Playing)
                    exit.Show();
            }
        }

        public void Show(string menuName)
        {
            GetAttribute<Popup>(menuName).Show();
        }

        public T GetAttribute<T>(string _name)
        {
            return (T) typeof(PopupManager).GetField(_name).GetValue(this);
        }

        private void CheckMainMenu()
        {
            if (SceneManager.GetActiveScene().name == "main")
            {
                if (transform.childCount <= 1 && !mainMenu.IsActive()) mainMenu.Show();
                else if (IsAnyPopupOpen() && mainMenu.IsActive()) mainMenu.GetObject().Hide();
            }
        }

        public bool IsAnyPopupOpen()
        {
            return transform.childCount > 2;
        }

        private void ShowDailyReward()
        {
            if (!ServerTime.THIS.dateReceived)
            {
                ServerTime.OnDateReceived += ShowDailyReward;
            }

            var DateReward = PlayerPrefs.GetString("DateReward", default(DateTime).ToString());
            var dateTimeReward = DateTime.Parse(DateReward);
            DateTime testDate = ServerTime.THIS.serverTime;

            if (SceneManager.GetActiveScene().name == "main")
            {
                if (DateReward == "" || DateReward == default(DateTime).ToString())
                    PopupManager.Instance.dailyReward.Show(false,CheckMainMenu);
                else
                {
                    var timePassedDaily = testDate.Subtract(dateTimeReward).TotalDays;
                    if (timePassedDaily >= 1)
                        PopupManager.Instance.dailyReward.Show(false,CheckMainMenu);
                    else PopupManager.Instance.mainMenu.Show();
                }
            }
        }

        public void HideMask()
        {
            unmask.gameObject.SetActive(false);
        }

        public void SetUnMask(Rect rect)
        {
            unmask.gameObject.SetActive(true);
            unmask.anchoredPosition = rect.min;
            unmask.sizeDelta = rect.size;
        }
    }
}