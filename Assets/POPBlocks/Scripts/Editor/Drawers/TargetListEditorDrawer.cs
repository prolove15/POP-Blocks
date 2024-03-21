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

using System.Collections.Generic;
using System.Linq;
using POPBlocks.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle.Editor
{
    [CustomPropertyDrawer(typeof(Target))]
    public class TargetListEditorDrawer : PropertyDrawer
    {
        private List<string> targetNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorApplication.isPlaying) return;

            var level = property.serializedObject.targetObject as Level;
            var list = Resources.LoadAll<TargetScriptable>("Targets").Where(i=>i.showInEditor).ToArray();
            targetNames = list.Select(i => i.name).ToList();
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var countArray = property.FindPropertyRelative("count");
            var targetIndex = targetNames.IndexOf(((Target) PropertyUtils.GetObject(property)).target?.name);//list.First(i=>i.prefab.name ) 
            property.FindPropertyRelative("index");
            Rect r1 = position;
            r1.height = EditorGUIUtility.singleLineHeight;
            var targetEditorObject = PropertyUtils.GetObject(property) as Target;
            if (targetEditorObject.target == null)
                targetEditorObject.target = list[0];
            if (targetEditorObject.count.values.Length == 0)
                targetEditorObject.count = new CountArray() {values = new int[20]};
            targetIndex = EditorGUI.Popup(r1, targetIndex, targetNames.ToArray());
            if (targetIndex >= 0 && list.Length > targetIndex)
            {
                var targetReference = list[targetIndex];
                if (EditorGUI.EndChangeCheck())
                {
                    var target = property.FindPropertyRelative("target");
                    target.objectReferenceValue = targetReference;

                    // AssetDatabase.SaveAssets();
                    // AssetDatabase.Refresh();
                }
            }

            try
            {
                var count = countArray;
                EditorGUI.PropertyField(position, count, GUIContent.none);
            }
            catch 
            {
                
            }


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 25;
        }
    }
}