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
using POPBlocks.Scripts.Items;
using UnityEngine;
using UnityEngine.Events;

namespace POPBlocks.Scripts.Effects
{
    public class CircleWave : MonoBehaviour
    {
        private float scaleFactor = 1.6f;
        private float scaleDuration = .6f;
        private float moveDistance = .5f;
        private float moveDuration = 0.2f;
        public UnityAction OnComplete;

        private CircleCollider2D circleCollider;
        private Rigidbody2D rb;


        private void Awake()
        {
            circleCollider = gameObject.GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        public void Start()
        {
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.one * scaleFactor, scaleDuration)
                .OnUpdate(() =>
                {
                    circleCollider.radius += Time.deltaTime * 10;
                }).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var item = other.GetComponent<Item>();
            if (item)
            {
                MoveItemRadiallyFromCenterAndBack(other.transform);
            }
        }

        private void MoveItemRadiallyFromCenterAndBack(Transform itemTransform)
        {
            Vector3 originalPosition = itemTransform.localPosition;
            Vector3 direction = (itemTransform.position - transform.position).normalized;
            Vector3 targetPosition = originalPosition + direction * moveDistance;

            // dotween sequence
            Sequence sequence = DOTween.Sequence();
            sequence.Append(itemTransform.DOLocalMove(targetPosition, moveDuration));
            sequence.Append(itemTransform.DOLocalMove(originalPosition, moveDuration*1.2f));

        }
    }
}