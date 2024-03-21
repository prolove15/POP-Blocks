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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.Integrations.Ads.Editor
{
    [CustomPropertyDrawer(typeof(AdNetwork))]
    public class AdNetworkProperty : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            position.width = 100;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("network"), GUIContent.none);
            var r1 = position;
            r1.x += position.width + 10;
            var network = (AdNetworks) property.FindPropertyRelative("network").enumValueIndex;
            if (network == AdNetworks.Admob)
            {
                if (!Directory.Exists("Assets/GoogleMobileAds")) ShowInstallButton(r1, network);
                else ShowSettingsButton(r1, network);
            }
            else ShowSettingsButton(r1, network);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private void ShowInstallButton(Rect r1, AdNetworks network)
        {
            if (UnityEngine.GUI.Button(r1, "Download"))
            {
                if (network == AdNetworks.Admob)
                    Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");
            }
        }

        private void ShowSettingsButton(Rect r1, AdNetworks network)
        {
            if (UnityEngine.GUI.Button(r1, "Settings"))
            {
                OnButtonClicked(network);
            }
        }

        private void OnButtonClicked(AdNetworks network)
        {
            if (network == AdNetworks.UnityAds)
                UnityEditor.SettingsService.OpenProjectSettings("Project/Services/Ads");
            else if (network == AdNetworks.Admob)
                EditorApplication.ExecuteMenuItem("Assets/Google Mobile Ads/Settings...");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}