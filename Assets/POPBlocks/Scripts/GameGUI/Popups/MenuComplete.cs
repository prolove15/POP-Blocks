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

using System.Collections;
using POPBlocks.Scripts.Scriptables;
using UnityEngine;
#if EPSILON || PLAYFAB
using POPBlocks.Server.Network;
using POPBlocks.Server.Network.Leadboard;
#endif

namespace POPBlocks.Scripts.GameGUI.Popups
{
    class MenuComplete : Popup
    {
        public GameObject[] stars;
        public Animator character;
        #if EPSILON || PLAYFAB
        public LeadboardManager LeadboardManager;
        #endif
        private void OnEnable()
        {
            if (Time.timeScale == 100) Time.timeScale = 1;

            for (int i = 0; i < 3; i++)
            {
                stars[i].SetActive(false);
            }

            StartCoroutine(ShowStars());
        }
        
        protected override void AfterShowAnimation()
        {
            ShowLeadboard();
            base.AfterShowAnimation();
        }
        
        void ShowLeadboard()
        {
            #if EPSILON || PLAYFAB
            LeadboardManager.level = GameManager.Instance._mapProgressManager.CurrentLevel;
            LeadboardManager.gameObject.SetActive(true);
            #endif
        }

        public void LoginOnComplete()
        {
            #if EPSILON
            NetworkManager.dataManager.DownloadPlayerData();
            #endif
        }

        private IEnumerator ShowStars()
        {
            var starsAmount = LevelManager.Instance.level.stars.GetEarnedStars(LevelManager.Instance.score.GetValue());
            for (int i = 0; i < starsAmount; i++)
            {
                yield return new WaitForSeconds(0.5f);
                stars[i].SetActive(true);
            }

            // if(starsAmount == 3) Invoke(nameof(AnimateCharacter), 1);
        }

        void AnimateCharacter()
        {
            character.SetTrigger("happy");
        }

        public void Home()
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

        public void Next()
        {
            if (LevelManager.Instance.gameSettings.goMap && LevelManager.Instance.gameSettings.afterLevel <= GameManager.Instance._mapProgressManager.CurrentLevel)
            {
                GameManager.Instance.openNextLevel = true;
                GameManager.Instance._mapProgressManager.CurrentLevel++;
                Home();
            }
            else
            {
                GameManager.Instance.openNextLevel = true;
                GameManager.Instance._mapProgressManager.CurrentLevel++;
                OpenScene("game");
            }
        }
    }
}