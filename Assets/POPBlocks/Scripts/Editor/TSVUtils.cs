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
using System.IO;
using POPBlocks.Scripts.Gameplay.LevelHandle;
using UnityEngine;

namespace POPBlocks.Scripts.Editor
{
    public static class TSVUtils
    {
        static List<string> header = new List<string>();

        public static List<TSVParseObject> readTextFile(string file_path)
        {
            if (file_path == null) return null;
            StreamReader inp_stm = new StreamReader(file_path);
            header = new List<string>();

            List<TSVParseObject> list = new List<TSVParseObject>();
            int counter = 0;
            string name = "";
            TSVParseObject obj = null;
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                if (inp_ln != null)
                {
                    string[] recs = inp_ln.Split(new string[] {"	"}, StringSplitOptions.RemoveEmptyEntries);
                    if (counter > 0)
                    {
                        name = recs[GetHeaderIndex("name")];
                        if (name == "win")
                        {
                            obj = new TSVParseObject();
                            obj.id = Int32.Parse(name);
                            obj.JsonLevelSettingsToInt(JsonUtility.FromJson<JsonLevelSettings>(recs[GetHeaderIndex("game_sim_settings")]));
                            obj.winChance = Int32.Parse(recs[GetHeaderIndex("avg")]);
                           
                            list.Add(obj);

                        }
                    }

                    foreach (string rec in recs)
                    {
                        if (counter == 0)
                            header.Add(rec);
                    }
                }

                counter++;
            }

            inp_stm.Close();
            return list;

        }

        static int GetHeaderIndex(string name)
        {
            return header.FindIndex(c => c == name);
        }
    }
}