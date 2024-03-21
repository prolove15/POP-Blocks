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

using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle.Editor
{
    [CustomPropertyDrawer(typeof(SpriteObject))]
    public class SpriteObjectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect r1 = position;
            r1.width = 20;

            Rect r2 = position;
            r2.width = 50;
            r2.xMin = r1.xMax + 10;
            Rect r3 = position;
            r3.xMin = r2.xMax + 50;
            EditorGUI.BeginProperty(position, label, property);
            var findPropertyRelative = property.FindPropertyRelative("icon");
            var sprite = (Sprite) findPropertyRelative.objectReferenceValue;
            EditorGUI.LabelField(r1, new GUIContent(sprite?.texture));
            var obj = property.serializedObject;
            EditorGUI.PropertyField(r2, property.FindPropertyRelative("uiSprite"), GUIContent.none);
            EditorGUI.PropertyField(r3, findPropertyRelative, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}