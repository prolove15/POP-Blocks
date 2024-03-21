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

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    public class DeleteUnnecessaryResources : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        string[] paths = new string[]
        {
            Application.dataPath + "/POPBlocks/Prefabs/snapshots"
        };

        public int callbackOrder => default;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!report.summary.options.HasFlag(BuildOptions.Development))
                for (int i = 0; i < paths.Length; i++)
                    AssetDatabase.MoveAsset(paths[i], paths[i] + "~");
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!report.summary.options.HasFlag(BuildOptions.Development))
                for (int i = 0; i < paths.Length; i++)
                    AssetDatabase.MoveAsset(paths[i] + "~", paths[i]);
            AssetDatabase.Refresh();
        }
    }
}