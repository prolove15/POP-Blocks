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
using System.Linq;
using Malee.List;
using POPBlocks.Scripts.Gameplay.LevelHandle;
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace POPBlocks.Scripts.LevelHandle
{
    [CreateAssetMenu(fileName = "Level", menuName = "POP Blocks/Add level", order = 1)]
    public class Level : ScriptableObject
    {
        [HideInInspector]
        public int num;
        public int moves = 20;
        [Range(3, 6)] public int colors = 4;
        public Vector2Int size = Vector2Int.one * 6;

        [Reorderable(elementNameOverride = "Star", draggable = false, sortable = false, add = false, remove = false, expandByDefault = false)]
        public StarsArray stars = new StarsArray() {100, 200, 300};

        [Reorderable(expandByDefault = true)] public TargetListLevelEditor targets;
        [HideInInspector] public LevelBlock[] Blocks = new LevelBlock[36];
        [FormerlySerializedAs("simulationRaw")] [HideInInspector]
        public List<TSVParseObject> simulationAggregate = new List<TSVParseObject>();

        private void OnEnable()
        {
            if (stars == null) stars = new StarsArray() {100, 200, 300};
        }
    }

    [Serializable]
    public class StarsArray : ReorderableArray<int>
    {
        public int GetEarnedStars(int score) => this.Count(i => i <= score);
    }
    

    [Serializable]
    public class LevelBlock
    {
        public BlocksTypes type;
        public GameObject prefab;
        public int color;
        public Texture2D icon;
        public int layer;
        [SerializeField]
        public GameObject overlapBlockObject;
        public TexPos[] layers;
        public Vector2Int size = Vector2Int.one;
        public EditorIconTypes secondClick;
        public bool rotate;

        public LevelBlock(BlocksTypes type, GameObject prefab, Texture2D icon, Vector2Int size, int col = 0)
        {
            this.type = type;
            this.prefab = prefab;
            this.icon = icon;
            this.color = col;
            this.size = size;
            if (prefab != null)
            {
                this.secondClick = prefab.GetComponent<IconEditor>().secondClick;
                var layeredBlock = prefab.transform.GetComponentInChildren<LayeredBlock>();
                if (layeredBlock != null)
                {
                    layers = layeredBlock.GetTextures(color);
                }
            }
        }


        public void ChangeLayer()
        {
            if (layers != null && layers.Any())
            {
                layer = (int) Mathf.Repeat(layer + 1, layers.Length + 2);
            }
        }

        public Texture2D[] GetTextures()
        {
            var t1 = new List<Texture2D>();
            t1.Add(icon);
            if (layers != null)
            {
                for (var i = 0; i < layer - 1; i++)
                {
                    t1.Add(RotateTexture(layers[i]));
                }

            }
            if (overlapBlockObject != null) t1.Add(overlapBlockObject.GetComponent<IconEditor>().icon[0]);

            return t1.ToArray();
        }

        private static Texture2D RotateTexture(TexPos t)
        {
            Texture2D rotateTexture = t.Texture2D;
            if (t.rotation != Vector3.zero)
                rotateTexture = t.Texture2D.rotateTexture();
            return rotateTexture;
        }

        public LevelBlock Clone()
        {
            var memberwiseClone = (LevelBlock) MemberwiseClone();
            memberwiseClone.layer = 0;
            return memberwiseClone;
        }

        public void Rotate()
        {
            rotate = !rotate;
        }
    }

    [Serializable]
    public class TexPos
    {
        public Texture2D Texture2D;
        public Vector2 pos;
        public Vector2 scale;
        public Vector3 rotation;
        // public List<TexPos> chidlren;
    }
}