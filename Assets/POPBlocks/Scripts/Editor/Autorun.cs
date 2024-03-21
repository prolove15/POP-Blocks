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

using System.IO;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            EditorApplication.update += InitProject;

        }

        static void InitProject()
        {
            EditorApplication.update -= InitProject;
            if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool(Application.dataPath+"AlreadyOpened"))
            {
                if (EditorSceneManager.GetActiveScene().name != "game" && Directory.Exists("Assets/POPBlocks/Scenes"))
                {
                    EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/main.unity");
                    SetAspectRatio();
                }
                EditorPrefs.SetBool(Application.dataPath+"AlreadyOpened", true);
            }
        }

        private static void SetAspectRatio()
        {
            var groupType = GameViewSizeHelper.GetCurrentGameViewSizeGroupType();
            var gameViewSize = new GameViewSizeHelper.GameViewSize();
            gameViewSize.type = GameViewSizeHelper.GameViewSizeType.AspectRatio;
            gameViewSize.width = 3;
            gameViewSize.height = 4;
            gameViewSize.baseText = "3:4 Aspect";

            if (!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
            }

            GameViewSizeHelper.ChangeGameViewSize(groupType, gameViewSize);

            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                    var assembly = typeof(UnityEditor.Editor).Assembly;
                    var type = assembly.GetType("UnityEditor.GameView");
                    var gameView = EditorWindow.GetWindow(type, false, "Game", false);
                    var minScaleProperty = type.GetProperty("minScale", flag);
                    float minScale = (float)minScaleProperty.GetValue(gameView, null);
                    type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { minScale });
                    gameView.Repaint();
                };
            };
        }
    }
}