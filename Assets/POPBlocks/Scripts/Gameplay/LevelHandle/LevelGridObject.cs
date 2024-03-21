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

using POPBlocks.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle
{
    public class LevelGridObject : MonoBehaviour
    {
        public int num;
        public GameObject lockObj;
        public TextMeshProUGUI[] textMeshProUguis;
        public GameObject[] stars;
        public LevelGridManager levelGridManager;

        private void Start()
        {
            textMeshProUguis.ForEachY(i => i.text = num.ToString());
            for (int i = 0; i < 3; i++)
            {
                stars[i].SetActive(false);
            }

            var starsAmount = GameManager.Instance._mapProgressManager.LoadLevelStarsCount(num);
            for (int i = 0; i < starsAmount; i++)
            {
                stars[i].SetActive(true);
            }      
        }
        
        public void Lock()
        {
            lockObj.SetActive(true);
        }

        public void Unlock()
        {
            lockObj.SetActive(false);
        }

        public void OpenLevel()
        {
            levelGridManager.OpenLevel(num);
        }
    }
}