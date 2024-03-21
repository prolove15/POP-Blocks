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
using UnityEngine.Events;

namespace POPBlocks.Scripts.GameGUI
{
    // Hide object by selected map type
    public class HideMapCondition : MonoBehaviour
    {
        public MapTypes mapType;
        public UnityEvent conditionTrue;
        public Vector3 localPosition;

        private void Start()
        {
            var settings = Resources.Load<GameSettings>("Settings/GameSettings");
            if (settings != null && settings.mapType == mapType)
            {
                conditionTrue?.Invoke();
                transform.localPosition = localPosition;
            }
        }
    }

}