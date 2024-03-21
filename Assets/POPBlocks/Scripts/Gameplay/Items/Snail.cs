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

using UnityEngine;

namespace POPBlocks.Scripts.Items
{
    public class Snail : Item
    {
        public Animator eyeAnimator;
        private int hp = 2;
        public AudioSource audioSource;
        public override void DestroyItemStart(bool noScore, float delay, bool noEffect, string effectName, bool destroyAround)
        {
            if (hp > 0)
            {
                if (latestMoveCounter >= LevelManager.Instance.moveCounter && latestMoveCounter > 0) return;
                latestMoveCounter = LevelManager.Instance.moveCounter;
                DestroyObstacle(this);
            }
            else
            {
                latestMoveCounter = 0;
                base.DestroyItemStart(noScore, delay, noEffect, effectName, destroyAround);
            }
        }

        protected override void DestroyObstacle(Item itemDestroyed)
        {
            hp--;
            if (hp <= 0)
                base.DestroyObstacle(itemDestroyed);
            else
            {
                audioSource.PlayDelayed(0f);
                eyeAnimator.SetTrigger("wake");
            }
            animator.SetTrigger("scale");

        }
    }
}