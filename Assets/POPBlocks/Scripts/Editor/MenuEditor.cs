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

using POPBlocks.MapScripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    public class MenuEditor
    {
        [MenuItem("POP Blocks/Scenes/Main scene _1")]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/main.unity");
        }

        [MenuItem("POP Blocks/Scenes/Game scene _2")]
        public static void GameScene()
        {
            EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/game.unity");
        }

        [MenuItem("POP Blocks/Scenes/Map scene _3")]
        public static void MapScene()
        {
            EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/map.unity");
        }

        [MenuItem("POP Blocks/Scenes/Grid Levels scene _4")]
        public static void GridLevels()
        {
            EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/grid.unity");
        }

        [MenuItem("POP Blocks/Edit latest level %g")]
        public static void EditLatestLevel()
        {
            Selection.activeObject =
                AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Levels/Level_" + PlayerPrefs.GetInt("OpenLastLevelTest", 1).ToString() + ".asset");
        }

        [MenuItem("POP Blocks/Level editor")]
        public static void LevelEditor()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Levels/Level_1.asset");
        }

        [MenuItem("POP Blocks/Settings/Ads settings")]
        public static void AdsSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/AdsSettings.asset");
        }

        // [MenuItem("POP Blocks/Settings/Pool settings")]
        // public static void PoolSettings()
        // {
        //     Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/PoolSettings.asset");
        // }   
        [MenuItem("POP Blocks/Settings/Tutorials settings")]
        public static void TutorialSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/TutorialSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Debug and shortcuts")]
        public static void DebugSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/DebugSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Game settings")]
        public static void GameSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/GameSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Bonus matches settings")]
        public static void BonusMatchSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/BonusMatchesSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Boosters settings")]
        public static void BoostSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/BoostSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Shop settings")]
        public static void ShopSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Settings/ShopSettings.asset");
        }

        [MenuItem("POP Blocks/Settings/Targets settings")]
        public static void TargetSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Targets/BallColored.asset");
        }
        
        [MenuItem("POP Blocks/Settings/Open all levels", false, 10)]
        public static void OpenAllLevels()
        {
            var _mapProgressManager = new PlayerPrefsMapProgressManager();
            _mapProgressManager.OpenAllLevels();
        }

        [MenuItem("POP Blocks/Settings/Clear player prefs", false, 10)]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("POP Blocks/Documentation/ADS/Ads setup")]
        public static void AdsDoc()
        {
            Application.OpenURL("https://www.candy-smith.com/pop-blocks-ads-settings");
        }
        [MenuItem("POP Blocks/Documentation/ADS/Unity ads")]
        public static void UnityadsDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit");
        }

        [MenuItem("POP Blocks/Documentation/ADS/Google mobile ads(admob)")]
        public static void AdmobDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit");
        }

        [MenuItem("POP Blocks/Documentation/Unity IAP (in-apps)")]
        public static void Inapp()
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");
        }

        [MenuItem("POP Blocks/Documentation/Leadboard/Facebook (step 1)")]
        public static void FBDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
        }

        [MenuItem("POP Blocks/Documentation/Leadboard/Server setup (step 2)")]
        public static void GSDoc()
        {
            Application.OpenURL("https://docs.google.com/document/d/1WUFLHA4ZadxmluIyWlSLZToSMPvDs4vaE6Ag4newTBg/edit");
        }

        [MenuItem("POP Blocks/Documentation/PDS map link")]
        public static void PSDLink()
        {
            Application.OpenURL("https://www.dropbox.com/s/bmssuqtx7xaadvv/MAP_POPblocks.psd?dl=0");
        }
    }
}