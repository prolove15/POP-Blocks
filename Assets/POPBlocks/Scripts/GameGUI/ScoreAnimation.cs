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

using DG.Tweening;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    public class ScoreAnimation : MonoBehaviour
    {
        private TextMeshProUGUI textmesh;
        public float time = 1;
        public Vector3 animTarget = Vector2.up;
        private void Awake()
        {
            textmesh = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            textmesh.color = Color.white;
            transform.DOMove(transform.position + animTarget, time).OnComplete(()=>gameObject.SetActive(false));
            textmesh.DOColor(new Color(1, 1, 1, 0), time);
        }
    }
}