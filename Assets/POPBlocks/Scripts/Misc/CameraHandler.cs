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

using System.Collections;
using DG.Tweening;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace POPBlocks.Scripts
{
    public class CameraHandler : MonoBehaviour
    {
        public Field field;
        [SerializeField] private RectTransform rect;
        public Transform pivot;
        private Vector3 savepos;
        private bool shake;

        private void Start()
        {
            transform.position = new Vector3(-1000, transform.position.y, -10);
            StartCoroutine(FindSize());
        }

        IEnumerator FindSize()
        {
            var camera = GetComponent<Camera>();
            yield return new WaitWhile(() => field.worldRect == Rect.zero);
            var fieldRect = field.worldRect;
            while (true)
            {
                var centerRect = GetWorldRect(rect, rect.lossyScale);
                if (fieldRect.width < centerRect.width && fieldRect.height < centerRect.height)
                    camera.orthographicSize -= Time.deltaTime * 10;
                else
                    camera.orthographicSize += Time.deltaTime * 10;
                if ((Mathf.Abs(fieldRect.width - centerRect.width) < 0.1f && fieldRect.height < centerRect.height) || (Mathf.Abs(fieldRect.height - centerRect.height) < 0.1f &&
                                                                                                                       fieldRect.width < centerRect.width))
                     break;
                yield return new FixedUpdate();
            }
            var rectPosition = (Vector3) ((Vector2) rect.position - field.GetCenter());
            var targetPos = camera.transform.position - rectPosition;
            transform.DOMove(targetPos, 3).OnComplete(BeginGame);
        }

        private void BeginGame()
        {
            savepos = transform.position;
            LevelManager.Instance.GameState = GameState.PrepareTutorial;
        }

        public void Shake()
        {
            if (shake) return;
            shake = true;
            transform.DOShakePosition(0.2f, Vector3.one, 10, 180).OnComplete(() =>
            {
                transform.position = savepos;
                shake = false;
            });
        }

        private Rect GetWorldRect(RectTransform rt, Vector2 scale)
        {
            // Convert the rectangle to world corners and grab the top left
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Rescale the size appropriately based on the current Canvas scale
            Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

            return new Rect(topLeft, scaledSize);
        }
    }
}