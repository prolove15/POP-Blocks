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
using UnityEngine;

//[ExecuteInEditMode]
namespace POPBlocks.Scripts.Effects
{
	/// <summary>
	/// Lightning effect after passing a level
	/// </summary>
	public class Lightning : MonoBehaviour
	{
		Vector2[] posList = new Vector2[10];
		LineRenderer line;
		public float amplitude = 5;
		public Vector2 pos1, pos2;
		public bool perlin = true;
		// Use this for initialization
		void StartLight()
		{
			line = GetComponent<LineRenderer>();
			StartCoroutine(StartLightning());

			Invoke("Kill", 0.5f);
		}

		// Update is called once per frame
		IEnumerator StartLightning()
		{
			while (true)
			{
				posList = new Vector2[10];
				line.positionCount = posList.Length;

				for (var i = 0; i < posList.Length; i++)
				{
					posList[i] = Vector2.Lerp(pos1, pos2, (float)i / line.positionCount);
					line.SetPosition(i, posList[i]);
				}
				if (perlin)
				{
					for (var i = 0; i < posList.Length; i++)
					{
						posList[i] = new Vector2(posList[i].x, posList[i].y + Mathf.PerlinNoise(Random.value, Random.value) * Random.Range(-1f, 1f));
						line.SetPosition(i, posList[i]);
					}
				}
				posList[0] = pos1;
				posList[posList.Length - 1] = pos2;
				yield return new WaitForEndOfFrame();
			}
		}

		void Kill()
		{
			Destroy(gameObject);
		}

		internal void SetLight(Vector3 position1, Vector3 position2)
		{
			pos1 = position1;
			pos2 = position2;

			StartLight();
		}
	}
}
