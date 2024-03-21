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
using POPBlocks.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    public class PostImporting : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            CheckDefines("Assets/GoogleMobileAds", "ADMOB");
            CheckDefines("Assets/Chartboost", "CHARTBOOST_ADS");
            CheckDefines("Assets/FacebookSDK", "FACEBOOK");
            CheckDefines("Assets/PlayFabSDK", "PLAYFAB");
            CheckDefines("Assets/GameSparks", "GAMESPARKS");
            CheckDefines("Assets/Appodeal", "APPODEAL");
            CheckDefines("Assets/GetSocial", "USE_GETSOCIAL_UI");
            CheckIronsourceFolder();
            CheckDefines("Assets/IronSource", "IRONSOURCE");
        }

        static void CheckDefines(string path, string symbols)
        {
            if (Directory.Exists(path))
            {
                DefineSymbolsUtils.AddSymbol(symbols);
            }
            else
            {
                DefineSymbolsUtils.DeleteSymbol(symbols);
            }
        }

        public static void CheckIronsourceFolder()
        {
            var str = "Assets/IronSource/Scripts";
            if (Directory.Exists(str))
            {
                string asmdefPath = Path.Combine(str, "IronsourceAssembly.asmdef");
                if (!File.Exists(asmdefPath))
                {
                    CreateAsmdef(asmdefPath);
                }
            }
        }

        private static void CreateAsmdef(string path)
        {
            var assemblyDefinition = new AssemblyDefinition
            {
                name = "IronsourceAssembly", 
                references = new string[0],
                includePlatforms = new string[0], 
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true,
                defineConstraints = new string[0],
                versionDefines = new VersionDefine[]
                {
                    new()
                    {
                        name = "com.unity.services.levelplay",
                        define = "IRONSOURCE"
                    }
                }
            };

            File.WriteAllText(path, JsonUtility.ToJson(assemblyDefinition, true));
            AssetDatabase.Refresh();
        }
    }

    [System.Serializable]
    public class AssemblyDefinition
    {
        public string name;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
    }

    [System.Serializable]
    public class VersionDefine
    {
        public string name;
        public string expression;
        public string define;
    }
}