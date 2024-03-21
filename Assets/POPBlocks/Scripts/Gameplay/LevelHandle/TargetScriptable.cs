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
using System.Collections.Generic;
using Malee.List;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle
{
    [CreateAssetMenu(fileName = "target", menuName = "POP Blocks/Add target", order = 1)]
    public class TargetScriptable : ScriptableObject
    {
        public bool showInEditor;
        public TargetActions action;
        public bool countFromField;
        [Reorderable(expandByDefault = true)] public Spritelist targetSprites;

        [Header("Optional")] public GameObject prefab;
        // [Reorderable(expandByDefault = true)]
        // public PrefabList prefabs;
    }

    [Serializable]
    public class PrefabList : ReorderableArray<GameObject>
    {
    }

    [Serializable]
    public class Spritelist : ReorderableArray<SpriteObject>
    {
    }

    [Serializable]
    public struct SpriteObject
    {
        public Sprite icon;

        //uses as separate target in UI 
        public bool uiSprite;
        public int count;
    }

    public enum TargetActions
    {
        Spread,
        ReachBottom,
        Destroy
    }

    [Serializable]
    public class TargetList : ReorderableArray<TargetScriptable>
    {
    }

    [Serializable]
    public class TargetListLevelEditor : ReorderableArray<Target>
    {
    }

    [Serializable]
    public class Target
    {
        public int index;
        public TargetScriptable target;
        public CountArray count;
    }

    [Serializable]
    public class CountArray
    {
        public int[] values = new int[20];
    }

    public class TargetSprite
    {
        public List<Sprite> sprites = new List<Sprite>();
        public int count;
        public bool countFromField;
        public GameObject prefab;
        public int[] hash;
    }
}