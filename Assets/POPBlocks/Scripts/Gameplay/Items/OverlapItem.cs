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

using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Pool;
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class OverlapItem : Item
    {
        [HideInInspector] public Item itemUnder;
        // [HideInInspector] public SortingGroup sortingGroup;
        // [HideInInspector] public Vector2Int pos;
        // [HideInInspector] public SpriteRenderer sr;
        [Header("Item don't fall")] public bool fixedItem;
        [Header("Item under can be activated by touch")]
        public bool canBeTouch;
        [Header("Destroy along with item inside")]
        public bool destroyWithItem;
        public Item ItemUnder => Field.Instance.items[index];
        // set => itemUnder = value;
        // public override void Awake()
        // {
        //     sortingGroup = GetComponent<SortingGroup>();
        //     sr = GetComponentInChildren<SpriteRenderer>();
        //     base.Awake();
        // }

        public override void ActivateItem()
        {
            ItemUnder?.ActivateItem();
        }

        public override void DestroyItemStart(bool noScore = false, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            if(ItemUnder == null)
                DestroyObstacle(this);
            else
                ItemUnder.DestroyItemStart();
        }

        protected override void DestroyObstacle(Item item)
        {
            LevelManager.Instance.DestroySpriteEvent(sr[0].sprite, transform.position, this, null);
            Field.Instance.overlapItems[index] = null;
            if (ItemUnder != null) ItemUnder.touchable = true;
            base.DestroyObstacle(item);
            ObjectPooler.Instance.PutBack(gameObject);
        }
        
    }
}