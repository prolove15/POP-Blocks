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

using POPBlocks.Timeline.Playable.TransformMove;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    public class StarAnimation : MonoBehaviour, ITransformMove
    {
        private AnimationCurve curveX;
        private AnimationCurve curveY;
        public Vector2 targetPos;
        public void OnPrecessFrame(float normalisedTime)
        {
            transform.position = new Vector2(curveX.Evaluate(normalisedTime), curveY.Evaluate(normalisedTime));
        }

        public void OnStartClip()
        {
            curveX = new AnimationCurve(new Keyframe(0, transform.position.x), new Keyframe(1, targetPos.x));
            curveY = new AnimationCurve(new Keyframe(0, transform.position.y), new Keyframe(1, targetPos.y));
        }
    }
}
