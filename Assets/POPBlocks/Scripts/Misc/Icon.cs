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

using System.Collections.Generic;
using POPBlocks.Scripts.Items;
using UnityEngine;

namespace POPBlocks.Scripts
{
    public class Icon : MonoBehaviour
    {
        public List<SpriteIcon> sprites = new List<SpriteIcon>();
        public SpriteRenderer spriteRenderer;
        public ColorComponent colorComponent;
        private int iconNum;
        private int color;
        private Item item;
        private void Awake()
        {
            item = GetComponentInParent<Item>();
            colorComponent.OnColorChanged += OnColorChanged;
            item.OnSetLayer += SetLayer;
            item.OnMatch += SetIcon;
        }

        private void OnEnable()
        {
            colorComponent.OnColorChanged += OnColorChanged;
            item.OnSetLayer += SetLayer;
            item.OnMatch += SetIcon;
        }

        private void OnDisable()
        {
            colorComponent.OnColorChanged -= OnColorChanged;
            item.OnSetLayer -= SetLayer;
            item.OnMatch -= SetIcon;
        }

        public void SetLayer(int layer)
        {
            spriteRenderer.sortingOrder = layer;
        }

        private void OnColorChanged(int color)
        {
            this.color = color;
            UpdateIcon();
        }

        public void SetIcon(int i)
        {
            iconNum = i;
            UpdateIcon();
        }

        void UpdateIcon()
        {
            spriteRenderer.sprite = sprites[iconNum]._sprites[color];
        }
    }

    [System.Serializable]
    public class SpriteIcon
    {
        public Sprite[] _sprites;
    }
}