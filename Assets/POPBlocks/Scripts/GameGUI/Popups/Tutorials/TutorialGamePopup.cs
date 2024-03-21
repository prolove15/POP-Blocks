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

using System.Linq;
using POPBlocks.Scripts.Boosts;
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Scripts.GameGUI.Popups.Tutorials
{
    public class TutorialGamePopup : Popup
    {
        public TextMeshProUGUI textGUI;
        public GameObject button;
        [HideInInspector] public bool hideByTouchItem;
        public GameObject finger;
        public RectTransform mask;
        [HideInInspector] public Rect itemRect;
        [HideInInspector] public int selectMatch;
        [HideInInspector] public BonusItemTypes selectBonusItemType;
        [HideInInspector] public string uiHighlightObject;
        [HideInInspector] public bool moveFingerAnim;
        public Vector3 fingerOffset;
        [HideInInspector] public Transform hightLightedUIObject;
        [HideInInspector] public bool selectAllItems;

        [HideInInspector] public Vector2Int selectBlock;
        [HideInInspector] public Conditions condition;

        public SelectionTypes selection;
        public ShowNextTutorial showNextWindowOn;
        public int selectRowColumn;

        private void OnEnable()
        {
            finger.SetActive(false);
        }

        private void Start()
        {
            if (uiHighlightObject != "")
            {
                var strings = uiHighlightObject.Split('/');
                hightLightedUIObject = GameObject.Find(strings[0]).transform.Find(uiHighlightObject.Remove(uiHighlightObject.IndexOf(strings[0]), strings[0].Length + 1));
                var boostButton = hightLightedUIObject.GetComponent<Button>();
                if (boostButton != null)
                {
                    var boostBur = boostButton.GetComponent<BoostButton>();
                    if (boostBur.count == 0)
                    {
                        boostBur.count = 3;
                        boostBur.UpdateText();
                    }
                    boostButton.onClick.AddListener(TutorialCompleted);
                }
            }          
        }
        
        

        private void ItemTouched(Item item)
        {
            if(selectBlock != Vector2Int.zero && item.pos == selectBlock) TutorialCompleted();
            else if(selectBlock == Vector2Int.zero) TutorialCompleted();
        }

        protected override void ShowBeforeAnimation()
        {
            var worldRect = mask.GetWorldRect();
            worldRect.x -= transform.position.x;
            if (selection == SelectionTypes.SelectMatch)
            {
                if (LevelManager.Instance.matches.Any(i => MatchCondition(i)))
                {
                    var list = LevelManager.Instance.matches.Where(i => i.items.Any(x => worldRect.Contains(x.transform.position))).First(i =>
                        MatchCondition(i));
                    UnmaskItems(list.items);
                }
            }
            else if (selection == SelectionTypes.SelectBonus)
            {
                if (Field.Instance.items.Any(i => !(i?.bonus is null) && i.bonus.type == selectBonusItemType))
                {
                    var item = Field.Instance.items.First(i => !(i?.bonus is null) && i.bonus.type == selectBonusItemType);
                    UnmaskItems(new[] {item});
                }
            }
            else if (selection == SelectionTypes.SelectAllBlocks)
            {
                var list = Field.Instance.items;
                UnmaskItems(list);
            }
            else if (selection == SelectionTypes.SelectBlock)
            {
                var list = Field.Instance.items.Where(i => i != null && i.pos == selectBlock).ToArray();
                UnmaskItems(list);
            }
            
            else if (selection == SelectionTypes.SelectRow)
            {
                var list = Field.Instance.GetRow(selectRowColumn).ToArray();
                UnmaskItems(list);
            }
            else if (selection == SelectionTypes.SelectColumn)      
            {
                var list = Field.Instance.GetColumn(selectRowColumn).ToArray();
                UnmaskItems(list);
            }
            base.ShowBeforeAnimation();
        }

        protected override void AfterShowAnimation()
        {
            PrepareTutorial();
            base.AfterShowAnimation();
        }

        private void OnDisable()
        {
            if (hideByTouchItem)
                LevelManager.OnItemTouched -= ItemTouched;
            if (selectAllItems || selectBlock != Vector2Int.zero)
                LevelManager.OnItemTouched -= ItemTouched;
        }

        public void SetText(string txt, bool showButton)
        {
            textGUI.text = txt;
            button.SetActive(showButton);
        }

        void PrepareTutorial()
        {
            hideByTouchItem = selectMatch > 0 || selectBonusItemType != BonusItemTypes.None || selectRowColumn > 0;
            if (hideByTouchItem)
                LevelManager.OnItemTouched += ItemTouched;
            if (selectAllItems || selectBlock != Vector2Int.zero)
                LevelManager.OnItemTouched += ItemTouched;
            
            var worldRect = mask.GetWorldRect();
            if (selection == SelectionTypes.SelectMatch)
            {
                if (LevelManager.Instance.matches.Any(i => MatchCondition(i)))
                {
                    var list = LevelManager.Instance.matches.Where(i => i.items.Any(x => worldRect.Contains(x.transform.position))).First(i =>
                        MatchCondition(i));
                    var posFinger = list.items.Last().transform.position;
                    finger.SetActive(true);
                    finger.transform.position = (Vector2) posFinger + Vector2.right * 0.5f + Vector2.down * 0.5f;
                }
            }
            else if (selection == SelectionTypes.SelectBonus)
            {
                if (Field.Instance.items.Any(i => !(i?.bonus is null) && i.bonus.type == selectBonusItemType))
                {
                    var item = Field.Instance.items.First(i => !(i?.bonus is null) && i.bonus.type == selectBonusItemType);
                    var posFinger = (Vector2) item.transform.position;
                    finger.SetActive(true);
                    finger.transform.position = posFinger + Vector2.right * 0.5f + Vector2.down * 0.5f;
                    PopupManager.Instance.SetUnMask(new Rect(mask.anchoredPosition + GetComponent<RectTransform>().anchoredPosition, mask.sizeDelta));

                }
            }
            else if (selection == SelectionTypes.SelectAllBlocks)
            {
                var list = Field.Instance.items;
                UnmaskItems(list);
            }
            else if (selection == SelectionTypes.SelectBlock)
            {
                var list = Field.Instance.items.Where(i => i != null && i.pos == selectBlock).ToArray();
                UnmaskItems(list);
            }

            else if (selection == SelectionTypes.SelectRow)
            {
                var list = Field.Instance.GetRow(selectRowColumn).ToArray();
                var item = list.WhereNotNull().Where(i => i.bonus == false).NextRandom();
                var posFinger = (Vector2) item.transform.position;
                finger.SetActive(true);
                finger.transform.position = posFinger + Vector2.right * 0.5f + Vector2.down * 0.5f;
                UnmaskItems(list);
            }
            else if (selection == SelectionTypes.SelectColumn)
            {
                var list = Field.Instance.GetColumn(selectRowColumn).ToArray();
                var item = list.WhereNotNull().Where(i => i.bonus == false).NextRandom();
                var posFinger = (Vector2) item.transform.position;
                finger.SetActive(true);
                finger.transform.position = posFinger + Vector2.right * 0.5f + Vector2.down * 0.5f;
                UnmaskItems(list);
            }
            
            if (uiHighlightObject != "")
            {
                var canvas = hightLightedUIObject.gameObject.AddComponent<Canvas>();
                hightLightedUIObject.gameObject.AddComponent<GraphicRaycaster>();
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "Popups";
                if (moveFingerAnim)
                {
                    finger.SetActive(true);
                    finger.transform.position = hightLightedUIObject.position + fingerOffset;
                }
            }

            // if (mask.gameObject.activeSelf)
            // {
            //     PopupManager.Instance.SetUnMask(new Rect(mask.anchoredPosition + GetComponent<RectTransform>().anchoredPosition, mask.sizeDelta));
            // }

            if (showNextWindowOn == ShowNextTutorial.AfterFinishMove)
                LevelManager.OnAfterMove += NextTutorial;
            else if (showNextWindowOn == ShowNextTutorial.OnTouch)
                OnHide += NextTutorial;
            else if (showNextWindowOn == ShowNextTutorial.None)
                OnHide += NextTutorial;
            LevelManager.Instance.GameState = GameState.Tutorial;
            if (button != null && button.transform.GetChild(0)?.GetComponent<Button>() != null)
            {
                button.transform.GetChild(0).GetComponent<Button>().interactable = true;
            }
        }

        private void NextTutorial()
        {
            if (showNextWindowOn == ShowNextTutorial.AfterFinishMove)
                LevelManager.OnAfterMove -= NextTutorial;
            else if (showNextWindowOn == ShowNextTutorial.OnTouch)
                OnHide -= NextTutorial;
            else if (showNextWindowOn == ShowNextTutorial.None)
                OnHide -= NextTutorial;
            
            if (showNextWindowOn == ShowNextTutorial.None)
                LevelManager.Instance.GameState = GameState.Playing;
            else
                FindObjectOfType<TutorialManager>().ShowNextTutorial();
        }

        private bool MatchCondition(Matches i)
        {
            if (condition == Conditions.Equals)
                return i.items.Length == selectMatch;
            else if (condition == Conditions.MoreOrEquals)
                return i.items.Length >= selectMatch;
            else if (condition == Conditions.More)
                return i.items.Length > selectMatch;
            else if (condition == Conditions.Less)
                return i.items.Length < selectMatch;
            else if (condition == Conditions.LessOrEquals)
                return i.items.Length <= selectMatch;
            return false;
        }

        private void UnmaskItems(Item[] list)
        {
            Vector2 v1 = new Vector2(list.Min(x => x.transform.position.x), list.Max(x => x.transform.position.y));
            Vector2 v2 = new Vector2(list.Max(x => x.transform.position.x), list.Min(x => x.transform.position.y));
            var v1Screen = PopupManager.Instance.GetComponent<Canvas>().WorldToCanvas(v1);
            var v2Screen = PopupManager.Instance.GetComponent<Canvas>().WorldToCanvas(v2);
            var vector2 = new Vector2(v1Screen.x + itemRect.x, v1Screen.y + itemRect.y);
            var delta = new Vector2(v2Screen.x + itemRect.width, v2Screen.y - itemRect.height) - v1Screen;
            var rect = new Rect(vector2.x, vector2.y, Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            foreach (var item in list)
            {
                item.BackupLayer();
                item.SetLayer(1, "Popups");
            }

            PopupManager.Instance.SetUnMask(rect);
        }

        public void TutorialCompleted()
        {
            if (uiHighlightObject != "")
            {
                var strings = uiHighlightObject.Split('/');
                var tr = GameObject.Find(strings[0]).transform.Find(uiHighlightObject.Remove(uiHighlightObject.IndexOf(strings[0]), strings[0].Length + 1));
                Destroy(tr.GetComponent<GraphicRaycaster>());
                Destroy(tr.GetComponent<Canvas>());
            }

            Field.Instance.items.WhereNotNull().ForEachY(i => i.RestoreLayer());
            finger.SetActive(false);
            Hide();
        }

        public override void BackButton()
        {
            // TutorialCompleted();
            base.BackButton();
        }
    }
}