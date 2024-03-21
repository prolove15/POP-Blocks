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

namespace POPBlocks.Scripts.LevelHandle.Editor
{
    [CustomPropertyDrawer(typeof(CountArray))]
    public class CountArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var parentProp = PropertyUtils.GetParent(property) as Target;
            var size = 25;
            var offsetX = 10;
            Rect r2 = position;
            r2.yMin += 20;
            r2.width = size;
            r2.height = size;
            if (parentProp.target != null)
            {
                for (int i = 0; i < parentProp.target.targetSprites.Length; i++)
                {
                    if (parentProp.target.targetSprites[i].uiSprite)
                    {
                        EditorGUI.LabelField(r2, new GUIContent(parentProp.target.targetSprites[i].icon.texture));
                        r2.x +=  size + offsetX;
                    }
                }

                Rect r3 = position;
                r3.yMin = r2.yMax + 5;
                r3.width = size;
                r3.height = 18;
                if(!parentProp.target.countFromField)
                {
                    for (int i = 0; i < parentProp.target.targetSprites.Length; i++)
                    {
                        if (parentProp.target.targetSprites[i].uiSprite && property.FindPropertyRelative("values").arraySize > 0)
                        {
                            EditorGUI.PropertyField(r3, property.FindPropertyRelative("values").GetArrayElementAtIndex(i), GUIContent.none);
                            r3.x += size + offsetX;
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}