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

using POPBlocks.Scripts.Items;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    [CustomEditor(typeof(ItemTypeChanger))]
    public class ItemDebugEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ItemTypeChanger myTarget = (ItemTypeChanger) target;
            if (GUILayout.Button("Set rocket"))
            {
                myTarget.SetRocket();
            }

            if (GUILayout.Button("Set bomb"))
            {
                myTarget.SetBomb();
            }
            if (GUILayout.Button("Set pinwheel"))
            {
                myTarget.SetPinwheel();
            }
            if (GUILayout.Button("Set ingredient"))
            {
                myTarget.SetIngredient();
            }
            if (GUILayout.Button("Set ingredient big"))
            {
                myTarget.SetIngredientBig();
            }
        }
    }
}