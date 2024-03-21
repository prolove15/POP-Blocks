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

using System.Linq;
using DG.Tweening;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Utils;
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class JellySpreading : Item
    {
        private static bool jellyDestroyed;
        private static bool jellySpreaded;

        public override void OnEnable()
        {
            LevelManager.OnAfterMove += OnAfterMove;
            LevelManager.OnStartMove += OnStartMove;
            base.OnEnable();
        }

        public override void OnDisable()
        {
            LevelManager.OnAfterMove -= OnAfterMove;
            LevelManager.OnStartMove -= OnStartMove;
            base.OnDisable();
        }

        private void OnStartMove()
        {
            jellySpreaded = false;
            jellyDestroyed = false;
        }

        private void OnAfterMove()
        {
            if (!jellyDestroyed && !jellySpreaded)
            {
                jellySpreaded = true;
                var list = FindObjectsOfType<JellySpreading>().Select(i => i).OrderByDescending(i=>i.pos.y);
                foreach (var item1 in list)
                {
                    var crossItems = Field.Instance.GetItemsCross(item1).Where(i=>i != null && i.overlapItem==null).Randomize();
                    foreach (var crossItem in crossItems)
                    {
                        if (crossItem.simpleItem)
                        {
                            var item = crossItem.GetComponent<ItemTypeChanger>().SetType("JellySpreading");
                            item.transform.localScale = Vector3.zero;
                            item.transform.DOScale(Vector3.one, 0.5f);
                            LevelManager.Instance.UpdateTargets();
                            return;
                        }
                    }
                }
            }
        }

        public override void DestroyItemStart(bool noScore, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            jellyDestroyed = true;
            base.DestroyItemStart(noScore,  0, noEffect: noEffect);
        }

        protected override void DestroyObstacle(Item itemDestroyed)
        {
            base.DestroyObstacle(itemDestroyed);
            DestroyItemStart(false, 0);
        }
    }
}