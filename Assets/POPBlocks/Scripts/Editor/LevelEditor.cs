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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.LevelHandle;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace POPBlocks.Scripts.Editor
{
    [CustomEditor(typeof(Level))]
    public class LevelEditor : UnityEditor.Editor
    {
        private Level level;
        private LevelBlock brush;
        private List<LevelBlock> prefabs;
        private int squareSize = 40;
        private int color = 0;
        private List<LevelBlock> prefabsSimpleItems;
        private int num;
        private bool foldRandomize;
        private float squareRandomize;
        private GUISkin bntSkin;

        public override VisualElement CreateInspectorGUI()
        {
            InitPalette();

            level = (Level) target;
            if (level.Blocks.All(i => i.prefab == null))
            {
                InitBlocks(level.size.x * level.size.y);
            }

            bntSkin = (GUISkin) Resources.Load("buttonAlign");
            if(!Application.isPlaying)
            {
                num = GetLevelNum();
                // PlayerPrefs.SetInt("OpenLevelTest", num);
                PlayerPrefs.SetInt("OpenLastLevelTest", num);
                PlayerPrefs.Save();
            }
            return base.CreateInspectorGUI();
        }

        private void InitPalette()
        {
            var item = Resources.Load<GameObject>("Blocks/Item");
            int colors = item.GetComponentInChildren<ColorComponent>().Sprites.Count;
            prefabsSimpleItems = new List<LevelBlock>();
            for (int col = 0; col < colors; col++)
            {
                prefabsSimpleItems.Add(GetTextures(item, col));
            }

            var list = Resources.LoadAll<IconEditor>("Blocks").OrderBy(i => i.order).ToArray();
            prefabs = new List<LevelBlock>();
            AddPrefabsToPalette(list);
            prefabs.Add(new LevelBlock(BlocksTypes.None, null, null, Vector2Int.one));

        }

        private void AddPrefabsToPalette(IconEditor[] list)
        {
            foreach (var item in list)
            {
                if (item.name == "Item") continue;
                prefabs.Add(GetTextures(item.gameObject, color));
            }
        }

        private LevelBlock GetTextures(GameObject item, int _color)
        {
            var iconEditor = item.GetComponent<IconEditor>();
            var component = item.GetComponent<Item>();
            var componentSize = component?.size ?? Vector2Int.one;
            return new LevelBlock(iconEditor.type, item, iconEditor.icon.Length > _color ? iconEditor.icon[_color] : iconEditor.icon[0], componentSize, _color);
        }

        public override void OnInspectorGUI()
        {
            LevelSwitcher();
            DrawDefaultInspector();
            GUIBlocks();
            GUIField();
            EditorGUILayout.Space(10);
            GUIBottom();
            GUISimulationResult();
        }

        private void GUISimulationResult()
        {
            UnityEditor.EditorGUILayout.Space(10);
            if (GUILayout.Button("Select Log file"))
            {
                var path = EditorUtility.OpenFilePanel("Select raw result", "", "TSV");
                level.simulationAggregate = TSVUtils.readTextFile(path);
                Save();
            }
            
            if (GUILayout.Button("Check simulation"))
            {
                UnityWebRequest www = UnityWebRequest.Get("http://79.143.29.70:9000/" + level.moves + "," + level.colors + "," + level.num + ",aggregate_results (12).tsv");
                UnityWebRequestAsyncOperation asyncOperation = www.SendWebRequest();
                asyncOperation.completed += (result) =>
                {
                    if (www.downloadHandler.text != "")
                        chance = int.Parse(www.downloadHandler.text);
                };
            }

            if (chance != -1)
                GUILayout.TextArea("Chance to win: " + chance);


            // if(level.simulationAggregate.Any())
            // {
            //     if (GUILayout.Button("Show simulation results"))
            //     {
            //         CurveWIndow window = (CurveWIndow) EditorWindow.GetWindow(typeof(CurveWIndow));
            //         window.PlotCurve(level.simulationAggregate);
            //         window.Show();
            //     }
            // }
        }

        private int chance=-1;

        IEnumerator GetChance()
        {
            UnityWebRequest www = UnityWebRequest.Get("http://79.143.29.70:9000/" + level.moves +"," + level.colors + "," + level.num + ",aggregate_results (12).tsv");
            yield return www.SendWebRequest();
            if(www.downloadHandler.text != "")
                chance = int.Parse(www.downloadHandler.text);
        }
        
        

        private void GUIBottom()
        {
            if (GUILayout.Button("Clear", GUILayout.Width(squareSize * 2)))
            {
                var size = level.size.x * level.size.y;
                InitBlocks(size);
            }

            EditorGUILayout.Separator();
            foldRandomize = EditorGUILayout.BeginFoldoutHeaderGroup(foldRandomize, "Randomize");
            if (foldRandomize)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    squareRandomize = EditorGUILayout.Slider("% of empty squares", squareRandomize, 0, 100);
                    if (GUILayout.Button("Randomize squares", GUILayout.Width(squareSize * 3)))
                    {
                        var size = level.size.x * level.size.y;
                        InitBlocks(size);
                        var emptySquare = prefabs.Find(x => x.type == BlocksTypes.Square);
                        for (int i = 0; i < level.Blocks.Length; i++)
                        {
                            level.Blocks[i] = new LevelBlock(Random.Range(0, 100) > squareRandomize ? BlocksTypes.Square : BlocksTypes.None, emptySquare.prefab, null, Vector2Int
                                .one);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void LevelSwitcher()
        {
            if (GUILayout.Button("Play level " + num))
            {
                PlayLevel();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Level");
                GUILayout.Space(50);
                if (GUILayout.Button(new GUIContent("<<", "Prev level")))
                {
                    OpenPrevLevel();
                }

                num = level.num;
                if(!Application.isPlaying)
                {
                    // PlayerPrefs.SetInt("OpenLevelTest", num);
                    // PlayerPrefs.Save();
                }
                EditorGUI.BeginChangeCheck();
                num = EditorGUILayout.DelayedIntField(num, GUILayout.Width(50));
                if (EditorGUI.EndChangeCheck())
                {
                    LoadLevel(num);
                }

                if (GUILayout.Button(new GUIContent(">>", "Next level")))
                {
                    OpenNextLevel();
                }

                if (GUILayout.Button("+"))
                {
                    NewLevel();
                }
            }
            GUILayout.EndHorizontal();


            void OpenPrevLevel()
            {
                if (num > 1)
                {
                    LoadLevel(num - 1);
                }
            }

            void OpenNextLevel()
            {
                if (num < Resources.LoadAll<Level>("Levels").Length)
                {
                    LoadLevel(num + 1);
                }
            }
        }

        private void PlayLevel()
        {
            PlayerPrefs.SetInt("OpenLevelTest", num);
            PlayerPrefs.Save();
            EditorSceneManager.OpenScene("Assets/POPBlocks/Scenes/game.unity");
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            else
                EditorApplication.isPlaying = true;
        }

        private void NewLevel()
        {
            var levelsNum = GetLevels().Length + 1;
            var instance = ScriptableObject.CreateInstance<Level>();
            instance.name = "Level_" + levelsNum;
            // instance.targets = level.targets;
            instance.num = levelsNum;
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/POPBlocks/Resources/Levels/Level_" + levelsNum + ".asset");
            AssetDatabase.CreateAsset(instance, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = instance;
        }

        private void LoadLevel(int n)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/POPBlocks/Resources/Levels/Level_" + (n) + ".asset");
            if(!Application.isPlaying)
            {
                // PlayerPrefs.SetInt("OpenLevelTest", n);
                PlayerPrefs.Save();
            }
        }

        private int GetLevelNum()
        {
            return GetLevels().OrderBy(i => i.num).ToList().FindIndex((x) => level == x) + 1;
        }

        private Level[] GetLevels()
        {
            return Resources.LoadAll<Level>("Levels");
        }


        void GUIBlocks()
        {
            int columns = 7;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            int c=0;
            {
                if (brush != null && brush.type == BlocksTypes.None)
                    UnityEngine.GUI.backgroundColor = Color.gray;
                else
                    UnityEngine.GUI.backgroundColor = Color.white;
            
                UnityEngine.GUI.backgroundColor = Color.white;
                int i;
                for (i = 0; i < prefabsSimpleItems.Count; i++)
                {
                    var prefab = prefabsSimpleItems[i];
                    if (brush != null && (brush.prefab == prefab.prefab && brush.color == prefab.color))
                        UnityEngine.GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button(new GUIContent("", prefab.prefab.name), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                    {
                        color = i;
                        brush = prefab;
                        brush.color = color;
                        brush.size = prefab.prefab.GetComponent<Item>().size;
                        InitPalette();
                    }
            
                    var lastRect = GUILayoutUtility.GetLastRect();
                    UnityEngine.GUI.DrawTexture(lastRect, prefab.icon);
                    UnityEngine.GUI.backgroundColor = Color.white;
                }

                for (c = 0; c < columns - i; c++)
                {
                    var prefab = prefabs[c];
                    if (brush != null && brush.prefab == prefab.prefab)
                        UnityEngine.GUI.backgroundColor = Color.gray;
                    string contextName = "x";
                    if (prefab.prefab != null)
                    {
                        contextName = "";
                    }

                    if (GUILayout.Button(new GUIContent(contextName, prefab.prefab?.name), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                    {
                        brush = prefab;
                    }

                    var lastRect = GUILayoutUtility.GetLastRect();
                    UnityEngine.GUI.DrawTexture(lastRect, prefab.icon);
                    UnityEngine.GUI.backgroundColor = Color.white;
                }
            }
            GUILayout.EndHorizontal();
            if (prefabs == null) return;
            int length = prefabs.Count();
            GUILayout.BeginVertical();
            {
                for (int i = c; i < length; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (i + j >= length) break;
                            var prefab = prefabs[i + j];
                            if (brush != null && brush.prefab == prefab.prefab)
                                UnityEngine.GUI.backgroundColor = Color.gray;
                            string contextName = "x";
                            if (prefab.prefab != null)
                            {
                                contextName = "";
                            }

                            if (GUILayout.Button(new GUIContent(contextName, prefab.prefab?.name), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                            {
                                brush = prefab;
                            }

                            var lastRect = GUILayoutUtility.GetLastRect();
                            if (prefab.icon != null) UnityEngine.GUI.DrawTexture(lastRect, prefab.icon);
                            UnityEngine.GUI.backgroundColor = Color.white;
                        }

                        i += columns - 1;
                    }

                    GUILayout.EndHorizontal();
                }

            }

            GUILayout.EndVertical();
        }

        private void GUIField()
        {
            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            var sizeX = level.size.x;
            var sizeY = level.size.y;
            var sX = level.Blocks.Where(i => i.size.x > 1).Sum(i => i.size.x);
            var sY = level.Blocks.Where(i => i.size.y > 1).Sum(i => i.size.y);
            // if(sX>0)sizeX -= (sX - 1);
            // if(sY>0)sizeY -= (sY - 1);
            var blockTexturesArray = new List<Texture2DSize>();

            for (int y = 0; y < sizeY; y++)
            {
                GUILayout.BeginHorizontal();
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        var size = level.size.x * level.size.y;
                        if (level.Blocks.Length != size) InitBlocks(size);

                        var sq = level.Blocks[y * level.size.x + x];
                        if (sq.type == BlocksTypes.None) UnityEngine.GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
                        var vector2Int = sq?.size == null || sq.size == Vector2Int.zero ? Vector2Int.one : sq.size;
                        if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                        {
                            if (brush != null)
                            {
                                if (sq.icon != brush.icon || brush.icon == null)
                                {
                                    if (brush.prefab != null && brush.prefab.GetComponent<OverlapItem>() && sq.type == BlocksTypes.Item)
                                    {
                                        level.Blocks[y * level.size.x + x].overlapBlockObject = level.Blocks[y * level.size.x + x].overlapBlockObject != null ? null : brush.prefab;
                                    }
                                    else
                                    {
                                        level.Blocks[y * level.size.x + x] = brush.Clone();
                                        level.Blocks[y * level.size.x + x].ChangeLayer();
                                    }
                                }
                                else if (sq.icon == brush.icon && sq.layer > 0)
                                    level.Blocks[y * level.size.x + x].ChangeLayer();
                                else if (sq.icon == brush.icon && sq.secondClick == EditorIconTypes.Rotatable)
                                {
                                    level.Blocks[y * level.size.x + x].Rotate();
                                    // GUIUtility.RotateAroundPivot (0.0f, new Vector2(240.0f, 160.0f));
                                }
                                else
                                    level.Blocks[y * level.size.x + x] = new LevelBlock(BlocksTypes.Square, null, null, Vector2Int.one);
                            }
                        }

                        var lastRect = GUILayoutUtility.GetLastRect();
                        lastRect.width *= vector2Int.x;
                        lastRect.height *= vector2Int.y;
                        var t = sq.GetTextures();
                        foreach (var texture2D in t)
                        {
                            blockTexturesArray.Add(new Texture2DSize(texture2D, lastRect, sq.rotate));
                            // if (texture2D != null) UnityEngine.GUI.DrawTexture(lastRect, texture2D);
                        }

                        UnityEngine.GUI.backgroundColor = Color.white;
                    }
                }
                GUILayout.EndHorizontal();
            }

            foreach (var texture2DSiz in blockTexturesArray)
            {
                var matrix4X4 = UnityEngine.GUI.matrix;
                if (texture2DSiz.rotate)
                    GUIUtility.RotateAroundPivot(90.0f, texture2DSiz.rect.center);
                if (texture2DSiz != null && texture2DSiz.Texture2D != null) UnityEngine.GUI.DrawTexture(texture2DSiz.rect, texture2DSiz.Texture2D);
                UnityEngine.GUI.matrix = matrix4X4;
            }

            if (EditorGUI.EndChangeCheck()) Save();
        }

        public class Texture2DSize
        {
            public Texture2D Texture2D;
            public Rect rect;
            public bool rotate;

            public Texture2DSize(Texture2D texture2D, Rect r, bool rotate)
            {
                Texture2D = texture2D;
                rect = r;
                this.rotate = rotate;
            }
        }

        private void InitBlocks(int size)
        {
            level.Blocks = new LevelBlock[size];
            var emptySquare = prefabs.Find(x => x.type == BlocksTypes.Square);
            for (int i = 0; i < level.Blocks.Length; i++)
            {
                level.Blocks[i] = new LevelBlock(BlocksTypes.Square, emptySquare.prefab, null, Vector2Int.one);
            }

            Save();
        }


        void Save()
        {
            EditorUtility.SetDirty(level);
            // AssetDatabase.SaveAssets();
        }
    }
}