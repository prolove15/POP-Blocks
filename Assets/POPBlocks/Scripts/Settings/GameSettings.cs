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

using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace POPBlocks.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        [Header("Default settings")] 
        public int CapOfLife;
        public int coins;
        [Header("Life settings")] 
        public int refillLifeCost;
        public int TotalTimeForRestLifeHours;
        public int TotalTimeForRestLifeMin;
        public int TotalTimeForRestLifeSec;
        [Header("Failed settings")] public int failAttempts = 1;
        [Header("Pay to continue game after fail")]
        public int continuePrice; 
        [Header("Add Moves to continue game after fail")]
        public int movesContinue;
        [Header("Go map after win")]
        public bool goMap;
        public int afterLevel = 1;
        public bool openMenuPlay = false;

        [HideInInspector]public int coinsReward = 10;
        [Header("Level selection")] public MapTypes mapType;
        
        public AnimationCurve fallingCurve = AnimationCurve.Linear(0, 0, 1, 0);

        private void OnValidate()
        {
#if UNITY_EDITOR
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes != null && scenes.Any())
            {
                scenes.First(i => i.path.Contains("grid")).enabled = mapType == MapTypes.GridLevels;
                scenes.First(i => i.path.Contains("map")).enabled = mapType == MapTypes.ScrollingsMap;
                EditorBuildSettings.scenes = scenes;
            }

#endif
        }

        public T GetAttribute<T> ( string _name ) {
            return (T)typeof(GameSettings).GetField( _name ).GetValue (this);
        } 
    }

    public enum MapTypes
    {
        NoMap,
        ScrollingsMap,
        GridLevels
    }
}