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

using POPBlocks.MapScripts;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle
{
    public class LevelGridManager : Levels
    {
        public GameObject[] arrows;
        public LevelGridObject levelGridPrefab;
        private int screen = 0;
        private int num;
        private int lastLevel;
        public int levelsOnScreen = 20;
        private void Start()
        {
            num = Resources.LoadAll<Level>("Levels").Length;
            lastLevel = GameManager.Instance._mapProgressManager.GetLastLevel();
            SwitchScreen();
        }

        private void SwitchScreen()
        {
            foreach (Transform level in transform)
            {
                Destroy(level.gameObject);
            }
            int from = screen * levelsOnScreen + 1;
            int to = screen * levelsOnScreen + levelsOnScreen;
            to = Mathf.Clamp(to, 0, num);
            for (int i = from; i <= to; i++)
            {
                var level = Instantiate(levelGridPrefab, transform);
                level.num = i;
                level.levelGridManager = this;
                if (i <= lastLevel) level.Unlock();
                else level.Lock();
            }
        }

        public void Next()
        {
            screen++;
            screen = Mathf.Clamp(screen, 0, num/levelsOnScreen);
            SwitchScreen();
        }
        
        public void Prev()
        {
            screen--;
            screen = Mathf.Clamp(screen, 0, num/levelsOnScreen);
            SwitchScreen();
        }

        private void Update()
        {
            if(screen == 0) arrows[0].SetActive(false);
            else arrows[0].SetActive(true);
            if(screen >= num/levelsOnScreen) arrows[1].SetActive(false);
            else arrows[1].SetActive(true);

        }

        public void OpenLevel(int number)
        {
            LevelSelected_(null, number);
        }
    }
}