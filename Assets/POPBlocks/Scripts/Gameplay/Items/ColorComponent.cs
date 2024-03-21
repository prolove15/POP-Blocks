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
using UnityEngine;

// [ExecuteAlways]
namespace POPBlocks.Scripts.Items
{
    public class ColorComponent : MonoBehaviour
    {
        public int color;

        public List<Sprite> Sprites = new List<Sprite>();

        public delegate void ColorChangeDel(int color);

        public event ColorChangeDel OnColorChanged;
        public SpriteRenderer spriteRenderer;
        private ColorComponent[] colorComponents;
        public bool RandomColorOnAwake = true;

        private void Awake()
        {
            colorComponents = GetComponentsInChildren<ColorComponent>();
        }

        private void OnEnable()
        {
            if (RandomColorOnAwake)
                RandomizeColor(new int[0]);
        }


        public void SetColor(int _color)
        {
            spriteRenderer.sprite = Sprites[Mathf.Clamp(_color,0, Sprites.Count-1)];
            color = _color;
            OnColorChanged?.Invoke(_color);
        }

        public void RandomizeColor(int[] exceptColors)
        {
            color = ColorGenerator.GenColor(LevelManager.Instance.maxColors, exceptColors);
            SetColor(color);
            foreach (var i in colorComponents) i.SetColor(color);
        }
    }
}