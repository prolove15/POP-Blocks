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
using System.Collections;
using POPBlocks.Scripts.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace POPBlocks.Scripts.Effects
{
	/// <summary>
	/// Trail effect for win animations
	/// </summary>
	public class TrailEffect : MonoBehaviour
	{
		public Item target;
		public GameObject explosion;
		// Use this for initialization
		public void StartAnim(Action<Item> callback_)
		{
			StartCoroutine(StartAnimation(callback_));
		}

		private IEnumerator StartAnimation(Action<Item> callback)
		{
			var offset = 2.5f;
			var duration = .5f;
			var curveX = new AnimationCurve(new Keyframe(0, transform.localPosition.x), new Keyframe(duration, target.transform.position.x));
			var curveY = new AnimationCurve(new Keyframe(0, transform.localPosition.y), new Keyframe(duration, target.transform.position.y));
			curveX.AddKey(duration * .5f, curveX.Evaluate(duration * .5f) + Random.Range(-offset, offset));
			curveY.AddKey(duration * .5f, curveY.Evaluate(duration * .5f) + Random.Range(-offset, offset));

			//curveX.AddKey(duration * .75f, curveX.Evaluate(duration * .75f) + UnityEngine.Random.Range(-offset, offset));
			//curveY.AddKey(duration * .75f, curveY.Evaluate(duration * .75f) + UnityEngine.Random.Range(-offset, offset));

			var startTime = Time.time;
			float distCovered = 0;
			while (distCovered < duration)
			{
				distCovered = (Time.time - startTime);
				transform.localPosition = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
				yield return new WaitForFixedUpdate();
			}

			var expl = Instantiate(explosion);
			expl.transform.position = transform.position;
			callback(target);
			yield return new WaitForSeconds(5);
			Destroy(gameObject);
		}
	}
}
