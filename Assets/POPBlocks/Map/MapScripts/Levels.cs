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

using System;
using POPBlocks.MapScripts;
using UnityEngine;

namespace POPBlocks.MapScripts
{
    public class Levels : MonoBehaviour
    {
        public static event EventHandler<LevelReachedEventArgs> LevelSelected;
        public static event EventHandler<LevelReachedEventArgs> LevelReached;
        
        protected static void LevelSelected_(LevelsMap _instance, int number)
        {
            if (LevelSelected != null) LevelSelected(_instance, new LevelReachedEventArgs(number));
        }
        
        protected static void LevelReached_(LevelsMap _instance, int number)
        {
            if (LevelSelected != null) LevelReached(_instance, new LevelReachedEventArgs(number));
        }
        
    }
}