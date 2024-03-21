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

using POPBlocks.Scripts.GameGUI.BonusSpin;
using POPBlocks.Scripts.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class MainMenu : Popup
    {
        public BonusSpinButton bonusSpinButton;

        private void OnEnable()
        {
            bonusSpinButton.spin = this.gameObject;
            bonusSpinButton.CheckForSpin();
        }

        public void StartGame()
        {
            var gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            switch (gameSettings.mapType)
            {
                case MapTypes.NoMap:
                    GameManager.Instance._mapProgressManager.CurrentLevel = GameManager.Instance._mapProgressManager.GetLastLevel();
                    PopupManager.Instance.play1.Show();
                    break;
                case MapTypes.GridLevels:
                    SceneManager.LoadScene("grid");
                    break;
                case MapTypes.ScrollingsMap:
                    if(gameSettings.afterLevel >= GameManager.Instance._mapProgressManager.CurrentLevel)
                        SceneManager.LoadScene("game");
                    else SceneManager.LoadScene("map");
                    break;
            }
        }
    }
}