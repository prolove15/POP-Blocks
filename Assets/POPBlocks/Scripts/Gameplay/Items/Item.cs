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
using POPBlocks.Scripts.Pool;
using POPBlocks.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace POPBlocks.Scripts.Items
{
    /// <summary>
    /// Used in all items on the field
    /// </summary>
    [RequireComponent(typeof(PoolBehaviour))]
    public class Item : MonoBehaviour
    {
        //sprite renderer list
        [HideInInspector] public SpriteRenderer[] sr;
        //color component list of all object can have changeable color and matchable
        [HideInInspector] public ColorComponent[] colorComponent;
        //can be used in system object pool
        [Header("Item places into a pool")] public bool canBePooled;
        //can be destroy by explosion effects of bonuses
        [Header("Explode by effects")] public bool explodable = true;
        //don't fall and stay on a place
        [Header("Fixed position on the field (don't fall)")]
        public bool fixedPosition;
        //can be matched with the same color
        [Header("Item is matching by a color")]
        public bool matchable;
        //can be matched with any color
        [Header("Considering as item with any color")]
        public bool anyColor;
        //destroy on bottom row (toy targets)
        [Header("Destroy on bottom row")] public bool destroyOnBottomRow;
        //can be destroyed by near exploded item with the same color
        [Header("Will be destroyed near with item of the same color")]
        public bool coloredObstacle;
        //can be destroyed by near exploded item with any color
        [Header("Will be destroyed near with any item")]
        public bool nonColoredObstacle;
        //can destroy obstacles around
        [Header("Destroy obstacles around")] public bool destroyObstaclesAround;
        //items can fall through this item
        [Header("Items can fall through this item")]
        public bool canFallThrough;
        //animate falling after destroy if the item is not target
        [Header("Fall after destroy")]
        public bool fallAfter;
        [HideInInspector] public bool fallAnimated;
        //can be activated by touch
        [HideInInspector] public bool touchable;

        [Header("Item size in cells")] public Vector2Int size = Vector2Int.one;

        [HideInInspector]
        public int[] spritesHash;
        
        //common sounds
        public AudioClip appearSound;
        public AudioClip activateSound;
        public AudioClip explosionSound;

        //count of matched item with this item
        private int localMatches;
        //bonus icon number
        [HideInInspector] public int bonusNum;
        //is the item was put from editor
        [HideInInspector] public bool premadeFromEditor;
        
        //Color of the item
        //items without color have -1000 color num
        //items with matchable with any color have 999 color num
        public int Color
        {
            get
            {
                if (anyColor) return 999;
                return colorComponent.Length > 0 ? colorComponent[0].color : -1000;
            }
            set { colorComponent.ForEachY(i => i.SetColor(value)); }
        }
        //index in the Field.Instance.squares and Field.Instance.items
        [HideInInspector] public int index;
        //position on the field 
        [HideInInspector] public Vector2Int pos;
        //bonus item reference is not null if the item is bomb, rocket and pinwheel
        [HideInInspector] public BonusItem bonus;
        //moves counter
        public int latestMoveCounter;
        //does the item is not bonus item
        public bool simpleItem;
        //animator reference
        public Animator animator;
        //score amount
        public int score = 100;
        //appearance order purpose
        public int layerMultiplicator;
        //sorting group reference
        [HideInInspector] public SortingGroup sortingGroup;
        //explosion particles
        [Header("Explosion particles")]
        public GameObject itemsParticles;
        //Unity event on touch item
        public UnityEvent OnPress;
        //Unity event on destroy item
        public UnityEvent OnDestroyEvent;
        //sorting layer name for tutorial purpose
        private string sortingLayerName;
        //used for layered items like box with planks
        [HideInInspector]public LayeredBlock layeredBlock;
        //save scale
        private Vector3 saveScale;
        //is item falling
        [HideInInspector]public bool falling;
        private Vector2 targetPosition;
        private IEnumerator moveToCor;
        private float startTimeGlobal;
        public OverlapItem overlapItem => Field.Instance.overlapItems[index];
        private int latestMoveCounterMatch;
        public bool activated; //item is activated by touch

        public delegate void DelEvent(Sprite spr);

        public event DelEvent OnDestroy;

        public delegate void SetIntDel(int layer);

        public event SetIntDel OnSetLayer;
        public event SetIntDel OnMatch;

        public virtual void Awake()
        {
            colorComponent = GetComponentsInChildren<ColorComponent>();
            sr = GetComponentsInChildren<SpriteRenderer>();
            sortingGroup = GetComponent<SortingGroup>();
            bonus = GetComponent<BonusItem>();
            TryGetComponent(out animator);
            layeredBlock = GetComponentInChildren<LayeredBlock>();
        }

        public virtual void OnEnable()
        {
            fallAnimated = false;
            touchable = true;
            targetPosition = Vector2.one * -100;
            startTimeGlobal = 0;
            falling = false;
            activated = false;
            if (appearSound != null && LevelManager.Instance.GameState == GameState.Playing || LevelManager.Instance.GameState == GameState.PreWinAnimation) 
                SoundBase.Instance.PlayOneShot(appearSound);
            animator?.Rebind();
            //fill sprites hash from sr
            var icons = GetComponent<IconEditor>().icon;
            spritesHash = new int[sr.Length + icons.Length];
            for (int i = 0; i < sr.Length; i++)
            {
                spritesHash[i] = sr[i].sprite.texture.GetHashCode();
            }
            for (int i = 0; i < icons.Length; i++)
            {
                spritesHash[sr.Length + i] = icons[i].GetHashCode();
            }
        }

        public virtual void OnDisable()
        {
        }

        public void BackupLayer()
        {
            if(sortingLayerName == null)
                sortingLayerName = sortingGroup.sortingLayerName;
        }
        public void RestoreLayer()
        {
            sortingGroup.sortingOrder = 20 - pos.y;
            sortingGroup.sortingLayerName = sortingLayerName;
        }
        public void SetLayer(int layer, string layerName = "Default")
        {
            sortingGroup.sortingOrder = layer + layerMultiplicator;
            sortingGroup.sortingLayerName = layerName;
            OnSetLayer?.Invoke(layer + 1);
        }

        private void OnMouseDown()
        {
            if(LevelManager.Instance.blockInput) return;
            ActivateItem();
        }

        public virtual void ActivateItem()
        {
            if (!touchable && !GameManager.Instance.boostTypesGame.Any())
                return;
            if (LevelManager.Instance.GameState == GameState.Playing || LevelManager.Instance.GameState == GameState.Tutorial)
            {
                if (CanvasExtensions.IsPointerOverUIObject())
                {
                    return;
                }
                if(activated) return;
                activated = true;
                OnPress?.Invoke();
                LevelManager.Instance.ActivateItem(this);
            }
        }

        public int CheckMatches()
        {
            LevelManager.Instance.matchedSquares.Clear();
            LevelManager.Instance.matchedSquares.Add(this);
            CheckMatches(Color);
            localMatches = LevelManager.Instance.matchedSquares.Count;
            bonusNum = LevelManager.Instance.bonusMatch.BonusMatches.First(i => i.minMatches <= localMatches && i.maxMatches >= localMatches).bonusNum;
            latestMoveCounterMatch = LevelManager.Instance.moveAfterCounter;
            OnMatch?.Invoke(bonusNum);
            return localMatches;
        }

        private void CheckMatches(int color)
        {
    
            var list = Field.Instance.GetItemsCross(this);
            foreach (var item in list)
            {
                if(!item) continue;
                if (!item.matchable || item.falling) continue;
                var nextItemColor = item.Color;
                if ((color == nextItemColor || nextItemColor == 999) && !LevelManager.Instance.matchedSquares.Exists(x => x == item))
                {
                    LevelManager.Instance.matchedSquares.Add(item);
                    item.CheckMatches(color);
                }
            }
        }

        // private void Update()
        // {
        //     if(!fixedPosition)
        //         CheckBelow(pos.x, pos.y);
        // }

        public void CheckBelow(int itemX, int itemY)
        {
            int oldIndex = itemY * Field.Instance.size.x + itemX;
            int indexEmpty = -1;
            int index2 = -1;
            int wideCounter = 0;
            int wideCounterlast = 0;
            Vector2Int newPos = Vector2Int.zero;
            for (int y = itemY + size.y; y < Field.Instance.size.y; y++)
            {
                Item nextitem = null;
                wideCounter = 0;
                Square sq = null;
                OverlapItem over = null;
                for (int x = itemX; x < Mathf.Clamp(itemX + size.x, 0, Field.Instance.size.x); x++)
                {
                    index2 = y * Field.Instance.size.x + x;
                    nextitem = Field.Instance.items[index2];
                    sq = Field.Instance.squares[index2];
                    over = Field.Instance.overlapItems[index2];
                    if (nextitem == null && sq.CanMoveIn() )
                    {
                        if (x == itemX)
                        {
                            indexEmpty = index2;
                        }

                        wideCounter++;
                    }
                }

                if (wideCounter == size.x)
                {
                    wideCounterlast = wideCounter;
                    newPos.x = itemX;
                    newPos.y = y - (size.y - 1);
                }

                if (overlapItem?.fixedItem ?? false) return;
                if (nextitem != null || (wideCounter < size.x && wideCounter > 0) || ((over?.fixedItem ?? false) && over.gameObject.activeSelf)) 
                    break;
            }

            if (indexEmpty <= -1 || wideCounterlast < size.x) return;
            if (size.x > 1 || size.y > 1)
            {
                Field.Instance.FillItemsArray(null, pos.x, pos.y, size, false);
                Field.Instance.FillItemsArray(this, newPos.x, newPos.y, size, false);
            }
            else
            {
                Field.Instance.items[oldIndex] = null;
                Field.Instance.items[indexEmpty] = this;
            }

            index = indexEmpty;
            pos = newPos;
            var ov = Field.Instance.GetSquare(pos).overlapItem;
            MoveTo();
        }

        public void MoveTo()
        {
            SetLayer(20 - (int) pos.y, sortingGroup.sortingLayerName);
            if(animator != null && animator.parameters.Any(x => x.name == "fall"))
                animator?.SetTrigger("fall");
            var speedColumn = Field.Instance.speedColumns[(int) pos.x];
            var newPosition = Square.GetWorldPosition(Field.Instance.squares[index], this);
            if(targetPosition != newPosition && moveToCor!= null)
                StopCoroutine(moveToCor);
            targetPosition = newPosition;
            moveToCor = MoveToCor(targetPosition, speedColumn / 1.5f);
            StartCoroutine(moveToCor);
        }

        IEnumerator MoveToCor(Vector2 destPos, float speed)
        {
            if(startTimeGlobal == 0)
                startTimeGlobal = Time.time;
            var startTime = Time.time;
            var distCovered = (Time.time - startTime) * speed;
            speed = LevelManager.Instance.gameSettings.fallingCurve.Evaluate(0);

            var startPos = transform.position;
            var distance = Vector2.Distance(startPos, destPos);

            float fracJourney = 0;
            while (fracJourney < 1)
            {
                falling = true;
                yield return new WaitUntil(() => LevelManager.Instance.GameState != GameState.Win);
                distCovered = (Time.time - startTime) * speed;
                speed = LevelManager.Instance.gameSettings.fallingCurve.Evaluate(Time.time - startTimeGlobal );
                fracJourney = distCovered / distance;
                transform.position = Vector2.Lerp(startPos, destPos, fracJourney);
                yield return new WaitForEndOfFrame();
                if(fracJourney>0.5 && fracJourney<0.7 && !IsOverlapFixed()) CheckBelow(pos.x,pos.y);
            }
            if(!(overlapItem?.fixedItem??false))
                CheckBelow(pos.x,pos.y);

            StopFall();
        }

        private void UpdateMove()
        {
            falling = true;
            if (Field.Instance.blockColumn[pos.x] && Field.Instance.blockColumn[pos.x].activeSelf) DOTween.Pause(transform);
            else DOTween.Play(transform);
        }

        private void StopFall()
        {
            if (destroyOnBottomRow && Field.Instance.IsBottom(pos, size))
            {
                DestroyItemStart();
                LevelManager.Instance.ActivateItem(this);
            }

            var ov = overlapItem;
            if(ov) touchable = ov.canBeTouch;
            startTimeGlobal = 0;
            falling = false;
            LevelManager.Instance.fallingAnimation = false;
            if(animator != null && animator.parameters.Any(x => x.name == "bounce"))
                animator?.SetTrigger("bounce");
        }


        public virtual void DestroyItemStart(bool noScore = false, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            if (latestMoveCounter >= LevelManager.Instance.moveCounter && latestMoveCounter > 0) return;
            latestMoveCounter = LevelManager.Instance.moveCounter;
            if (bonus != null && !bonus.activated) return;
            layeredBlock?.ChangeLayer(-1, null);
            OnDestroyEvent?.Invoke();
            

            if(!LevelManager.Instance.blockFalling)
                DestroyObstacles(destroyAround);

            DestroyItemFinish(delay, noEffect, effectName, noScore);
        }

        public void DestroyObstacles(bool destroyAround)
        {
            // Field.Instance.overlapItems.Where(i => i != null && i.gameObject.activeSelf && i.pos==pos).ForEachY(i=>i.DestroyObstacle(this));
            if (destroyObstaclesAround && destroyAround)
            {
                DestroyObstaclesAround();
            }
        }

        public void DestroyItemFinish(float delay = 0, bool noEffect = false, string effectName = "", bool noScore = false)
        {
            if (overlapItem != null && !GetComponent<OverlapItem>())
            {
                var overlapItemDestroyWithItem = overlapItem.destroyWithItem;
                overlapItem.DestroyObstacle(this);
                if (!noEffect && (!overlapItemDestroyWithItem || (!overlapItem?.explodable ?? false || !explodable)))
                    return;
            }
            if (Field.Instance.items.Contains(this) && (layeredBlock == null || !layeredBlock.AnyLayersExist()))
                Field.Instance.FillItemsArray(null, pos.x, pos.y, size);
            if (explosionSound != null) SoundBase.Instance.PlayLimitSound(explosionSound);

            if (!noEffect && itemsParticles != null)
            {
                var overrideEffect = itemsParticles.name;
                if (effectName != String.Empty) overrideEffect = effectName;
                var obj = ObjectPooler.Instance.GetPooledObject(overrideEffect);
                if (obj == null) obj = Instantiate(itemsParticles);
                var explosionColors = obj.GetComponent<ExplosionColors>();
                if (explosionColors != null) explosionColors.SetColor(Color);
                obj.transform.position = transform.position;
            }

            if(!noScore)
                LevelManager.Instance.score.IncrementValue(score);
            if (layeredBlock == null || !layeredBlock.AnyLayersExist())
            {
                if (sr is { Length: > 0 })
                {
                    LevelManager.Instance.DestroySpriteEvent(sr[0].sprite, transform.position, this, null);
                }
            }       

            OnDestroy?.Invoke(sr[0].sprite);
            if (layeredBlock == null || !layeredBlock.AnyLayersExist())
                PutBackIntoPool(delay);
        }


        public void SqueezeAnimation(TweenCallback squeeze, TweenCallback unsqueeze, float duration = 0.3f)
        {
            saveScale = transform.localScale;
            transform.DOScale(Vector3.zero, duration).OnComplete(()=>
            {
                squeeze();
                UnSqueezeAnimation(unsqueeze, duration);
            });
        }

        private void UnSqueezeAnimation(TweenCallback callback, float duration = 0.3f)
        {
            transform.DOScale(saveScale, duration).OnComplete(callback);
        }

        public void PutBackIntoPool(float delay = 0)
        {
            SetLayer(1);
            if (delay == 0)
                DeleteDelay();
            else
                StartCoroutine(DeleteDelay(delay));
        }

        private IEnumerator DeleteDelay(float delay = 0)
        {
            transform.position = Vector3.left * 2000;
            yield return new WaitForSeconds(delay);
            Field.Instance.DeleteItem(this);
            ObjectPooler.Instance.PutBack(gameObject);
        }
        
        private void DeleteDelay()
        {
            transform.position = Vector3.left * 2000;
            if(!GetComponent<OverlapItem>())
                Field.Instance.DeleteItem(this);
            ObjectPooler.Instance.PutBack(gameObject);
        }
/// <summary>
/// Destroy near objects marked as an obstacle (colored or non colored)
/// </summary>
        public void DestroyObstaclesAround()
        {
            var itemsAround = Field.Instance.GetItemsCross(this);
            itemsAround.ForEachY(i => i?.DestroyObstacles(this));
        }


        public void DestroyObstacles(Item item)
        {
           if(coloredObstacle || nonColoredObstacle)
               DestroyObstacle(item);
        }

        protected virtual void DestroyObstacle(Item itemDestroyed)
        {
            if (coloredObstacle)
            {
                if (Color < 6 && Color == itemDestroyed.Color)
                    DestroyItemStart();
            }
            else if (nonColoredObstacle)
                DestroyItemStart();
        }

        public virtual void Rotate(Quaternion quaternion)
        {
            transform.localRotation = quaternion;
        }

        public bool IsOverlapFixed()
        {
            return !(overlapItem is null) && overlapItem.fixedItem&& overlapItem.gameObject.activeSelf;
        }
    }
}