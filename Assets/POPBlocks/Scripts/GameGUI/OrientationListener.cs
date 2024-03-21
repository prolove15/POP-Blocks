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

using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    /// <summary>
    /// Orientation listener. 
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationListener : MonoBehaviour
    {
        public delegate void OrientationChanged(ScreenOrientation orientation);
        public static event OrientationChanged OnOrientationChanged;

        private float previousAspect;
        public static ScreenOrientation previousOrientation;

        void Update()
        {
            // #if UNITY_EDITOR 
            var v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            v = new Vector3(Screen.width, Screen.height, 0);
            var aspect = Screen.height / (float)Screen.width;
            // #endif
            // if (Application.isPlaying)
            // {
            //     if (LevelManager.This.gameStatus == GameState.WaitForPopup) return;
            // }
            if (( v.x > v.y) && (ScreenOrientation.LandscapeLeft != previousOrientation) || aspect != previousAspect)
            {
                SetOrientation(ScreenOrientation.LandscapeLeft);
            }

            else if (( v.x > v.y) && (ScreenOrientation.LandscapeLeft != previousOrientation) || aspect != previousAspect)
            {
                SetOrientation(ScreenOrientation.LandscapeLeft);
            }

            else if (( v.x < v.y) && (ScreenOrientation.Portrait != previousOrientation) || aspect != previousAspect)
            {
                SetOrientation(ScreenOrientation.Portrait);
            }

            else if (( v.x < v.y) && (ScreenOrientation.Portrait != previousOrientation) || aspect != previousAspect)
            {
                SetOrientation(ScreenOrientation.Portrait);
            }

        }

        private void SetOrientation(ScreenOrientation orientation)
        {
            previousAspect = Screen.height / (float)Screen.width;
            previousOrientation = orientation;
            if (OnOrientationChanged != null)
                OnOrientationChanged(orientation);
        }
    }
}