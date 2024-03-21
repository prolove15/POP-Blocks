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
using System.Linq;
using UnityEngine;

namespace POPBlocks.Scripts
{
    /// <summary>
    /// Color generator, checks available colors
    /// </summary>
    public static class ColorGenerator
    {
        public static int GenColor(int maxColors, int[] exceptColors, bool onlyNONEType = false)
        {
            List<int> remainColors = new List<int>();

            int randColor = 0;
            do
            {
                randColor = Random.Range(0, maxColors);
            
            } while (exceptColors.Contains(randColor) && exceptColors.Count() < maxColors-1 );
            if (remainColors.Count > 0)
                randColor = remainColors[Random.Range(0, remainColors.Count)];
            if (exceptColors.Contains( randColor))
                randColor = (int) Mathf.Repeat( randColor++, maxColors);
            return randColor;
        }
    }
}