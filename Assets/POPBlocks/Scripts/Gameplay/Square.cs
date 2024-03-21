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
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;

namespace POPBlocks.Scripts
{
    public class Square : MonoBehaviour
    {
        public Vector2 size;
        public Vector2Int pos;
        public float offsetBase = 0.33f;
        public Transform sprite;
        private Item _item;
        public float pivotY;
        public BlocksTypes type;
        public bool bottom;
        [Header("Outline options")] public GameObject[] outline;

        public Vector2 offset = new Vector2(0.8f, 0.9f);
        public Vector2 offsetCorner = new Vector2(0.735f, 0.835f);
        public Vector2 offsetInnerCorner = new Vector2(0.9f, 0.9f);
        public OverlapItem overlapItem;

        private void Start()
        {
            SetBorders();
            // sprite.localScale = size * 100;
            sprite.position = GetWorldPosition();
            if(type == BlocksTypes.None) sprite.gameObject.SetActive(false);
            if(bottom) CreateArrow();
        }


        public void AdjustOffset()
        {
            pivotY = transform.position.y + (offsetBase * pos.y);
        }

        public Vector2 GetWorldPosition() => new Vector2(transform.position.x, pivotY);

        public static Vector2 GetWorldPosition(Square square, Item item)
        {
            Vector2 targetPosition = square.GetWorldPosition();
            if (item.size.x > 1 || item.size.y > 1)
            {
                var list = Field.Instance.items.Select((a, i) => new {item = a, index = i}).Where(i => i.item == item).Select(i => i.index).ToArray();
                var worldPosition = Vector2.zero;
                for (int i = 0; i < list.Length; i++) worldPosition += Field.Instance.squares[list[i]].GetWorldPosition();
                targetPosition = worldPosition / list.Length;
            }

            return targetPosition;
        }

        public bool CanMoveIn()
        {
            return type != BlocksTypes.None;
        }

        bool IsLeftEmpty() => pos.x == 0 || (Field.Instance.GetSquare(pos.x - 1, pos.y) == null || Field.Instance.GetSquare(pos.x - 1, pos.y).type == BlocksTypes.None);

        bool IsRightEmpty() => pos.x >= Field.Instance.size.x - 1 || Field.Instance.GetSquare(pos.x + 1, pos.y) == null ||
                               Field.Instance.GetSquare(pos.x + 1, pos.y).type == BlocksTypes.None;

        bool IsUpEmpty() => pos.y == 0 || Field.Instance.GetSquare(pos.x, pos.y - 1) == null || Field.Instance.GetSquare(pos.x, pos.y - 1).type == BlocksTypes.None;

        bool IsDownEmpty() => pos.y >= Field.Instance.size.y - 1 || Field.Instance.GetSquare(pos.x, pos.y + 1) == null ||
                              Field.Instance.GetSquare(pos.x, pos.y + 1).type == BlocksTypes.None;

        bool IsLeftTopEmpty() => (pos.x == 0 && pos.y == 0) ||
                                 (Field.Instance.GetSquare(pos.x - 1, pos.y - 1) == null || Field.Instance.GetSquare(pos.x - 1, pos.y - 1).type == BlocksTypes.None);

        bool IsRightTopEmpty() => (pos.x >= Field.Instance.size.x - 1 && pos.y == 0) ||
                                  (Field.Instance.GetSquare(pos.x + 1, pos.y - 1) == null || Field.Instance.GetSquare(pos.x + 1, pos.y - 1).type == BlocksTypes.None);

        bool IsLeftDownEmpty() => (pos.x == 0 && pos.y >= Field.Instance.size.y - 1) ||
                                  (Field.Instance.GetSquare(pos.x - 1, pos.y + 1) == null || Field.Instance.GetSquare(pos.x - 1, pos.y + 1).type == BlocksTypes.None);

        bool IsRightDownEmpty() => (pos.x >= Field.Instance.size.x - 1 && pos.y >= Field.Instance.size.y - 1) ||
                                   (Field.Instance.GetSquare(pos.x + 1, pos.y + 1) == null || Field.Instance.GetSquare(pos.x + 1, pos.y + 1).type == BlocksTypes.None);

        /// <summary>
        /// Create animated arrow for bottom row
        /// </summary>
        /// <param name="enterPoint"></param>
        public void CreateArrow()
        {
            var obj = Instantiate(Resources.Load("Prefabs/Arrow")) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, sprite.transform.localPosition.y,0) + Vector3.down*0.8f;
            var angle = Vector3.Angle(Vector2.down, Vector2.down);
            angle = Mathf.Sign(Vector3.Cross(Vector2.down, Vector2.down).z) < 0 ? (360 - angle) % 360 : angle;
            Vector2 pos = obj.transform.localPosition;
            pos = Quaternion.Euler(0, 0, angle) * pos;
            obj.transform.localPosition = pos;
            obj.transform.rotation = Quaternion.Euler(0, 0, angle);
            ParticleSystem.MainModule mainModule = obj.GetComponent<ParticleSystem>().main;
            mainModule.startRotation = -angle * Mathf.Deg2Rad;
        }
        
