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
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    public class GUICounter : MonoBehaviour
    {
        private int value;
        [Header("enter a name to save the value in player prefs")]
        public string saveName;
        [Header("field name from GameSettings for default value")]
        public string defaultField;
        public TextMeshProUGUI guiObject;

        public delegate void ChangeValue(int i);

        public event ChangeValue OnChangeValue;
        private void Awake()
        {
            if (guiObject == null)
                guiObject = GetComponent<TextMeshProUGUI>();
            if (saveName != "")
            {
                value = PlayerPrefs.GetInt(saveName);
                UpdateGUI();
            }
            if (PlayerPrefs.GetInt("SessionCounter") < 1 && defaultField != "")
            {
                RestoreDefault();
            }
        }

        public void RestoreDefault()
        {
            var settings = Resources.Load<GameSettings>("Settings/GameSettings");
            SetValue(settings.GetAttribute<int>(defaultField));
        }

        public void SetValue(int v)
        {
            value = v;
            if (value < 0) value = 0;
            if(saveName != "")
            {
                PlayerPrefs.SetInt(saveName, value);
                PlayerPrefs.Save();
            }
            UpdateGUI();
        }

        public void DecrementValue(int v)
        {
            SetValue(value-v);
            UpdateGUI();
        }

        public void IncrementValue(int v)
        {
            SetValue(value+v);
            UpdateGUI();
        }

        void UpdateGUI()
        {
            OnChangeValue?.Invoke(value);
            if (guiObject != null) guiObject.text = value.ToString();
        }

        public int GetValue() => value;
  
    }
}