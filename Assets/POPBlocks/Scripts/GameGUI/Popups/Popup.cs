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
using DG.Tweening;
using POPBlocks.Scripts.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class Popup : MonoBehaviour
    {
        public Animator animator;
        public bool noFade;
        public Button backButton;
        [SerializeField]
        private bool wooshSound;
        private PopupManager popupManager => PopupManager.Instance;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }

        public delegate void ShowEvent(string popupName);

        public static event ShowEvent OnShowPopup;
        public static event ShowEvent OnHidePopup;

        public Popup Show(bool createNewAnyway = false, TweenCallback onHide = null)
        {
            if (!noFade)
                popupManager.FadeIn();
            Popup popup = this;
            if(popup.IsPrefab())
                popup = GetPopup(createNewAnyway);
            if (!popup.gameObject.activeSelf) popup.gameObject.SetActive(true);
            popup.name = name;
            popup.OnHide = onHide;
            ShowBeforeAnimation();
            popup.ShowAnimation();
            OnShowPopup?.Invoke(popup.name);
            return popup;
        }

        public Popup GetPopup(bool createNewAnyway = false)
        {
            GameObject window = popupManager.transform.Find(name)?.gameObject;
            if (window == null || createNewAnyway) window = Instantiate(gameObject, popupManager.transform);
            // window.SetActive(false);
            return window.GetComponent<Popup>();
        }

        private bool IsPrefab()
        {
            return gameObject.scene.name == null;
        }

        public void ShowAnimation()
        {
            if (animator != null && animator.parameters.Any(i => i.name == "show"))
            {
                animator.SetTrigger("show");
                Invoke(nameof(AfterShowAnimation), 1f);
            }
        }

        public void WooshSound()
        {
            if(wooshSound)
                DOVirtual.DelayedCall(0.1f, ()=>SoundBase.Instance.PlayOneShot(SoundBase.Instance.woosh));
        }
        
        protected virtual void ShowBeforeAnimation()
        {
            
        }

        protected virtual void AfterShowAnimation()
        {
        }


        public event TweenCallback OnHide;

        public virtual void BackButton()
        {
            backButton?.onClick?.Invoke();
        }

        public void Hide()
        {
            popupManager.HideMask();
            OnHidePopup?.Invoke(name);
            HideAnimation();
        }

        public bool IsActive() => popupManager.transform.Find(name);
        public Popup GetObject() => popupManager.transform.Find(name).GetComponent<Popup>();

        private void HideAnimation()
        {
            if (animator != null && animator.parameters.Any(i => i.name == "hide"))
                animator.SetTrigger("hide");
            if (!noFade && FindObjectsOfType<Popup>().Length <= 1)
                popupManager.FadeOut(name);
            Destroy(gameObject, 1);
        }
        
        public void OnHideEvent()
        {
            OnHide?.Invoke();
            Destroy(gameObject);
        }

        public void OpenScene(string scene)
        {
            OnHide += () =>
            {
                var lifeAvailable = GameManager.Instance.CheckLife();
                if (lifeAvailable && scene == "game")
                    SceneManager.LoadScene(scene);
                else if(scene != "game")
                    SceneManager.LoadScene(scene);
            };
            Hide();
        }

        public void RestartLevel()
        {
            LevelManager.Instance.RestartLevel();
        }

        public void Show(string popupName)
        {
            OnHide += () => PopupManager.Instance.Show(popupName);
            Hide();
        }


        public void QuitLevel()
        {
            var settings = Resources.Load<GameSettings>("Settings/GameSettings");
            switch (settings.mapType)
            {
                case MapTypes.GridLevels:
                    OpenScene("grid");
                    break;
                case MapTypes.ScrollingsMap:
                    OpenScene("map");
                    break;
                case MapTypes.NoMap:
                    OpenScene("main");
                    break;
            }
        }

        public void SpendLife(bool checkLife)
        {
            GameManager.Instance.SpendLife(1,checkLife);
        }
    }
}