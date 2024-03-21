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
using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class Aquarium : Item
    {
        [SerializeField] private Sprite fishSprite;
        public GameObject fishEffect;
        public AudioSource audioSource;

        public override void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            base.Awake();
        }

        public override void DestroyItemStart(bool noScore, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            DestroyObstacle(this);
        }

        protected override void DestroyObstacle(Item itemDestroyed)
        {
            var targetObject = LevelManager.Instance.targetObjects.Where(i=>i.TargetBind.sprites.Any(x=>x==fishSprite));
            if(targetObject.Any() && !targetObject.First().IsDone())
            {
                audioSource.Play();
                LevelManager.Instance.DestroySpriteEvent(fishSprite, transform.position, this, null);
                Instantiate(fishEffect, transform.position + Vector3.up * 1, Quaternion.identity);
                animator.SetTrigger("fish");
            }
        }
    }
}