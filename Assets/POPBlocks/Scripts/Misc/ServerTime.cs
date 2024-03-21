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
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace POPBlocks.Scripts
{
    public class ServerTime : UnityEngine.MonoBehaviour
    {
        public static ServerTime THIS;
        public DateTime serverTime;
        public bool dateReceived;
        public delegate void DateReceived();
        public static event DateReceived OnDateReceived;
        [Header("Test date example: 2019-08-27 09:12:29")]
        public string TestDate; 
        private void Awake()
        {
            if (THIS == null)
                THIS = this;
            else if(THIS != this)
                Destroy(gameObject);
            GetServerTime();
        }

        private void OnEnable()
        {
            GetServerTime();
        }

        void GetServerTime ()
        {
            StartCoroutine(getTime());
        }

        void Update()
        {
            serverTime = serverTime.AddSeconds(Time.deltaTime);
        }

        IEnumerator getTime()
        {
#if UNITY_WEBGL
            serverTime = DateTime.Now;
#else
            UnityWebRequest www = UnityWebRequest.Get("https://candy-smith.info/gettime.php");
            yield return www.SendWebRequest();
            if(www.downloadHandler.text != "")
                serverTime = DateTime.Parse(www.downloadHandler.text);
            else
                serverTime = DateTime.Now;
            if(TestDate!="" && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
                serverTime = DateTime.Parse(TestDate);
#endif
            yield return  null;
            dateReceived = true;
            OnDateReceived?.Invoke();
        }
    }
}