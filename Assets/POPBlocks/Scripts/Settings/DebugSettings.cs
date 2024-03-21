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

using UnityEngine;

namespace POPBlocks.Scripts
{
    public class DebugSettings : ScriptableObject
    {
        [Range(0, 100)] public float TimeScaleUI = 1;
        [Range(0, 100)] public float TimeScaleItems = 1;
        [Header("Auto test AI")] public bool AI;
        [Space(10)]
        
        [Header("Editor shortcuts")] public bool enableShortcuts = true;
        [Tooltip("press to win")] public KeyCode Win;
        [Tooltip("set moves to 1")] public KeyCode Lose;
        [Tooltip("restart the level")] public KeyCode Restart;
        [Tooltip("android's back button")] public KeyCode Back;
        [Tooltip("regenerate items")] public KeyCode Regen;
        [Tooltip("set moves to 1")] public KeyCode lastMove;
        
    }
}