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
using System.Collections.Generic;
using System.Linq;

namespace POPBlocks.Scripts.Utils
{
    public static class LinqUtils
    {
        public static bool AllNull<T>(this IEnumerable<T> seq)
        {
            var result = seq.All(i => i == null || i.Equals(null) || !seq.Any());
            return result;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> seq)
        {
            var result = seq.Where(i => i != null && !i.Equals(null));
            return result;
        }

        public static T TryGetElement<T>(this IEnumerable<T> seq, int index, T ifnull=default(T))
        {
            if (index < seq.Count())
            {
                var element = seq.ToArray()[index];
                return element;
            }
            return ifnull;
        }

        public static bool InsideBounds<T>(this IEnumerable<T> seq, int index) => index < seq.Count() && index >= 0;

        public static IEnumerable<T> ForEachY<T>(this IEnumerable<T> seq, Action<T> action)
        {
            seq.ToList().ForEach(action);
            var result = seq;
            return result;
        }
    
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy<T, int>((item) => rnd.Next());
        }

        public static T ElementAtOrDefault<T>(this IList<T> list, int index, T @default)
        {
            return index >= 0 && index < list.Count ? list[index] : @default;
        }
    
        public static T Addd<T>(this List<T> list, T newItem)
        {
            list.Add(newItem);
            return newItem;
        }
    
        public static T NextRandom<T>(this IEnumerable<T> source)
        {
            Random gen = new Random((int)DateTime.Now.Ticks);
            return source.Skip(gen.Next(0, source.Count() - 1) - 1).Take(1).FirstOrDefault();
        }

        public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> source)
        {
            List<T> Remaining = new List<T>(source);
            while (Remaining.Count >= 1)
            {
                T temp = NextRandom(Remaining);
                Remaining.Remove(temp);
                yield return temp;
            }
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}