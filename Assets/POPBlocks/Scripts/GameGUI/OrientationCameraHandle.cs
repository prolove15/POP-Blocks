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
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    /// <summary>
    /// Changes camera size depending from orientation and aspect ratio
    /// </summary>
    [ExecuteInEditMode]
    public class OrientationCameraHandle : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform objectTransform;
//    public List<OrientationRatio> list = new List<OrientationRatio>();
        public Vector2 currentAspectRatio;
        public Rect currentCameraRect;
        void OnEnable()
        {
            OrientationListener.OnOrientationChanged += OnOrientationChanged;
        }

        void OnDisable()
        {
            OrientationListener.OnOrientationChanged -= OnOrientationChanged;
        }
        void OnOrientationChanged(ScreenOrientation orientation)
        {
            currentAspectRatio = GetAspectRatio(Screen.width, Screen.height);

//        var orientationRatio = list.Where(i => i.ratio.x == currentAspectRatio.x && i.ratio.y == currentAspectRatio.y).FirstOrDefault();
//        var size = orientationRatio?.cameraSize ?? 0;
            if (mainCamera != null)
            {
                currentCameraRect = mainCamera.rect;
                mainCamera.orthographicSize = 5.3f;
                mainCamera.orthographicSize =15.3f / Screen.width * Screen.height / 2f;
//            if (size > 0) mainCamera.orthographicSize = size;
            }
//        if (objectTransform != null && orientationRatio != null) objectTransform.position = orientationRatio.cameraPosition;
        }
        public Vector2 GetAspectRatio(int x, int y)
        {
            var f = x / (float)y;
            var i = 0;
            while (true)
            {
                i++;
                if (Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                    break;
            }
            return new Vector2((float)Math.Round(f * i, 2), i);
        }
        [Serializable]
        public class OrientationRatio
        {
            public Vector2 ratio;
            public float cameraSize;
            public Vector2 cameraPosition;

        }
    }
}



