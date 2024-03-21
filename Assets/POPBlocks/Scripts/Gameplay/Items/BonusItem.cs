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
using DG.Tweening;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Utils;
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class BonusItem : Item
    {
        public BonusItemTypes type;
        readonly List<Vector3> childTransformsPositions = new List<Vector3>();
        readonly List<Vector3> childTransformsScale = new List<Vector3>();
        protected bool doubleBonus;

        public override void Awake()
        {
            foreach (Transform tr in transform)
            {
                childTransformsPositions.Add(tr.localPosition);
                childTransformsScale.Add(tr.localScale);
            }

            base.Awake();
        }

        public override void OnEnable()
        {
            doubleBonus = false;
            activated = false;
            // _animator?.Reset("init");
            int i = 0;
            foreach (Transform tr in transform)
            {
                tr.localPosition = childTransformsPositions[i];
                tr.localScale = childTransformsScale[i];
                i++;
            }

            base.OnEnable();
        }

        public override void DestroyItemStart(bool noScore = false, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            if (activated) return;
            ActivateBonus(false, () => { 
                Field.Instance.FillItemsArray(null, pos.x, pos.y, size);
                DestroyItemFinish();
            });
            // base.DestroyItem(noScore, delay, noEffect, effectName, destroyAround);
        }


        public virtual void ActivateBonus(bool byTouch, Action callback)
        {
            activated = true;
            var list = Field.Instance.GetItemsCross(this).Where(i => i != null && i.bonus).Select(i=>i.bonus);
            if (list.Any() && byTouch)
            {
                LevelManager.Instance.moveCounter++;
                var secondBonus = list.NextRandom();
                CheckDoubleBonus(secondBonus, callback);
            }

            if (list.Any() && byTouch) doubleBonus = true;
            else PlayActivationSound();
        }

        protected void PlayActivationSound()
        {
            if (activateSound != null) SoundBase.Instance.PlayOneShot(activateSound);
        }

        private void CheckDoubleBonus(BonusItem secondBonus, Action callback)
        {
            float time = 0.5f;
            if (secondBonus.overlapItem != null) secondBonus.overlapItem.destroyWithItem = true;
            if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Rocket)
            {
                DoubleBonusStartAnimation(secondBonus, callback, 0.3f);
            }
            else if (bonus.type == BonusItemTypes.Bomb && secondBonus.bonus.type == BonusItemTypes.Bomb)
            {
                DoubleBonusStartAnimation(secondBonus, callback, time);
            }
            else if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Bomb ||
                secondBonus.bonus.type == BonusItemTypes.Rocket && bonus.type == BonusItemTypes.Bomb)
            { 
                DoubleBonusStartAnimation(secondBonus, callback, 0.3f);
            }
            else if (bonus.type == BonusItemTypes.Pinwheel || secondBonus.bonus.type == BonusItemTypes.Pinwheel)
            {
                if(bonus.type != BonusItemTypes.Pinwheel) secondBonus.bonus.ActivateBonus(true, callback);
                else DoubleBonusStartAnimation(secondBonus, callback, 0.3f);
            }
            else
            {
                secondBonus.transform.DOMove(secondBonus.transform.position + (transform.position - secondBonus.transform.position) / 2f, time);
                transform.DOMove(transform.position + (secondBonus.transform.position - transform.position) / 2f, time)
                .OnComplete(() => { ActivateDoubleBonus(secondBonus, callback); });
            }
        }

        protected void FinishRocketAnimation(Item rocket, float time, int xOffset, int yOffset, float delay, Vector3 offsetPos, Vector3 scale, Vector2Int position, Action callback)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(rocket.transform.GetChild(0).DOScale(scale, time).SetDelay(delay));
            sequence.Join(rocket.transform.GetChild(0).DOMove(rocket.transform.position + offsetPos, time));
            sequence.AppendCallback(() =>
            {
                rocket.bonus.AnimateBonus(position,callback);
            });
        }

        

        public virtual void DoubleBonusStartAnimation(BonusItem secondBonus, Action callback, float time)
        {
        }

        protected void ActivateDoubleBonus(Item secondBonus, Action callback)
        {
            secondBonus.activated = true;
            if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Bomb ||
                secondBonus.bonus.type == BonusItemTypes.Rocket && bonus.type == BonusItemTypes.Bomb)
            {
                Field.Instance.DestroyBombRocket(this, secondBonus, callback);
            }
            else if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Rocket)
            {
                Field.Instance.DestroyRocketRocket(this, secondBonus, callback);
            }
            else if (bonus.type == BonusItemTypes.Bomb && secondBonus.bonus.type == BonusItemTypes.Bomb)
            {
                Field.Instance.DestroyBombBomb(this.pos, this, secondBonus, callback);
            }
            else if (bonus.type == BonusItemTypes.Pinwheel || secondBonus.bonus.type != BonusItemTypes.Pinwheel)
            {
                Field.Instance.DestroyPinWheel(this, secondBonus, callback);
            }
            else if (bonus.type != BonusItemTypes.Pinwheel || secondBonus.bonus.type == BonusItemTypes.Pinwheel)
            {
                Field.Instance.DestroyPinWheel(secondBonus, this, callback);
            }
        }

        public virtual void AnimateBonus(Vector2Int vector2Int, Action callback)
        {
        }

        public static Func<Item, bool> NotAnyIsBonus(Item firstBonus, Item secondBonus)
        {
            return item => item != null && item.explodable && item != firstBonus && item != secondBonus;
        }
    }

    public enum BonusItemTypes
    {
        Rocket,
        Bomb,
        Pinwheel,
        None
    }
}