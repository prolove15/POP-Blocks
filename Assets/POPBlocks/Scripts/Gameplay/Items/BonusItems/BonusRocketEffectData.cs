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
using System.Linq;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Utils;
using UnityEngine;

namespace POPBlocks.Scripts.Items.BonusItems
{
    public class BonusRocketEffectData : MonoBehaviour
    {
        [HideInInspector] public Vector2Int pos;
        public GameObject blockingObject;
        public RocketTrigger[] rockets;
        public BonusItem[] bonusItems;
        public Action callBack;
        public float delayToDestroy;
        public bool doubleBonus;
        public Vector2Int Pos
        {
            get => pos;
            set
            {
                pos = value;
                rockets.ForEachY(i => i.startPos = pos);
            }
        }

        public void OnStartColumn()
        {
            rockets.Where(i => i.dir.y != 0).ForEachY(i => i.StartTrigger(this));
        }

        public void OnStartRow()
        {
            rockets.Where(i => i.dir.x != 0).ForEachY(i => i.StartTrigger(this));
        }

        private void BlockColumns(int radius = 2)
        {
            Field.Instance.blockColumn[Pos.x] = blockingObject;
            for (int i = 0; i < radius; i++)
            {
                if(Field.Instance.blockColumn.InsideBounds(Pos.x - i))
                    Field.Instance.blockColumn[Pos.x - i] = blockingObject;
                if (Field.Instance.blockColumn.InsideBounds(Pos.x + i))
                    Field.Instance.blockColumn[Pos.x + i] = blockingObject;
            }
        }

        public void OnFinishedColumn()
        {
            rockets.Where(i => i.dir.y != 0).ForEachY(i => i.Finish());
        }

        public void OnFinishedRow()
        {
            rockets.Where(i => i.dir.x != 0).ForEachY(i => i.Finish());
            callBack?.Invoke();
        }

        public void StartTriggerAllRockets()
        {
            rockets.ForEachY(i => i.StartTrigger(this));
        }

        private void OnDestroy()
        {
            callBack?.Invoke();
        }

        public void FinishTriggerAllRockets()
        {
            rockets.ForEachY(i => i.Finish());
            callBack?.Invoke();
        }

        public void DoubleBomb()
        {
            Field.Instance.DestroyBombBomb(Pos, bonusItems[0], bonusItems[1], null);
            BlockColumns(3);
        }

        public void SetVertical()
        {
            rockets.ForEachY(i => i.isVertical = true);
        }
    }
}