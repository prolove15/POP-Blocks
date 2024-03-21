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
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.Items.BonusItems;
using POPBlocks.Scripts.Pool;
using POPBlocks.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace POPBlocks.Scripts.LevelHandle
{
    public class Field : MonoBehaviour
    {
        public static Field Instance;
        public Vector2Int size;
        public Square[] squares;
        public Item[] items;
        public OverlapItem[] overlapItems;
        public Transform fieldPivot;
        public GameObject squarePrefab;
        [SerializeField] public Rect worldRect;
        private Vector2 sqSize;
        public float[] speedColumns;
        readonly Vector2Int[] around = {new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)};
        private Level level;
        public List<TargetSprite> targets = new List<TargetSprite>();
        public GameObject outline3;
        public GameObject outline2;
        public GameObject outline1;
        private bool ai;
        public bool AI
        {
            set
            {
                ai = value;
                if (ai) aiCoroutine = StartCoroutine(AIPlay(0.5f));
                else if (aiCoroutine != null) StopCoroutine(aiCoroutine);
            }
            get => ai;
        }
        private Coroutine aiCoroutine;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
            cameraHandler = Camera.main.GetComponent<CameraHandler>();
        }

        // Start is called before the first frame update
        public void LoadLevel(Level level)
        {
            PrepareTargets(level);
            this.level = level;
            size = level.size;
            blockColumn = new GameObject[size.x];
            speedColumns = new float[size.x];
            GenField();
            transform.position = transform.position + (fieldPivot.transform.position - (Vector3) GetCenter());
            GenItems(false);
            PreCreateItems();
            CorrectMatches(items.ToList());
            CreateArrows();
            LevelManager.Instance.GameState = GameState.Prepare;
            CheckMatches();
            StartCoroutine(RareUpdate());
            AI = LevelManager.Instance.DebugSettings.AI;
            StartCoroutine(CheckNoMatch());
        }

        private void CreateArrows()
        {
            if (items.WhereNotNull().Any(i => i.destroyOnBottomRow))
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        var sq = GetSquare(x, y);
                        if (sq.CanMoveIn())
                        {
                            sq.bottom = true;
                            break;
                        }
                    }
                }
            }
        }

        //prevent to generate big matches sequences
        public void CorrectMatches(List<Item> newItems)
        {
            CheckMatches();
            StartCoroutine(CorrectMatchesCoroutine(LevelManager.Instance.bonusMatch.BonusMatches.First(i=>i.bonusNum==0).maxMatches));
        }

        private IEnumerator CorrectMatchesCoroutine(int maxMatchesNonBonus)
        {
            while (Matcheses().Any() && LevelManager.Instance.GameState != GameState.CameraAnimation)
            {
                yield return new WaitForFixedUpdate();
                var matchesEnumerable = Matcheses();
                foreach (var matches in matchesEnumerable)
                {
                    foreach (var item in matches.items)
                    {
                        if(!item.premadeFromEditor)
                        {
                            var exceptColors = GetExceptColors(item);
                            item.colorComponent[0].RandomizeColor(exceptColors);
                        }
                    }
                }
                CheckMatches();
            }

            IEnumerable<Matches> Matcheses()
            {
                return LevelManager.Instance.matches.Where(i => i.items.Count(x =>x/* !x.premadeFromEditor*/) > maxMatchesNonBonus);
            }
        }

        private int[] GetExceptColors(Item item)
        {
            var c = GetItemsAround(item.pos, 2).Select(i => i.Color).GroupBy(i => i).Select(i => new {Color = i.Key, Count = i.Count()});
            var exceptColors = c.Where(i => i.Count > 1).OrderByDescending(i => i.Count).Take(LevelManager.Instance.maxColors - 2).Select(i => i.Color).ToArray();
            return exceptColors;
        }

        private void PrepareTargets(Level level)
        {
            foreach (var target in level.targets)
            {
                var targetSprite = new TargetSprite();
                for (var i = 0; i < target.target.targetSprites.Count; i++)
                {
                    if (target.count.values[i] == 0 && !target.target.countFromField) continue;
                    var spr = target.target.targetSprites[i];
                    targetSprite.sprites.Add(spr.icon);
                    if (spr.uiSprite)
                    {
                        targetSprite.count = target.count.values[i];
                        targetSprite.countFromField = target.target.countFromField;
                        targetSprite.sprites.AddRange(target.target.targetSprites.Where(x => !x.uiSprite).Select(x => x.icon).ToList());
                        targetSprite.prefab = target.target.prefab;
                        targets.Add(targetSprite);
                        targetSprite = new TargetSprite();
                    }
                }
            }
        }

        private void PreCreateItems()
        {
            for (int x = 0; x < size.x; x++)
            {
                int verticalIndex = 0;
                for (int y = size.y - 1; y >= 0; y--)
                {
                    var index = y * size.x + x;
                    var block = level.Blocks[y * size.x + x];
                    if (block.type == BlocksTypes.Item)
                    {
                        var item = CreateItem(block.prefab.name, index, new Vector2Int(x, y), false, verticalIndex);
                        item.Color = block.color;
                        item.Rotate(block.rotate ? Quaternion.Euler(0, 0, -90) : Quaternion.identity);
                        item.premadeFromEditor = true;
                        if (item.layeredBlock != null)
                        {
                            item.layeredBlock.SetColor(item.Color);
                            item.layeredBlock.ChangeLayer(block.layer);
                        }

                        if (block.overlapBlockObject != null)
                        {
                            CreateOverlapItem(items[y * size.x + x], block.overlapBlockObject.name);
                        }

                        verticalIndex++;
                    }
                    else if (block.type == BlocksTypes.Overlap)
                    {
                        CreateOverlapItem(items[y * size.x + x], block.prefab.name);
                    }
                }
            }

            foreach (var item in items.WhereNotNull())
            {
                for (int y = item.pos.y - 1; y >= 0; y--)
                {
                    var item1 = GetItem(item.pos.x, y);
                    if (item1 != null && (item1.fixedPosition || item1.IsOverlapFixed()) && !item.premadeFromEditor)
                    {
                        item.PutBackIntoPool();
                        break;
                    }
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && !items[i].gameObject.activeSelf)
                    items[i] = null;
            }

            // items[15].GetComponent<ItemDebug>().SetIngredientBig();
            // CreateOverlapItem(items[10]);
            // items[15].GetComponent<ItemTypeChanger>().SetBallItem();
            // items[15].Color = 6;
            // items[15].GetComponent<ItemTypeChanger>().SetType("Aquarium");
            // items[15].GetComponent<ItemTypeChanger>().SetType("JellySpreading");
        }

        private void CreateOverlapItem(Item item, string prefabName)
        {
            var pooledObject = GetPooledObject(prefabName);
            var overlapItem = pooledObject.GetComponent<OverlapItem>();
            overlapItems[item.index] = overlapItem;
            GetSquare(item.pos).overlapItem = overlapItem;
            item.touchable = overlapItem.canBeTouch;
            overlapItem.index = GetIndex(item);
            // overlapItem.ItemUnder = item;
            overlapItem.pos = item.pos;
            pooledObject.transform.position = item.transform.position;
            overlapItem.sortingGroup.sortingOrder = item.sortingGroup.sortingOrder + 100;
        }

        private void GenField()
        {
            squares = new Square[size.x * size.y];
            items = new Item[size.x * size.y];
            overlapItems = new OverlapItem[size.x * size.y];

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    CreateSquare(x, y);
                }
            }
        }

        public List<Item> GenItems(bool fall = true)
        {
            List<Item> newItems = new List<Item>();
            for (int x = 0; x < size.x; x++)
            {
                // if(blockColumn[x] && blockColumn[x].activeSelf) continue;;
                int verticalIndex = 0;
                for (int y = GetBottomEmptySquare(x); y >= 0; y--)
                {
                    var index = y * size.x + x;
                    var indexBelow = y + 1 * size.x + x;
                    if (items[index] == null)
                    {
                        var item = CreateItem("Item", index, new Vector2Int(x, y), fall, verticalIndex);
                        newItems.Add(item);
                        if (y < size.y && LevelManager.Instance.GameState == GameState.Playing && items[indexBelow] != null && item != null)
                        {
                            item.Color = ColorGenerator.GenColor(LevelManager.Instance.maxColors, GetExceptColors(item));
                        }

                        verticalIndex++;
                    }
                }
            }

            return newItems;
        }

        int GetBottomEmptySquare(int x)
        {
            for (int y = 0; y < size.y; y++)
            {
                var index = y * size.x + x;
                if (items[index] != null && !items[index].canFallThrough) return y - 1;
                if (overlapItems[index] != null && !overlapItems[index].canFallThrough) return y;
            }

            return size.y - 1;
        }

        public bool IsBottom(Vector2Int pos, Vector2Int size)
        {
            bool isBottom = false;
            for (int x = pos.x; x < size.x + pos.x; x++)
            {
                for (int y = pos.y; y < size.y + pos.y; y++)
                {
                    isBottom = squares.First(i => i.pos == new Vector2Int(x, y)).bottom;
                }
            }

            return isBottom;
        }

        public void FillItemsArray(Item item, int x, int y, Vector2Int itemSize, bool destroyItems = true)
        {
            int i = 0;
            for (int iX = 0; iX < itemSize.x; iX++)
            {
                for (int iY = 0; iY < itemSize.y; iY++)
                {
                    var expandedPos = new Vector2Int(x + iX, y + iY);
                    if (expandedPos.x < size.x && expandedPos.y < size.y)
                    {
                        var index0 = expandedPos.y * size.x + expandedPos.x;
                        var itemToDelete = items[index0];
                        if (itemToDelete != null && destroyItems) ObjectPooler.Instance.PutBack(itemToDelete.gameObject);
                        items[index0] = item;
                    }

                    i++;
                }
            }
        }

        public IEnumerator RareUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                yield return new WaitWhile(() => LevelManager.Instance.blockFalling);
                FallAndGenerateItems();
            }
        }

        public void FallAndGenerateItems()
        {
            PrepareFallItems();
            var list = GenItems();
            // CorrectMatches(list);
            // PrepareFallItems();
            // FallItems();
            // Field.Instance.CheckMatches();
        }

        private void Update()
        {
            if (!LevelManager.Instance.levelLoaded) return;
            Field.Instance.CheckMatches();
            LevelManager.Instance.CheckMovesAndTargets();
        }

        IEnumerator CheckNoMatch()
        {
            while (true)
            {
          
                if (NoMatchCondition())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        yield return new WaitForSeconds(0.5f);
                        yield return new WaitWhile(() => Field.Instance.items.Any(x => x?.falling ?? false));
                        yield return new WaitWhile(() => GameObject.FindWithTag("AnimatedTarget"));
                        if (!NoMatchCondition())
                            break;
                    }
                    if (NoMatchCondition())
                    {
                        LevelManager.Instance.GameState = GameState.Regen;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }

            bool NoMatchCondition()
            {
                return LevelManager.Instance.matches.Count == 0 && !Field.Instance.items.Any(i=>i != null && i.bonus) && LevelManager.Instance.GameState == GameState.Playing;
            }
        }

        IEnumerator AIPlay(float wait)
        {
            while (true)
            {
                var bonuses = Field.Instance.items.Where(i=>i != null && i.bonus);
                if(bonuses.Any()) bonuses.NextRandom().ActivateItem();
                else if(LevelManager.Instance.matches.Any() && LevelManager.Instance.GameState == GameState.Playing)
                    LevelManager.Instance.matches.OrderByDescending(i=>i.items.Length).Take(2).NextRandom().items.Where(i=>i.touchable).NextRandom()?.ActivateItem();
                yield return new WaitForSeconds(wait);
                #if !UNITY_GAME_SIMULATION
                if(LevelManager.Instance.GameState == GameState.Prefailed || LevelManager.Instance.GameState == GameState.PreWin) LevelManager.Instance.RestartLevel();
                #endif
            }
        }

        private void FallItems()
        {
            items.Where(i => !(i is null) && i.falling).ForEachY(i => i.MoveTo());
        }

        public GameObject[] blockColumn;
        public CameraHandler cameraHandler;

        public void PrepareFallItems()
        {
            for (var index = 0; index < speedColumns.Length; index++) speedColumns[index] = Random.Range(LevelManager.Instance.speed, LevelManager.Instance.speed + 5f);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = size.y - 1; y >= 0; y--)
                {
                    // if (blockColumn[x] && blockColumn[x].activeSelf) continue;
                    var item = items[y * size.x + x];
                    if (item != null && item.pos.x == x && item.pos.y == y && !item.fixedPosition && !(item.IsOverlapFixed()))
                    {
                        if (!item.falling) item.CheckBelow(x, y);
                        else break;
                    }
                }
            }
        }


        private void CreateSquare(int x, int y)
        {
            var square = Instantiate(squarePrefab.GetComponent<Square>(), transform);
            square.name = "Square_" + x + "_" + y;
            square.transform.localPosition = SetSquarePosition(x, y, square.size);
            square.pos = new Vector2Int(x, y);
            sqSize = square.size;
            squares[y * size.x + x] = square;
            square.type = level.Blocks[y * size.x + x].type;

            square.AdjustOffset();
        }

        private Item CreateItem(string name, int index, Vector2Int pos, bool fall, int verticalIndex)
        {
            Square square = squares[index];
            if (!square.CanMoveIn()) return null;
            var pooledObject = GetPooledObject(name);

            var item = pooledObject.GetComponent<Item>();
            FillItemsArray(item, pos.x, pos.y, item.size);
            var targetPosition = Square.GetWorldPosition(square, item);
            item.transform.position = targetPosition;
            item.index = index;
            item.pos = pos;
            item.SetLayer(20 - pos.y, item.sortingGroup.sortingLayerName);
            if (fall)
            {
                var position = item.transform.position;
                position = new Vector2(position.x, worldRect.yMax * 1.08f + verticalIndex * square.size.y);
                item.transform.position = position;
                item.MoveTo();
            }

            return item;
        }

        private GameObject GetPooledObject(string name)
        {
            var pooledObject = ObjectPooler.Instance.GetPooledObject(name);
            if (pooledObject == null)
            {
                Debug.LogError(name + " not found in pool");
                return pooledObject;
            }

            return pooledObject;
        }

        public int GetIndex(Item item) => item.pos.y * size.x + item.pos.x;

        public Item CreateItem(string prefabName, int index, Vector2Int pos)
        {
            return CreateItem(prefabName, index, pos, false, 0);
        }

        Vector3 SetSquarePosition(int x, int y, Vector2 squareSize)
        {
            var halfX = size.x / 2f;
            var halfY = size.y / 2f;
            var x1 = (x - halfX);
            var y1 = (-y + halfY);
            var squarePosition = new Vector3(x1 * squareSize.x, y1 * squareSize.y);
            return squarePosition;
        }

        public Vector2 GetCenter()
        {
            worldRect = GetWorldRect();
            return worldRect.center;
        }

        private Rect GetWorldRect()
        {
            var minX = squares.Min(x => x.GetWorldPosition().x);
            var minY = squares.Min(x => x.GetWorldPosition().y);
            var maxX = squares.Max(x => x.GetWorldPosition().x);
            var maxY = squares.Max(x => x.GetWorldPosition().y);
            return Rect.MinMaxRect(minX - sqSize.x / 2, minY - sqSize.y / 2, maxX + sqSize.x / 2, maxY + sqSize.y / 2);
        }

        public Square GetSquare(Vector2Int pos)
        {
            return GetSquare(pos.x, pos.y);
        }

        public Square GetSquare(int x, int y, bool notNull = false)
        {
            if (!notNull)
            {
                if (x >= size.x || y >= size.y || x < 0 || y < 0)
                    return null;
                return squares[y * size.x + x];
            }

            var row = Mathf.Clamp(y, 0, size.y - 1);
            var col = Mathf.Clamp(x, 0, size.x - 1);
            return squares[row * size.x + col];
        }

        public void CheckMatches(int matchAmount = 2)
        {
            LevelManager.Instance.matches.Clear();
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var item = items[y * size.x + x];
                    if (item != null && item.matchable && !item.falling)
                    {
                        if (item.CheckMatches() >= matchAmount)
                        {
                            LevelManager.Instance.AddMatches();
                        }
                    }
                }
            }
        }


        public Item[] GetItemsCross(Item item)
        {
            Item[] list = new Item[4];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = null;
            }
            var l = GetCrossVectors(item.pos);
            for (var i = 0; i < l.Length; i++)
            {
                var vector2Int = l[i];
                if (vector2Int == Vector2Int.left*1000) continue;
                var nextItemIndex = vector2Int.y * Instance.size.x + vector2Int.x;
                var item1 = Instance.items[nextItemIndex];
                var sq = squares[nextItemIndex];
                if (item1 != null) list[i] = item1;
                else if (item1 == null && sq != null && sq.overlapItem != null && sq.overlapItem.gameObject.activeSelf) list[i] = sq?.overlapItem;
            }

            return list; 
        }
        public Vector2Int[] GetCrossVectors(Vector2Int pos)
        {
            Vector2Int[] v = new Vector2Int[4];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = Vector2Int.left*1000;
            }
            for (int i = 0; i < around.Length; i++)
            {
                Vector2Int newPos = pos + around[i];
                if (!IsInsideField(newPos)) continue;
                v[i] = newPos;
            }
            return v;
        }

        public bool IsInsideField(Vector2Int newPos)
        {
            return newPos.x >= 0 && newPos.x < Instance.size.x && newPos.y >= 0 && newPos.y < Instance.size.y;
        }

        // public int GetIndex(Item itemMatching) => Array.IndexOf(items, itemMatching);

        public Vector2Int GetPosition(Item item, int index)
        {
            int y = index / Instance.size.x;
            int x = index - y * Instance.size.x;
            return new Vector2Int(x, y);
        }

        public Item GetItem(int x, int y)
        {
            if (x < 0 || x >= size.x) return null;
            if (y < 0 || y >= size.y) return null;
            var index = y * size.x + x;
            if (index < 0 || index >= items.Length) return null;
            return items[index];
        }

        public Vector2 GetWorldPosition(Vector2Int pos) => GetSquare(pos).GetWorldPosition();

        public List<Item> GetItemsAround(Vector2Int pos, int radius = 1)
        {
            List<Item> list = new List<Item>();
            for (int x = pos.x - radius; x <= pos.x + radius; x++)
            {
                for (int y = pos.y - radius; y <= pos.y + radius; y++)
                {
                    var item1 = Field.Instance.GetItem(x, y);
                    if (item1 != null)
                        list.Add(item1);
                }
            }

            return list;
        }

        public void DestroyRow(int row, Item itemExcept = null)
        {
            GetRow(row).Where(i => i.explodable && i != itemExcept).ForEachY(i => i.DestroyItemStart());
        }

        public Item[] GetRow(int row) => items.Where(i => i != null && i.pos.y == row).ToArray();
        public Item[] GetColumn(int col) => items.Where(i => i != null && i.pos.x == col).ToArray();

        public void DestroyColumn(int col, Item itemExcept = null)
        {
            GetColumn(col).Where(i => i.explodable && i != itemExcept).ForEachY(i => i.DestroyItemStart());
        }

        public void DestroyAround(Item item)
        {
            
            int score = 0;
            for (int x = item.pos.x - 1; x <= item.pos.x + 1; x++)
            {
                // if (x >= 0 && x < size.x)
                    // Field.Instance.blockColumn[x] = item.gameObject;
                for (int y = item.pos.y - 1; y <= item.pos.y + 1; y++)
                {
                    var item1 = Field.Instance.GetItem(x, y);
                    if (item1 != null && (item1 != item && item1.explodable))
                    {
                        if (item1.bonus != null && LevelManager.Instance.GameState == GameState.PreWinAnimation) continue;
                        score += item1.score;
                        item1.DestroyItemStart();
                    }
                    else 
                    {
                        item1?.overlapItem?.DestroyItemFinish();
                    }
                }
            }
            item.DestroyObstacles(false);
            item.DestroyItemFinish(0, false, "", false);

            LevelManager.Instance.ShowScorePop(score, item.transform.position);
        }

        public void DestroyColor(Item item)
        {
            int score = 0;
            var list = Field.Instance.items.Where(i => i != null && i.matchable && i.Color == item.Color);
            foreach (var item1 in list)
            {
                if (item1 != null && item1.explodable)
                {
                    item1.DestroyItemStart();
                    score += item1.score;
                }
            }

            LevelManager.Instance.ShowScorePop(score, item.transform.position);

            item.DestroyItemStart();
        }

        public void DestroyBombRocket(Item item1, Item item2, Action callback)
        {
            Camera.main.transform.DOShakePosition(0.2f, Vector3.one, 25, 180);

            int score = 0;

            for (int y = 0; y < Field.Instance.size.y; y++)
            {
                var item = Field.Instance.GetItem(item1.pos.x - 1, y);
                if (item != null && item.explodable)
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int y = 0; y < Field.Instance.size.y; y++)
            {
                var item = Field.Instance.GetItem(item1.pos.x, y);
                if (item != null && item.explodable)
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int x = 0; x < Field.Instance.size.x; x++)
            {
                var item = Field.Instance.GetItem(x, item1.pos.y - 1);
                if (item != null && item.explodable)
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int x = 0; x < Field.Instance.size.x; x++)
            {
                var item = Field.Instance.GetItem(x, item1.pos.y);
                if (item != null && item.explodable)
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            LevelManager.Instance.ShowScorePop(score, item1.transform.position);
            item1.overlapItem.DestroyItemStart();

            // item1.DestroyItem();
            // item2?.DestroyItem();
            callback?.Invoke();
        }

        public void DestroyRocketRocket(Item item1, Item item2, Action callback)
        {
            int score = 0;
            int xOffset = 1;
            int yOffset = 1;
            if (item1.pos.x > item2.pos.x) xOffset = -1;
            if (item1.pos.y > item2.pos.y) xOffset = -1;
            for (int y = 0; y < Field.Instance.size.y; y++)
            {
                var item = Field.Instance.GetItem(item1.pos.x + xOffset, y);
                if (item != null && item.explodable && (item != item1 && item != item2))
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int y = 0; y < Field.Instance.size.y; y++)
            {
                var item = Field.Instance.GetItem(item1.pos.x, y);
                if (item != null && item.explodable && (item != item1 && item != item2))
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int x = 0; x < Field.Instance.size.x; x++)
            {
                var item = Field.Instance.GetItem(x, item1.pos.y + yOffset);
                if (item != null && item.explodable && (item != item1 && item != item2))
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            for (int x = 0; x < Field.Instance.size.x; x++)
            {
                var item = Field.Instance.GetItem(x, item1.pos.y);
                if (item != null && item.explodable && (item != item1 && item != item2))
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            LevelManager.Instance.ShowScorePop(score, item1.transform.position);

            callback?.Invoke();
        }

        public void DestroyBombBomb(Vector2Int pos, Item item1, Item item2, Action callback)
        {
            Camera.main.transform.DOShakePosition(0.2f, Vector3.one, 25, 180);

            item1.bonus.activated = true;
            item2.bonus.activated = true;
            int score = 0;
            var list = GetItemsAround(pos, 2).Where(BonusItem.NotAnyIsBonus(item1, item2)).Where(i=>i!=item1).ToArray();
            score = list.Sum(i => i.score);
            list.ForEachY(i => i.DestroyItemStart());
            LevelManager.Instance.ShowScorePop(score, GetWorldPosition(pos));

            // item1?.DestroyItem();
            // item2?.DestroyItem();
            callback?.Invoke();
        }

        public void DestroyBombRocket(BonusItem item1, BonusItem item2, Action callback, float time)
        {
            StartCoroutine(AnimateBombRocket(item1, item2, callback, time));
        }

        private IEnumerator AnimateBombRocket(BonusItem item1, BonusItem item2, Action callback, float time)
        {
            var v1 = item1.transform.position;
            var rotatedRocketPos = v1;
            item2.transform.DOMove(rotatedRocketPos, time);
            yield return new WaitForSeconds(time);
            item1.PutBackIntoPool();
            item2.PutBackIntoPool();
            var rocketBombEffect = Instantiate(Resources.Load<BonusRocketEffectData>("DoubleBonusPrefabs/RocketBomb"));
            rocketBombEffect.bonusItems = new[] {item1, item2};
            rocketBombEffect.transform.position = v1;
            rocketBombEffect.callBack = callback;
            rocketBombEffect.Pos = item1.pos;
            yield return new WaitForSeconds(time*2);
            // var itemsCross = Field.Instance.GetItemsAround(item1);
            // var list = itemsCross.SelectMany(i => Field.Instance.GetRow(i.pos.y))
                // .Concat(itemsCross.SelectMany(i => Field.Instance.GetColumn(i.pos.x))).Distinct().Where(BonusItem.NotAnyIsBonus(item1, item2)).ToArray();
            // list.ForEachY(i => i.DestroyItem());
            // yield return new WaitForSeconds(time);
            // Field.Instance.DestroyBombBomb(item1, null, callback);
        }

        public Item[] GetDoubleRocketLaunchHorizontal(Vector2Int pos)
        {
            Item[] items;
            Vector2Int[] l = new Vector2Int[3];
            l[0] = Field.Instance.IsInsideField(pos + Vector2Int.left) ? pos + Vector2Int.left : pos;
            l[1] = Field.Instance.IsInsideField(pos + Vector2Int.zero) ? pos + Vector2Int.zero : pos;
            l[2] = Field.Instance.IsInsideField(pos + Vector2Int.right) ? pos + Vector2Int.right : pos;
            items = l.SelectMany(i => Field.Instance.GetColumn(i.x)).Distinct().ToArray();
            return items;
        }

        public Item[] GetDoubleRocketLaunchVertical(Vector2Int pos)
        {
            Item[] items;
            Vector2Int[] l = new Vector2Int[3];
            l[0] = Field.Instance.IsInsideField(pos + Vector2Int.up) ? pos + Vector2Int.up : pos;
            l[1] = Field.Instance.IsInsideField(pos + Vector2Int.zero) ? pos + Vector2Int.zero : pos;
            l[2] = Field.Instance.IsInsideField(pos + Vector2Int.down) ? pos + Vector2Int.down : pos;
            items = l.SelectMany(i => Field.Instance.GetRow(i.y)).Distinct().ToArray();
            return items;
        }

        public List<Item> GetRandomItems(int count, bool simpleItem, bool notNear)
        {
            var list = items.ToList();
            if (simpleItem) list = items.WhereNotNull().Where(i => i.CompareTag("Item")).ToList();
            var list2 = new List<Item>();
            int i = 0;
            while (list2.Count < Mathf.Clamp(count, 0, items.Count()))
            {
                i++;
                if (i > 100) notNear = false;
                if(i>1000)
                {
                    if (count > list2.Count) break;
                }
                if (!list.Any()) return list;
                var newItem = list[Random.Range(0, list.Count)];
                if(notNear && (list2.Any(i=> Mathf.Abs(i.pos.x - newItem.pos.x)<=0) || list2.Any(i=> Mathf.Abs(i.pos.y - newItem.pos.y)<=0))) continue;
                if (list2.IndexOf(newItem) < 0)
                {
                    list2.Add(newItem);
                }
            }

            return list2;
        }

        public void DestroyPinWheel(Item pinWheelBonus, Item secondBonus, Action callback)
        {
            StartCoroutine(DestroyPinWheelCor(pinWheelBonus, secondBonus, callback));
        }

        private IEnumerator DestroyPinWheelCor(Item pinWheelBonus, Item secondBonus, Action callback)
        {
            int score = 0;
            var list = Field.Instance.items.Where(i => i != null && i.matchable && i.Color == pinWheelBonus.Color);
            List<Item> list2 = new List<Item>();
            foreach (var item1 in list)
            {
                var item = Field.Instance.CreateItem(secondBonus.name, item1.index, item1.pos);
                list2.Add(item);
                LevelManager.Instance.destroyingBonusesList.Add(item);
                item1.DestroyItemStart();
            }

            yield return new WaitForSeconds(0.2f);
            foreach (var item in list2)
            {
                if (item != null && item.explodable)
                {
                    item.DestroyItemStart();
                    score += item.score;
                }
            }

            LevelManager.Instance.ShowScorePop(score, pinWheelBonus.transform.position);

            // pinWheelBonus.DestroyItem();
            secondBonus.DestroyItemStart();
            callback?.Invoke();
        }

        public void PinwheelEffectFinish(Action callback, List<Item> list2, int sc, Item item, Item secondBonus = null)
        {
            StartCoroutine(PinwheelEffect(callback, list2, sc, item, secondBonus));
        }
        
        IEnumerator PinwheelEffect(Action callback, List<Item> list2, int sc, Item item, Item secondBonus = null)
        {
            secondBonus?.DestroyItemFinish();
            item.DestroyItemFinish();
            int countBonuses = 0;
            foreach (var item1 in list2)
            {
                if (item1 != null && item1.gameObject.activeSelf && item1.explodable)
                {
                    item1.bonus.ActivateBonus(false, ()=>countBonuses++);
                    yield return new WaitForSeconds(0.5f);
                }
                
            }

            yield return new WaitUntil(() => countBonuses >= list2.Count);
            LevelManager.Instance.ShowScorePop(sc, transform.position);


            callback?.Invoke();
        }

        public void DeleteItem(Item item)
        {
            int oldIndex = item.pos.y * Field.Instance.size.x + item.pos.x;
            items[oldIndex] = null;
        }

        public Item[] GetNearItems(Bounds boxBounds)
        {
            return items.Where(i => !(i is null) && boxBounds.Contains(i.transform.position)).ToArray();
        }

        private Item GetItem(Vector2Int pos) => GetItem(pos.x, pos.y);

        public void AnimateWave(Vector2 itemPos)
        {
            // DOVirtual.DelayedCall(.5f, () =>LevelManager.Instance.bonusAppearAnimate = false);
            var expl = Instantiate(Resources.Load<GameObject>("Effects/explosive_effect"));
            expl.transform.position = itemPos;
            var wave = Instantiate(Resources.Load<CircleWave>("Effects/WaveEffect"));
            wave.transform.position = itemPos;
            wave.OnComplete += () =>
            {
                LevelManager.Instance.blockFalling = false;
                Destroy(wave.gameObject);
            };
        }
    }
}