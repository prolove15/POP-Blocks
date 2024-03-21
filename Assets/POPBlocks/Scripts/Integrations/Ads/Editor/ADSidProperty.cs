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

using POPBlocks.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.Integrations.Ads.Editor
{
    [CustomPropertyDrawer(typeof(IDs))]
    public class ADSidProperty : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var adItem = (AdItem) PropertyUtils.GetParent(property);
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            int space = 20;
            // Calculate rects
            var positionWidth = (position.width + 100) / 3;
            var positionX = position.x;
            var r1 = new Rect(positionX, position.y, positionWidth-100, position.height);
            var r2 = new Rect(r1.xMax + space, position.y, positionWidth-100, position.height);
            var r3 = new Rect(r2.xMax + space, position.y, positionWidth+50, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            if(adItem.adNetwork.network != AdNetworks.UnityAds && adItem.adNetwork.network != AdNetworks.Ironsource)
                EditorGUI.PropertyField(r1, property.FindPropertyRelative("type"), GUIContent.none);
            EditorGUI.PropertyField(r2, property.FindPropertyRelative("platform"), GUIContent.none);
            EditorGUI.PropertyField(r3, property.FindPropertyRelative("id"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}