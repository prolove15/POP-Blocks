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
using System.Collections;
using System.Linq;
using POPBlocks.Scripts.GameGUI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace POPBlocks.Scripts.LevelHandle
{
    public class Progressbar : MonoBehaviour
    {
        public GUIStar[] stars;
        private Slider slider;
        private StarsArray levelStars;
        bool[] starsAwarded = new bool[3];
        public GameObject animationPrefab;


        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            LevelManager.OnLevelLoaded += InitBar;

        }

        private void OnDisable()
        {
            LevelManager.OnLevelLoaded -= InitBar;
            LevelManager.Instance.score.OnChangeValue -= ScoreChange;

        }

        private void InitBar()
        {
            LevelManager.Instance.score.OnChangeValue += ScoreChange;
            levelStars = LevelManager.Instance.level.stars;
            slider.maxValue = levelStars.Last();
            PrepareStars();
        }

        private void ScoreChange(int score)
        {
            slider.value = score;
            var list = levelStars.Where(i => i <= score).ToArray();
            for (var i = 0; i < list.Length; i++)
            {
                CheckAnim(i);
            }
        }

        void PrepareStars()
        {
            var width = GetComponent<RectTransform>().rect.width;
            stars[0].transform.localPosition = new Vector3(levelStars[0] * 100f / levelStars[2] * width / 100 - (width / 2f),
                stars[0].transform.localPosition.y, 0);
            stars[1].transform.localPosition = new Vector3(levelStars[1] * 100f / levelStars[2] * width / 100 - (width / 2f),
                stars[1].transform.localPosition.y, 0);
            foreach (var star in stars)
            {
                star.Hide();
            }
        }
        
        private void CheckAnim(int i)
        {
            if (!stars[i].isShown() && !starsAwarded[i])
            {
                StartAnim(i);
            }
        }

        private void StartAnim(int i)
        {
            var starAnim = Instantiate(animationPrefab);
            starAnim.transform.position = LevelManager.Instance.lastTouchPosition;
            starAnim.GetComponent<StarAnimation>().targetPos = stars[i].transform.position;
            starsAwarded[i] = true;
            var playableDirector = starAnim.GetComponent<PlayableDirector>();
            playableDirector.Play();
            StartCoroutine(WaitFor(() => playableDirector.time >= 1.1f, () => { stars[i].Show(); }));
            StartCoroutine(WaitFor(() => playableDirector.time >= playableDirector.duration, () => {  Destroy(starAnim); }));
        }
        
        IEnumerator WaitFor(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback?.Invoke();
        }

        public int GetStarsEarned()
        {
            return stars.Count(i => i.gameObject.activeSelf);
        }
    }
}