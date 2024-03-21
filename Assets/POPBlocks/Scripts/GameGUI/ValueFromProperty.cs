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
using System.Reflection;
using TMPro;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI
{
    public class ValueFromProperty : MonoBehaviour
    {
        public MonoBehaviour instanceObject;
        public string propertyPath;
        public string extraText;
        public bool update;
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = extraText + " " + GetDeepPropertyValue(instanceObject, propertyPath).ToString();

        }

        private void Update()
        {
            if(update)
                GetComponent<TextMeshProUGUI>().text = extraText + " " + GetDeepPropertyValue(instanceObject, propertyPath).ToString();
        }

        public object GetDeepPropertyValue(object instance, string path){
            var pp = path.Split('.');
            Type t = instance.GetType();
            foreach(var prop in pp){
                FieldInfo propInfo = t.GetField(prop);
                if(propInfo != null){
                    instance = propInfo.GetValue(instance);
                }else throw new ArgumentException("Properties path is not correct");
            }
            return instance;
        }
    }
}