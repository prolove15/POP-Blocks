// // ©2015 - 2021 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Coffee.UIExtensions
{
	/// <summary>
	/// Unmask Raycast Filter.
	/// The ray passes through the unmasked rectangle.
	/// </summary>
	[AddComponentMenu("UI/Unmask/UnmaskRaycastFilter", 2)]
	public class UnmaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
	{
		//################################
		// Serialize Members.
		//################################
		[Tooltip("Target unmask component. The ray passes through the unmasked rectangle.")]
		[SerializeField] Unmask m_TargetUnmask;


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Target unmask component. Ray through the unmasked rectangle.
		/// </summary>
		public Unmask targetUnmask{ get { return m_TargetUnmask; } set { m_TargetUnmask = value; } }

		/// <summary>
		/// Given a point and a camera is the raycast valid.
		/// </summary>
		/// <returns>Valid.</returns>
		/// <param name="sp">Screen position.</param>
		/// <param name="eventCamera">Raycast camera.</param>
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			// Skip if deactived.
			if (!isActiveAndEnabled || !m_TargetUnmask || !m_TargetUnmask.isActiveAndEnabled)
			{
				return true;
			}

            // check inside
            if (eventCamera)
            {
                return !RectTransformUtility.RectangleContainsScreenPoint((m_TargetUnmask.transform as RectTransform), sp, eventCamera);
            }
            else
            {
                return !RectTransformUtility.RectangleContainsScreenPoint((m_TargetUnmask.transform as RectTransform), sp);
            }
		}


		//################################
		// Private Members.
		//################################

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		void OnEnable()
		{
		}
	}
}