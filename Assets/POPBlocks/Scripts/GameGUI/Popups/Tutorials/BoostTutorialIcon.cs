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
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI.Popups.Tutorials
{
    public class BoostTutorialIcon : MonoBehaviour
    {
        public TutorialGamePopup popup;
        public Image image;
        private void Start()
        {
            StartCoroutine(WaitFor());
        }

        private IEnumerator WaitFor()
        {
            yield return new WaitUntil(()=>popup.hightLightedUIObject != null);
            var original = popup.hightLightedUIObject.GetChild(0).GetChild(0);
            var go = Instantiate(original, image.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = original.localScale*3.5f;
            // image.SetNativeSize();
        }
    }
}