        private void SetBorders()
        {
            if (type != BlocksTypes.None)
            {
                if (IsLeftEmpty())
                {
                    var o = Instantiate(outline[1], transform);
                    o.transform.position = GetWorldPosition() + Vector2.left * offset.x;
                    if (!IsLeftTopEmpty() && !IsLeftDownEmpty())
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 1.5f);
                    else if (!IsLeftDownEmpty() && IsLeftTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.up * 0.3f;
                    }
                    else if (IsLeftDownEmpty() && !IsLeftTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.down * 0.3f;
                    }
                }

                if (IsRightEmpty())
                {
                    var o = Instantiate(outline[1], transform);
                    o.transform.position = GetWorldPosition() + Vector2.right * offset.x;
                    if (!IsRightTopEmpty() && !IsRightDownEmpty())
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 1.5f);
                    else if (!IsRightDownEmpty() && IsRightTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.up * 0.3f;
                    }
                    else if (IsRightDownEmpty() && !IsRightTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.down * 0.3f;
                    }
                }

                if (IsUpEmpty())
                {
                    var o = Instantiate(outline[2], transform);
                    o.transform.position = GetWorldPosition() + Vector2.up * offset.y;
                    if (!IsLeftTopEmpty() && !IsRightTopEmpty())
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 1.8f);
                    else if (!IsRightTopEmpty() && IsLeftTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.left * 0.2f;
                    }
                    else if (IsRightTopEmpty() && !IsLeftTopEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.right * 0.2f;
                    }
                }

                if (IsDownEmpty())
                {
                    var o = Instantiate(outline[2], transform);
                    o.transform.position = GetWorldPosition() + Vector2.down * offset.y;
                    if (!IsRightDownEmpty() && !IsLeftDownEmpty())
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 1.8f);
                    else if (!IsRightDownEmpty() && IsLeftDownEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.left * 0.2f;
                    }
                    else if (IsRightDownEmpty() && !IsLeftDownEmpty())
                    {
                        o.GetComponent<SpriteRenderer>().size = new Vector2(1, 3f);
                        o.transform.localPosition += Vector3.right * 0.2f;
                    }
                }

                if (IsLeftTopEmpty() && IsLeftEmpty() && IsUpEmpty())
                {
                    var o = Instantiate(outline[0], transform);
                    o.transform.position = GetWorldPosition() + Vector2.left * offsetCorner.x + Vector2.up * offsetCorner.y;
                    o.name = "LeftTop";
                }

                if (IsRightTopEmpty() && IsRightEmpty() && IsUpEmpty())
                {
                    var o = Instantiate(outline[0], transform);
                    o.transform.position = GetWorldPosition() + Vector2.right * offsetCorner.x + Vector2.up * offsetCorner.y;
                    o.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    o.name = "RightTop";
                }

                if (IsLeftDownEmpty() && IsLeftEmpty() && IsDownEmpty())
                {
                    var o = Instantiate(outline[0], transform);
                    o.transform.position = GetWorldPosition() + Vector2.left * offsetCorner.x + Vector2.down * offsetCorner.y;
                    o.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    o.name = "LeftBottom";
                }

                if (IsRightDownEmpty() && IsRightEmpty() && IsDownEmpty())
                {
                    var o = Instantiate(outline[0], transform);
                    o.transform.position = GetWorldPosition() + Vector2.right * offsetCorner.x + Vector2.down * offsetCorner.y;
                    o.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    o.name = "RightBottom";
                }
            }


            if (IsLeftTopEmpty() && !IsLeftEmpty() && !IsUpEmpty())
            {
                var o = Instantiate(outline[0], transform);
                o.transform.position = GetWorldPosition() + Vector2.left * offsetInnerCorner.x + Vector2.up * offsetInnerCorner.y;
                o.transform.localRotation = Quaternion.Euler(0, 0, 180);
                o.name = "InnerLeftTop";
            }

            if (IsRightTopEmpty() && !IsRightEmpty() && !IsUpEmpty())
            {
                var o = Instantiate(outline[0], transform);
                o.transform.position = GetWorldPosition() + Vector2.right * offsetInnerCorner.x + Vector2.up * offsetInnerCorner.y;
                o.transform.localRotation = Quaternion.Euler(0, 0, 90);
                o.name = "InnerRightTop";
            }

            if (IsLeftDownEmpty() && !IsLeftEmpty() && !IsDownEmpty())
            {
                var o = Instantiate(outline[0], transform);
                o.transform.position = GetWorldPosition() + Vector2.left * offsetInnerCorner.x + Vector2.down * offsetInnerCorner.y;
                o.transform.localRotation = Quaternion.Euler(0, 0, -90);
                o.name = "InnerLeftBottom";
            }

            if (IsRightDownEmpty() && !IsRightEmpty() && !IsDownEmpty())
            {
                var o = Instantiate(outline[0], transform);
                o.transform.position = GetWorldPosition() + Vector2.right * offsetInnerCorner.x + Vector2.down * offsetInnerCorner.y;
                o.name = "InnerRightBottom";
            }
        }
    }
}