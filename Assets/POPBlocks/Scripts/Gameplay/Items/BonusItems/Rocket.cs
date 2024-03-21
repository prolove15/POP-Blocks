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
using System.Linq;
using DG.Tweening;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace POPBlocks.Scripts.Items.BonusItems
{
    public class Rocket : BonusItem
    {
        public override void OnEnable()
        {
            Rotate(Quaternion.Euler(0, 0, Random.Range(0, 2) == 1 ? -90 : 0));
            base.OnEnable();
        }

        public override void DoubleBonusStartAnimation(BonusItem secondBonus, Action callback, float time)
        {
            activated = true;
            secondBonus.activated = true;
            if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Rocket)
                StartCoroutine(AnimateRocketRocket(secondBonus, callback, time));
            else if (bonus.type == BonusItemTypes.Rocket && secondBonus.bonus.type == BonusItemTypes.Bomb ||
                     secondBonus.bonus.type == BonusItemTypes.Rocket && bonus.type == BonusItemTypes.Bomb)
                Field.Instance.DestroyBombRocket(this, secondBonus, callback, time);
            base.DoubleBonusStartAnimation(secondBonus, callback, time);
        }

        public override void ActivateBonus(bool byTouch, Action callback)
        {
            base.ActivateBonus(byTouch, callback);
            if (doubleBonus) return;
            LevelManager.Instance.moveCounter++;
            AnimateBonus(pos, callback);
            DestroyItemFinish();
        }

        private IEnumerator AnimateRocketRocket(Item secondBonus, Action callback, float time)
        {
            var v1 = transform.position + GetOffset(transform);
            secondBonus.transform.DOMove(v1, time);
            yield return new WaitForSeconds(time);
            var rocketRocketAnimation = Instantiate(Resources.Load<BonusRocketEffectData>("DoubleBonusPrefabs/RocketRocket"));
            rocketRocketAnimation.bonusItems = new[] {bonus, secondBonus.bonus};
            rocketRocketAnimation.Pos = pos;
            rocketRocketAnimation.callBack = callback;
            rocketRocketAnimation.transform.position = v1;
            Item[] itemsV = { };
            Item[] itemsH = { };
            rocketRocketAnimation.GetComponent<SignalReceiver>().GetReactionAtIndex(0).AddListener(() =>
            {
                var sc = score * 2 + itemsV.Concat(itemsH).Sum(i => i.score);
                LevelManager.Instance.ShowScorePop(sc, transform.position);
            });
            rocketRocketAnimation.GetComponent<SignalReceiver>().GetReactionAtIndex(1).AddListener(() =>
            {
                itemsH = Field.Instance.GetDoubleRocketLaunchHorizontal(pos);
                // itemsH.ForEachY(i => i.DestroyItem());
            });
            rocketRocketAnimation.GetComponent<SignalReceiver>().GetReactionAtIndex(2).AddListener(() =>
            {
                itemsV = Field.Instance.GetDoubleRocketLaunchVertical(pos);
                // itemsV.ForEachY(i => i.DestroyItem());
            });
            PutBackIntoPool();
            secondBonus.PutBackIntoPool();
        }

        private Vector3 GetOffset(Transform bonus)
        {
            if (bonus.GetChild(0).localPosition != Vector3.zero)
                return bonus.GetChild(0).position - bonus.position;
            return Vector3.zero;
        }


        public override void AnimateBonus(Vector2Int vector2Int, Action callback)
        {
            int sc = 0;
            PlayActivationSound();
            Item[] items;
            Vector2Int[] l = new Vector2Int[3];
            if (IsHorrizontal())
                items = Field.Instance.GetRow(vector2Int.y);
            else
            {
                items = Field.Instance.GetColumn(vector2Int.x);
                // Field.Instance.blockColumn[pos.x] = gameObject;
            }

            sc += items.Sum(i => i.score);
            var rocketRocketAnimation = Instantiate(Resources.Load<BonusRocketEffectData>("DoubleBonusPrefabs/RocketEffect"));
            rocketRocketAnimation.Pos = pos;
            rocketRocketAnimation.callBack = callback;
            rocketRocketAnimation.transform.position = transform.position;
            rocketRocketAnimation.transform.localRotation = transform.localRotation;
            if (!IsHorrizontal())
            {
                rocketRocketAnimation.SetVertical();
            }

            rocketRocketAnimation.StartTriggerAllRockets();
            LevelManager.Instance.ShowScorePop(sc, transform.position);
            base.AnimateBonus(vector2Int, callback);
            // DestroyItem();
        }

        public bool IsHorrizontal()
        {
            return Vector2.Dot(Vector2.right, transform.right) == 1;
        }

        public void SetHorizontal(bool horizontal)
        {
            Rotate(Quaternion.Euler(0, 0, horizontal ? 0 : -90));
        }
    }
}