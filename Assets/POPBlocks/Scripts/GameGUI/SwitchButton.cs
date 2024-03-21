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
using UnityEngine.Events;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI
{
    public class SwitchButton : MonoBehaviour
    {
        private bool sw;
        public UnityEvent switchOn;
        public UnityEvent switchOff;

        private void Start()
        {
            GetComponent<Toggle>().onValueChanged.AddListener(OnSwitch);
        }

        public void OnSwitch(bool arg0)
        {
            sw = arg0;
            if(sw) switchOn.Invoke();
            else
                switchOff.Invoke();
        }
    }
}