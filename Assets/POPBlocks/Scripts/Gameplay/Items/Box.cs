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

namespace POPBlocks.Scripts.Items
{
    class Box : Item
    {
        public override void DestroyItemStart(bool noScore, float delay = 0, bool noEffect = false, string effectName = "", bool destroyAround = false)
        {
            animator.SetTrigger("fish");
            base.DestroyItemStart(noScore, delay, noEffect, effectName, destroyAround);
        }
        // protected override void DestroyObstacle(Item itemDestroyed)
        // {
        //     animator.SetTrigger("fish");
        //     // base.DestroyObstacle(itemDestroyed);
        // }
    }
}