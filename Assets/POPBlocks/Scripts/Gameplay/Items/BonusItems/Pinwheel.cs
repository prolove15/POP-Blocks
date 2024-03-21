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
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using POPBlocks.Scripts.Effects;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Pool;
using POPBlocks.Scripts.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace POPBlocks.Scripts.Items.BonusItems
{
    public class Pinwheel : BonusItem
    {
        public float rotateSpeed = 1;
        private Transform childTransform;
        public GameObject LightningPrefab;
        private float saveRotateSpeed;
        public ColorComponent[] colorComponents;
        public override void Awake()
        {
            saveRotateSpeed = rotateSpeed;
            childTransform = transform.Find("Sprites");
            base.Awake();
        }

        public override void OnEnable()
        {
            rotateSpeed = saveRotateSpeed;
            base.OnEnable();
        }

        private void Update()
        {
            childTransform.Rotate(0, 0, rotateSpeed);
        }

        public override void ActivateBonus(bool byTouch, Action callback)
        {
            base.ActivateBonus(byTouch, callback);
            if (doubleBonus || !gameObject.activeSelf) return;
            LevelManager.Instance.moveCounter++;
            StartCoroutine(ActivationEffect(callback));
        }

        IEnumerator ActivationEffect(Action callback, Item secondBonus=null)
        {
            if (!(secondBonus is null) && secondBonus.bonus.type == BonusItemTypes.Pinwheel)
            {
                colorComponents[0].SetColor(2);
                colorComponents[1].SetColor(5);
                colorComponents[2].SetColor(0);
                colorComponents[3].SetColor(4);
                colorComponents[4].SetColor(3);
                colorComponents[5].SetColor(1);
            }
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.pinWheelSound[1]);
            int sc = 0;
            GetComponent<SortingGroup>().sortingOrder = 1000;
            rotateSpeed = -1;
            SetLayer(1, "UI");
            falling = true;
            childTransform.DOScale(1.2f, 1f);
            yield return new WaitForSeconds(1f);

            List<Item> list2 = new List<Item>();
            var count = Field.Instance.items.Count(z => z != null && z.matchable && z.Color == Color && z.gameObject.activeSelf);
            var enumerable = Field.Instance.items.Where(x=>x != null && x.bonus == null && x.Color< 6);
            var list = Field.Instance.items.Where(i => i != null && i.Color == Color && i.bonus?.type != BonusItemTypes.Pinwheel && i.matchable);
           
            if(secondBonus == null || secondBonus.bonus.type != BonusItemTypes.Pinwheel)
            {
                int i = 0;
                foreach (var item1 in list)
                {
                    if (item1 != null && item1.explodable)
                    {
                        CreateLightning(transform.position, item1.transform.position);
                        SoundBase.Instance.PlayOneShot(SoundBase.Instance.pinWheelSound[0]);

                        yield return new WaitForSeconds(0.1f);
                        sc += item1.score;
                        item1.latestMoveCounter = 0;
                        if (item1.overlapItem != null) item1.overlapItem.destroyWithItem = true;
                        item1.DestroyItemStart(false, 0, noEffect: false, effectName: "", destroyAround: true);
                        if (secondBonus != null)
                        {
                            var item = Field.Instance.CreateItem(secondBonus.name, item1.index, item1.pos);
                            list2.Add(item);
                            LevelManager.Instance.destroyingBonusesList.Add(item);
                        }
                    }

                    if (i%3==0) ExplosionEffect();
                    i++;
                }
                for ( i = 0; i < 10; i++)
                {
                    if(!Field.Instance.items.Any(z => z != null && z.matchable && z.Color == Color)) break;
                    var item1 = Field.Instance.items.Where(z => z != null && z.Color == Color && z.gameObject.activeSelf).NextRandom();
                    if (item1 != null && item1.explodable)
                    {
                        CreateLightning(transform.position, item1.transform.position);
                        SoundBase.Instance.PlayOneShot(SoundBase.Instance.pinWheelSound[0]);

                        yield return new WaitForSeconds(0.1f);
                        sc += item1.score;
                        item1.latestMoveCounter = 0;
                        if (item1.overlapItem != null) item1.overlapItem.destroyWithItem = true;
                        item1.DestroyItemStart(false, 0, noEffect: false, effectName: "", destroyAround: true);
                        if (secondBonus != null)
                        {
                            var item = Field.Instance.CreateItem(secondBonus.name, item1.index, item1.pos);
                            list2.Add(item);
                            LevelManager.Instance.destroyingBonusesList.Add(item);
                        }
                    }

                    if (i%3==0) ExplosionEffect();
                }
                secondBonus?.DestroyItemFinish();
            }
            else if (secondBonus != null && secondBonus.bonus.type == BonusItemTypes.Pinwheel)
            {
                secondBonus.bonus.activated = true;
                bonus.activated = true;
                for (var x = 0; x < Field.Instance.size.x; x++)
                {
                    list = Field.Instance.GetColumn(x).Where(i=>i.explodable && i != this && i != secondBonus).ToList();
                    foreach (var a in list)
                    {
                        CreateLightning(transform.position, a.transform.position);
                        sc += a.score;
                        if(a.overlapItem != null) a.overlapItem.DestroyItemStart(false, 0);
                        else a.DestroyItemStart(false, 0, noEffect: false, effectName: "", destroyAround: true);
                    }
                    SoundBase.Instance.PlayOneShot(SoundBase.Instance.pinWheelSound[0]);
                    yield return new WaitForSeconds(0.1f);
                }
                secondBonus.DestroyItemFinish();
                DestroyItemFinish();
            }
            callback?.Invoke();
            yield return new WaitForSeconds(0.2f);
            Field.Instance.PinwheelEffectFinish(callback, list2, sc, this, secondBonus);

           
        }

        public override void DoubleBonusStartAnimation(BonusItem secondBonus, Action callback, float time)
        {
            activated = true;
            secondBonus.activated = true;
            StartCoroutine(AnimateDouble(secondBonus, time, callback));
            base.DoubleBonusStartAnimation(secondBonus, callback, time);
        }

        IEnumerator AnimateDouble(Item secondBonus, float time, Action callback)
        {
            secondBonus.SetLayer(99);
            SetLayer(200);
            var v2 = secondBonus.transform.position;
            var v1 = transform.position;
            var rocketPos = v1 + (v2 - v1) / 2f;
            var rotatedRocketPos = v2 + (v1 - v2) / 2f;
            secondBonus.transform.DOMove(rotatedRocketPos, time);
            transform.DOMove(rocketPos, time);
            yield return new WaitForSeconds(time);
            StartCoroutine(ActivationEffect(callback, secondBonus));

        }
        
        private void ExplosionEffect()
        {
            var obj = ObjectPooler.Instance.GetPooledObject("firework");
            obj.transform.position = transform.position;
        }

        private void CreateLightning(Vector3 pos1, Vector3 pos2)
        {
            var go = Instantiate(LightningPrefab, Vector3.zero, Quaternion.identity);
            var lightning = go.GetComponent<Lightning>();
            lightning.SetLight(pos1, pos2);
        }
    }
}