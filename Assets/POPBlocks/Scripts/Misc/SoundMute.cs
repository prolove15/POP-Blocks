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
using UnityEngine.Audio;
using UnityEngine.UI;

namespace POPBlocks.Scripts
{
    public class SoundMute : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private CanvasRenderer canvasRenderer;
        public string volumeVariable = "soundVolume";


        void Start()
        {
            //Fetch the Toggle GameObject
            var m_Toggle = GetComponent<Toggle>();
            //Add listener for when the state of the Toggle changes, to take action
            m_Toggle.onValueChanged.AddListener(delegate {
                Mute(m_Toggle);
            });

        }
        private void OnEnable()
        {
            SetSound(PlayerPrefs.GetInt(volumeVariable));
        }

        public void Mute(Toggle change)
        {
            if(!change.isOn) PlayerPrefs.SetInt(volumeVariable,-80);
            else PlayerPrefs.SetInt(volumeVariable,0);
            PlayerPrefs.Save();
            SetSound(PlayerPrefs.GetInt(volumeVariable));
        }

        private void SetSound(int getInt)
        {
            audioMixer.SetFloat(volumeVariable, getInt);
            canvasRenderer.SetAlpha(getInt<0?0.3f:1);
        }
    }
}