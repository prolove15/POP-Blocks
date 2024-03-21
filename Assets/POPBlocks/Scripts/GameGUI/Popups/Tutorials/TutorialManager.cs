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
using System.Linq;
using Malee.List;
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.LevelHandle;
using UnityEngine;

namespace POPBlocks.Scripts.GameGUI.Popups.Tutorials
{
    public class TutorialManager : MonoBehaviour
    {
        private TutorialList tutorialLevels;
        private int index = -1;
        private TutorialLevel tut;

        private void Awake()
        {
            tutorialLevels = Resources.Load<TutorialSettings>("Settings/TutorialSettings").tutorials;
        }

        private void OnEnable()
        {
            index = -1;
            LevelManager.OnGamestateChanged += CheckGameState;
        }

        private void OnDisable()
        {
            LevelManager.OnGamestateChanged -= CheckGameState;
        }

        private void CheckGameState(GameState state)
        {
            if (state == GameState.PrepareTutorial) CheckTutorial();
        }

        private void CheckTutorial()
        {
            if (tutorialLevels.All(i => i.level != LevelManager.Instance.levelNum) || Field.Instance.AI)
            {
                StartGame();
                return;
            }

            tut = tutorialLevels.First(i => i.level == LevelManager.Instance.levelNum);
            ShowNextTutorial();
        }

        public void ShowNextTutorial()
        {
            index++;
            if (index < tut.windows.Length)
            {
                LevelManager.Instance.GameState = GameState.SwitchTutorial;
                var tutTutorialNum = tut.windows[index];
                var pop = (TutorialGamePopup) tutTutorialNum.tutorialPopup.GetPopup(true);
                pop.SetText(tutTutorialNum.description, tutTutorialNum.showButton);
                pop.selection = tutTutorialNum.selection.selectionTypes;
                if (tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectMatch)
                {
                    pop.selectMatch = tutTutorialNum.selection.selectMatch;
                    pop.condition = tutTutorialNum.selection.condition;
                }
                else if (tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectBonus)
                    pop.selectBonusItemType = tutTutorialNum.selection.selectBonusItemType;
                else if (tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectAllBlocks)
                    pop.selectAllItems = tutTutorialNum.selection.selectAllBlocks;
                else if (tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectBlock)
                    pop.selectBlock = tutTutorialNum.selection.selectBlock;
                else if (tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectColumn || tutTutorialNum.selection.selectionTypes == SelectionTypes.SelectRow)
                    pop.selectRowColumn = tutTutorialNum.selection.selectRowColumn;
                pop.showNextWindowOn = tutTutorialNum.showNextWindowOn;
                pop.noFade = tutTutorialNum.noFade;
                pop.uiHighlightObject = tutTutorialNum.uiHighlightObject;
                pop.Show(false/*() => ShowPopupNum()*/);
            }
            else
                StartGame();
        }

        private static void StartGame()
        {
            LevelManager.Instance.GameState = GameState.Playing;
        }
    }

    [Serializable]
    public class TutorialLevel
    {
        public int level;
        [Reorderable(elementNameOverride = "Window", expandByDefault = true)]
        public TutorialNums windows;
    }
    
    [Serializable]
    public class TutorialNums : ReorderableArray<TutorialLevelDescription>{}

    [Serializable]
    public class TutorialLevelDescription
    {
        public string description;
        public TutorialSelection selection;
        public bool showButton;
        public Popup tutorialPopup;
        public bool noFade;
        public string uiHighlightObject;
        public ShowNextTutorial showNextWindowOn;
    }

    [Serializable]
    public class TutorialSelection
    {
        public SelectionTypes selectionTypes;
        public int selectMatch;
        public Conditions condition;
        public BonusItemTypes selectBonusItemType;
        public bool selectAllBlocks;
        public Vector2Int selectBlock;
        public int selectRowColumn;
    }

    public enum ShowNextTutorial
    {
        OnTouch,
        AfterFinishMove,
        None
    }

    public enum SelectionTypes
    {
        SelectMatch,
        SelectBonus,
        SelectAllBlocks,
        SelectBlock,
        DontSelectBlocks,
        SelectRow,
        SelectColumn
    }

    public enum Conditions
    {
        Equals,
        MoreOrEquals,
        More,
        Less,
        LessOrEquals
    }
}