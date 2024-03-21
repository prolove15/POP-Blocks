// // ©2015 - 2021 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using DG.Tweening;
using POPBlocks.MapScripts;
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Server.Network
{
    public class PlayerAvatar : MonoBehaviour, IAvatarLoader
    {
        public Image image;

        void Start()
        {
            // image.enabled = false;
        }

        void OnEnable () {
            LevelsMap.LevelReached += OnLevelReached;
#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.OnPlayerPictureLoaded += ShowPicture;
            if(NetworkManager.profilePic != null) ShowPicture();
#endif
            Invoke(nameof(Animate), 2);
        }

        private void Animate()
        {
            transform.DOLocalMoveY(transform.localPosition.y + 0.1f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        void OnDisable () {
#if PLAYFAB || GAMESPARKS || EPSILON
            NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
            LevelsMap.LevelReached -= OnLevelReached;
#endif

        }


        public void ShowPicture()
        {
            image.sprite = NetworkManager.profilePic;
            image.enabled = true;
        }

        private void OnLevelReached(object sender, LevelReachedEventArgs e)
        {
            Debug.Log(string.Format("Level {0} reached.", e.Number));
        }

    }
}