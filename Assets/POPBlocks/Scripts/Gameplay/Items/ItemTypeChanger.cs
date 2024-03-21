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

using POPBlocks.Scripts.LevelHandle;
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class ItemTypeChanger : MonoBehaviour
    {
        private Item item;

        private void Awake()
        {
            item = GetComponent<Item>();
        }

        public void SetRocket( bool hideInstantly = false)
        {
            SetType("Rocket", hideInstantly);
        }


        public void SetBomb( bool hideInstantly = false)
        {
            SetType("Bomb", hideInstantly);
        }

        public void SetPinwheel( bool hideInstantly = false)
        {
            SetType("Pinwheel", hideInstantly);
        }

        public void SetIngredient( bool hideInstantly = false)
        {
            SetType("Ingredient", hideInstantly);
        }

        public void SetIngredientBig( bool hideInstantly = false)
        {
            SetType("IngredientBig", hideInstantly);
        }

        public void SetBallItem( bool hideInstantly = false)
        {
            SetType("BallItem", hideInstantly);
        }

        public Item SetType(string name, bool hideInstantly = false)
        {
            if (hideInstantly)
                item.PutBackIntoPool();
            else
                item.DestroyItemStart(true, 0, true,"",false);
            var item1 = Field.Instance.CreateItem(name, item.index, item.pos);
            if (item1.bonus != null && item1.bonus.type == BonusItemTypes.Pinwheel) item1.Color = item.Color;
            return item1;
        }
    }
}