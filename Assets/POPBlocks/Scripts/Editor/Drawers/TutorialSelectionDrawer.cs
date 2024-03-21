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

using POPBlocks.Scripts.GameGUI.Popups.Tutorials;
using POPBlocks.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.Popups.Tutorials.Editor
{
    [CustomPropertyDrawer(typeof(TutorialSelection))]
    public class TutorialSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect r1 = new Rect(position.x, position.y, 150, position.height);
            Rect r2 = new Rect(position.x + 170, position.y, 60, position.height);
            Rect r3 = new Rect(position.x + 200, position.y, 150, position.height);

            EditorGUI.BeginProperty(position, label, property);
            var tutorialSelection = PropertyUtils.GetObject(property) as TutorialSelection;
            EditorGUI.PropertyField(r1, property.FindPropertyRelative("selectionTypes"), GUIContent.none);
            switch (tutorialSelection.selectionTypes)
            {
                case SelectionTypes.SelectMatch:
                    var serializedProperty = property.FindPropertyRelative("condition");
                    serializedProperty.enumValueIndex = EditorGUI.Popup(r2, serializedProperty.enumValueIndex , GetConditionList());
                    // EditorGUI.PropertyField(r2, serializedProperty, GUIContent.none);
                    EditorGUI.PropertyField(r3, property.FindPropertyRelative("selectMatch"), GUIContent.none);
                    break;
                case SelectionTypes.SelectBonus:
                    EditorGUI.PropertyField(r3, property.FindPropertyRelative("selectBonusItemType"), GUIContent.none);
                    break;
                case SelectionTypes.SelectBlock:
                    EditorGUI.PropertyField(r3, property.FindPropertyRelative("selectBlock"), GUIContent.none);
                    break;
                case SelectionTypes.SelectRow:
                    EditorGUI.PropertyField(r3, property.FindPropertyRelative("selectRowColumn"), GUIContent.none);
                    break;
                case SelectionTypes.SelectColumn:
                    EditorGUI.PropertyField(r3, property.FindPropertyRelative("selectRowColumn"), GUIContent.none);
                    break;
            }

            // Set indent back to what it was
            EditorGUI.EndProperty();
        }

        string[] GetConditionList()
        {
            string[] l = new[]
            {
                "==", ">=", ">", "<", "<="
            };
            return l;
        }
    }
}