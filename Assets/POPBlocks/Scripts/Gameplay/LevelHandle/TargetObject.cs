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
using DG.Tweening;
using POPBlocks.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace POPBlocks.Scripts.LevelHandle
{
    public class TargetObject : MonoBehaviour
    {
        public TextMeshProUGUI countUI;
        private int count;
        public GameObject check;
        public TargetSprite targetBind;
        public Image image;
        private bool done;
        public bool updatable = true;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                UpdateTarget();
            }
        }

        public TargetSprite TargetBind
        {
            get => targetBind;
            set
            {
                targetBind = value;
                targetBind.hash = targetBind.sprites.Select(i => i.texture.GetHashCode()).ToArray();
                Count = targetBind.count;
                UpdateTargetFromField();
                UpdateTarget();
                if (check != null) check.SetActive(false);
            }
        }

        public void UpdateTargetFromField()
        {
            if (targetBind.countFromField && updatable)
            {
                Count = CountFromField(targetBind.hash);
                // if (Count == 0) gameObject.SetActive(false);
                // /*else*/ countUI?.gameObject.SetActive(true);
            }
        }

        private int CountFromField(int[] targetBindSprites)
        {
            var countFromField = FindObjectsOfType<Item>().SelectMany(i => i.spritesHash).Count(i => targetBindSprites.Any(x => x == i));
            if (countFromField == 0)
                countFromField = FindObjectsOfType<OverlapItem>().Select(i => i.spritesHash[0]).Count(i => targetBindSprites.Any(x => x == i));
            return countFromField;
        }

        private void UpdateTarget()
        {
            if (countUI != null)
            {
                countUI.text = Count.ToString();
                if (Count <= 0)
                {
                    if (check != null) check.SetActive(true);
                    // countUI.gameObject.SetActive(false);
                    done = true;
                }
                else
                    done = false;
            }
        }

        private void OnEnable()
        {
            LevelManager.OnDestroySprite += OnCheck;
        }

        private void OnDisable()
        {
            LevelManager.OnDestroySprite -= OnCheck;
        }

        private void OnCheck(Sprite spr, Vector2 pos, Item item, Action<bool> action)
        {
            if ((item.colorComponent.Length == 0 && TargetBind.hash.Intersect(item.spritesHash).Any()) || TargetBind.hash.Any(i => i == spr.texture.GetHashCode()))
            {
                if(Count > 0)
                {
                    if (TargetBind.prefab != null) 
                        spr = image.sprite; // for nested prefabs with separated sprites take icon from target gui
                    AnimateSprite(spr, pos, 0.5f, item, transform.position, Vector3.down*0.5f+new Vector3(Random.Range(-.5f,.5f),0), ()=>
                    {
                        if (TargetBind.countFromField) Count = CountFromField(TargetBind.hash);
                        else Count--;
                        LevelManager.Instance.CheckMovesAndTargets();
                        action?.Invoke(false);
                    });
                }
            }
            else if(item.fallAfter && !item.fallAnimated)
            {
                var v = new Vector3(-item.transform.position.x * Random.Range(0.1f, 0.5f), 2,0);
                AnimateSprite(spr, pos, 0.7f, item, (item.transform.position+v) + Vector3.down * 10, v);
            }
        }

        private void AnimateSprite(Sprite spr, Vector2 pos, float time, Item item, Vector3 targetPosition, Vector3 randomOffset, Action callback = null)
        {
            item.fallAnimated = true;
            var gm = CreateSprite(spr, pos, out var sprRenderer, 10);
            var componentInChildren = item?.GetComponentInChildren<Icon>();
            if (componentInChildren != null)
            {
                var icon = componentInChildren.spriteRenderer.sprite;
                var h = CreateSprite(icon, pos, out var s1, 11);
                h.transform.SetParent(gm.transform);
            }
            var a2 = sprRenderer.transform.DOMove(targetPosition, time).SetEase(Ease.InQuart).OnComplete(() =>
            {
                callback?.Invoke();
                Destroy(gm);
            });
            sprRenderer.transform.DORotate(Vector3.back* 1000, time*2);
            var sequence = DOTween.Sequence();
            sequence.Append(a2);
            sequence.Play();
        }

        private GameObject CreateSprite(Sprite spr, Vector2 pos, out SpriteRenderer s, int order)
        {
            var gm = new GameObject("target_sprite");
            s = gm.AddComponent<SpriteRenderer>();
            s.tag = "AnimatedTarget";
            s.sprite = spr;
            s.sortingLayerName = "UI";
            s.sortingOrder = order;
            s.transform.position = pos;
            return gm;
        }

        public bool IsDone() => done;
    }
}