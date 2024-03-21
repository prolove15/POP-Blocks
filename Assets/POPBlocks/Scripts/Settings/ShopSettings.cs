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
using Malee.List;
using UnityEngine;

namespace POPBlocks.Scripts.Popups
{
    // [CreateAssetMenu(fileName = "ShopSettings", menuName = "MENUNAME", order = 0)]
    public class ShopSettings : ScriptableObject
    {
        // [Header("Enable Unity in-app purchasing")]
        // public bool enable;
        [Reorderable(expandByDefault = true)] public ShopItems shopItems;

        // private void OnValidate()
        // {
        //     #if UNITY_EDITOR
        //     if(enable)
        //         DefineSymbolsUtils.AddSymbol("UNITY_INAPP");
        //     else
        //         DefineSymbolsUtils.DeleteSymbol("UNITY_INAPP");
        //     #endif
        // }
    }

    [Serializable]
    public class ShopItems : ReorderableArray<ShopItemEditor>
    {
    }

    [Serializable]
    public class ShopItemEditor
    {
        public string productID;

        public int coins;
        // public Sprite texture;
    }
}