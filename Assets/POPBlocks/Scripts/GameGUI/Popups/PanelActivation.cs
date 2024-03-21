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
using UnityEngine;
using UnityEngine.Events;

namespace POPBlocks.Scripts.GameGUI.Popups
{
    public class PanelActivation : MonoBehaviour
    {
        public ActivationEvents[] events;
        private string lastPopup;
        private void OnEnable()
        {
            Popup.OnShowPopup += OnShowPopup;
            Popup.OnHidePopup += OnHidePopup;
        }

        private void OnHidePopup(string popupname)
        {
            foreach (var activationEventse in events)
            {
                if (popupname == activationEventse.activatingPopup && lastPopup == activationEventse.activatingPopup) activationEventse.OnHidePopupEvent?.Invoke();
            }
        }

        private void OnDisable()
        {
            Popup.OnShowPopup -= OnShowPopup;
            Popup.OnHidePopup -= OnHidePopup;
        }

        private void OnShowPopup(string popupname)
        {
            lastPopup = popupname;
            foreach (var activationEventse in events)
            {
                if (popupname == activationEventse.activatingPopup) activationEventse.OnShowPopupEvent?.Invoke();
                else gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    public class ActivationEvents
    {
        public string activatingPopup;
        public UnityEvent OnShowPopupEvent;
        public UnityEvent OnHidePopupEvent;
    }
}