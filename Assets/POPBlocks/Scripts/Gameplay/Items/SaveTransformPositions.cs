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

namespace POPBlocks.Scripts.Items
{
    public class SaveTransformPositions : MonoBehaviour
    {
        readonly List<Vector3> childTransforms = new List<Vector3>();

        public void Awake()
        {
            foreach (Transform tr in transform)
            {
                childTransforms.Add(tr.localPosition);
            }
        }

        public void OnEnable()
        {
            // _animator?.Reset("init");
            int i = 0;
            foreach (Transform tr in transform)
            {
                tr.localPosition = childTransforms[i];
                i++;
            }
        }
    }
}