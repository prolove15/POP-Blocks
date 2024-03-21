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
using System.Collections;
using DG.Tweening;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;
using UnityEngine.Timeline;

namespace POPBlocks.Scripts.Items.BonusItems
{
    public class Bomb : BonusItem
    {
        public override void ActivateBonus(bool byTouch, Action callback)
        {
            base.ActivateBonus(byTouch, callback);
            if (doubleBonus) return;
            if (LevelManager.Instance.GameState != GameState.PreWinAnimation && byTouch)
                Field.Instance.cameraHandler.Shake();
            LevelManager.Instance.moveCounter++;
            Field.Instance.DestroyAround(this);
            callback?.Invoke();
        }

        public override void DoubleBonusStartAnimation(BonusItem secondBonus, Action callback, float time)
        {
            activated = true;
            secondBonus.activated = true;
            if (bonus.type == BonusItemTypes.Bomb && secondBonus.bonus.type == BonusItemTypes.Bomb)
                StartCoroutine(AnimateBombBomb(secondBonus, callback, time));
            else if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Bomb ||
                     secondBonus.bonus.type == BonusItemTypes.Rocket && bonus.type == BonusItemTypes.Bomb)
                Field.Instance.DestroyBombRocket(this, (Rocket)secondBonus, callback, time);
            base.DoubleBonusStartAnimation(secondBonus, callback, time);
        }

        private IEnumerator AnimateBombBomb(Item secondBonus, Action callback, float time)
        {
            var v1 = transform.position;
            secondBonus.transform.DOMove(v1, time);
            yield return new WaitForSeconds(time);
            var bombBombAnimation = Instantiate(Resources.Load("DoubleBonusPrefabs/BombBomb") as GameObject);
            bombBombAnimation.GetComponent<SignalReceiver>().GetReactionAtIndex(1).AddListener(() => ActivateDoubleBonus(secondBonus, callback));
            bombBombAnimation.transform.position = v1;
            this.PutBackIntoPool();
            secondBonus.PutBackIntoPool();
        }
    }
}