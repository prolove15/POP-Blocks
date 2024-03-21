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
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace POPBlocks.Scripts.Items.Editor
{
    [CustomEditor(typeof(IconEditor))]
    [CanEditMultipleObjects]
    public class IconEditorE : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Update icon"))
            {
                var scene = SceneManager.GetActiveScene().name;
                EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/for_icons.unity");
                UpdateIcon();

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/" + scene + ".unity");
            }
        }

        private void UpdateIcon()
        {
            var iconEditors = targets;
            foreach (var editor_ in iconEditors)
            {
                var editor = (IconEditor) editor_;
                var file = DoSnapshot(editor.gameObject, null, Camera.main);
                editor.icon = new Texture2D[file.Length];
                for (int i = 0; i < file.Length; i++)
                {
                    editor.icon[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(file[i]);
                }

                EditorUtility.SetDirty(editor);
            }
        }

        private static string[] DoSnapshot(GameObject go, Canvas canvas, Camera cam)
        {
            var ins = GameObject.Instantiate(go.transform.GetChild(0).gameObject, null, false);
            ins.transform.position = Vector3.zero;
            ins.SetActive(true);
            var colorCOmponent = GetChildrenRecursevly(ins.transform, new List<ColorComponent>());
            var sr = ins.GetComponent<ColorComponent>();
            if(sr) colorCOmponent.Add(sr);
            var spritesCount = colorCOmponent != null && colorCOmponent.Count > 0 ? colorCOmponent[0].Sprites.Count : 1;
            string[] astPath = new string[spritesCount];
            for (int i = 0; i < spritesCount; i++)
            {
                colorCOmponent.ForEach(x => x.SetColor(i));
                string fileName = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(go)) + i + ".png";
                astPath[i] = "Assets/POPBlocks/Prefabs/snapshots/" + fileName;
                fileName = Application.dataPath + "/POPBlocks/Prefabs/snapshots/" + fileName;
                FileInfo info = new FileInfo(fileName);
                if (info.Exists)
                    File.Delete(fileName);
                else if (!info.Directory.Exists)
                    info.Directory.Create();

                var renderTarget = RenderTexture.GetTemporary(200, 200);
                cam.aspect = 350f / 350f;
                cam.orthographic = true;
                cam.targetTexture = renderTarget;
                cam.Render();

                RenderTexture.active = renderTarget;
                Texture2D tex = new Texture2D(renderTarget.width, renderTarget.height);
                tex.ReadPixels(new Rect(0, 0, renderTarget.width, renderTarget.height), 0, 0);
                File.WriteAllBytes(fileName, tex.EncodeToPNG());
                var loadMainAssetAtPath = (Texture2D) AssetDatabase.LoadAssetAtPath<Texture>(astPath[i]);
                SetTextureImporterFormat(loadMainAssetAtPath, true);
            }

            cam.targetTexture = null;
            Object.DestroyImmediate(ins);

            return astPath;
        }
        
        public static void SetTextureImporterFormat( Texture2D texture, bool isReadable)
        {
            if ( null == texture ) return;

            string assetPath = AssetDatabase.GetAssetPath( texture );
            var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
            if ( tImporter != null )
            {
                tImporter.textureType = TextureImporterType.Default;

                tImporter.isReadable = isReadable;

                AssetDatabase.ImportAsset( assetPath );
                AssetDatabase.Refresh();
            }
        }

        static List<ColorComponent> GetChildrenRecursevly(Transform item, List<ColorComponent> list)
        {
            foreach (Transform tr in item.transform)
            {
                var sr = tr.GetComponent<ColorComponent>();
                if (sr != null)
                    list.Add(sr);
                GetChildrenRecursevly(tr, list);
            }

            return list;
        }
    }
}