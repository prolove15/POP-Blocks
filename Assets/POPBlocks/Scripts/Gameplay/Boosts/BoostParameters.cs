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

namespace POPBlocks.Scripts.Boosts
{

    
    [Serializable]
    public class BoostParameters
    {
        [HideInInspector]
        public string name;
        public BoostType boostType;
        [Header("Cost of the boost")]
        public int cost = 10;
        [Header("Number of purchased boosters")]
        public int purchasingAmount = 10;
        [Header("Start number of boost for free")]
        public int startCount= 3;
        [Header("Number of boosters that will appear on the game field")]
        public int countItems = 3;
        [Header("The level at which the booster will be available")]
        public int openLevel = 1;
        public string description;
    }
}