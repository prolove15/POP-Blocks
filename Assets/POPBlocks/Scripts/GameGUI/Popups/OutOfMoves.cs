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

using POPBlocks.Scripts.Scriptables;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class OutOfMoves: Popup
    {
        public GUICounter coinsPriceGUI;
        private GameSettings gameSettings;

        private void Start()
        {
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            coinsPriceGUI.SetValue(gameSettings.continuePrice);
        }

        public void BuyContinue()
        {
            if (GameManager.Instance.Purchasing(gameSettings.continuePrice))
            {
                Continue();
            }

        }

        private void Continue()
        {
            LevelManager.Instance.moves.IncrementValue(gameSettings.movesContinue);
            LevelManager.Instance.GameState = GameState.Playing;
            Hide();
        }

        public void CancelContinue()
        {
            
            OnHide += () =>LevelManager.Instance.GameState = GameState.GameOver;
            Hide();
        }
        
        public void OnRewarded()
        {
            Continue();
        }

        public override void BackButton()
        {
            CancelContinue();
            base.BackButton();
        }
    }
}