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
using System.Collections.Generic;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class LayeredBlock : MonoBehaviour
    {
        public GameObject[] objects;
        public int currentLayer;

        public void ChangeLayer(int n, Action callback = null)
        {
            currentLayer += n;
            var index0 = Mathf.Clamp( currentLayer, 0, objects.Length+1);
  
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
            for (int i = 1; i < index0; i++)
            {
                objects[i-1].SetActive(true);
            }
            if(currentLayer == 0)
                callback?.Invoke();
        }

        public bool AnyLayersExist() => currentLayer > 0;

        public void SetColor(int color)
        {
            foreach (var i in GetComponentsInChildren<ColorComponent>())
            {
                i.SetColor(color);
            }
        }
        public TexPos[] GetTextures(int color)
        {
            List<TexPos> list = new List<TexPos>();
            foreach (var i in objects)
            {
                var pos = new TexPos();
                var colorComponent = i.GetComponent<ColorComponent>();
                if (colorComponent != null) pos.Texture2D = colorComponent.Sprites[color].texture;
                else pos.Texture2D = i.GetComponent<SpriteRenderer>().sprite.texture;
                pos.rotation = i.transform.rotation.eulerAngles;
                pos.scale = i.transform.lossyScale;
                list.Add(pos);
            }

            return list.ToArray();
        }
    }
}