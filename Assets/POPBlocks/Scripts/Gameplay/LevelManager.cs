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
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using POPBlocks.Scripts.Boosts;
using POPBlocks.Scripts.Effects;
using POPBlocks.Scripts.GameGUI;
using POPBlocks.Scripts.GameGUI.Popups;
using POPBlocks.Scripts.Items;
using POPBlocks.Scripts.LevelHandle;
using POPBlocks.Scripts.Pool;
using POPBlocks.Scripts.Popups;
using POPBlocks.Scripts.Scriptables;
using POPBlocks.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if EPSILON||GAMESPARKS||PLAYFAB
using POPBlocks.Server.Network;
#if EPSILON
using EpsilonServer;
#endif
#endif

namespace POPBlocks.Scripts
{
    /// <summary>
    /// basic gameplay functions and variables used from game scene 
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        //maximum colors for the current level, loading from the level file
        public int maxColors;
        //current level number
        public int levelNum = 1;
        //instance
        public static LevelManager Instance;
        //game status field
        private GameState gameState = GameState.Prepare;
        //temp list of matched items 
        public List<Item> matchedSquares = new List<Item>();
        //list of destroying bonuses for Pinwheel bonus, to wait while all of them destroy
        public List<Item> destroyingBonusesList = new List<Item>();
        //list of current matches on the field
        public List<Matches> matches = new List<Matches>();
        //basic falling items speed, using for randomise falling animation around its value
        public float speed = 20;
        //bonus match settings, keeping info about requiring items count
        [HideInInspector] public BonusMatchesSettings bonusMatch;
        //moves counter
        [HideInInspector] public int moveCounter;
        //level data reference
        public Level level;
        //GUI element for moves
        public GUICounter moves;
        //GUI element for score
        public GUICounter score;
        //GUI element for targets
        [Header("UI objects")] public Transform targetHolder;
        //target element prefab for GUI
        public GameObject targetPrefab;
        //field object reference
        private Field field;
        //GUI character animator reference
        public Animator characterAnimator;
        //skip win button
        public GameObject skipWIn;
        
        public delegate void GameStateDelegate(GameState state);
        //game status changing event
        public static event GameStateDelegate OnGamestateChanged;

        public delegate void DelEvent();
        //move was done event
        public static event DelEvent OnAfterMove;
        //level loaded event
        public static event DelEvent OnLevelLoaded;
        //start touching items event (begin move)
        public static event DelEvent OnStartMove;
        //any item was touched
        public delegate void DelEventItem(Item item);
        public static event DelEventItem OnItemTouched;

        public delegate void DelEventSprite(Sprite spr, Vector2 pos, Item item, Action<bool> callback);
        //item destroyed event for target counter
        public static event DelEventSprite OnDestroySprite;
        //event for unity simulation purposes
        public delegate void WinLose(int score, int stars, int moveRest, bool fail);
        public static event WinLose WinLoseEvent;
        //Popup manager reference
        private PopupManager popupManager;
        //debug settings reference
        [HideInInspector]public DebugSettings DebugSettings;
        //game status
        public GameState GameState
        {
            get => gameState;
            set
            {
                gameState = value;
                OnGamestateChanged?.Invoke(gameState);
                switch (gameState)
                {
                    case GameState.Prepare:
                    {
                        var popup = popupManager.play2.Show();
                        popup.OnHide += () => GameState = GameState.CameraAnimation;
                        break;
                    }
                    case GameState.CameraAnimation:
                    {
                       
                        break;
                    }
                    case GameState.Regen:
                    {
                        StartCoroutine(RegenLevel());
                        break;
                    }
                    case GameState.PreWin:
                    {
                        PlayerPrefs.SetInt("win",1);
                        PlayerPrefs.Save();
                        GameManager.Instance.win = true;
                        DelayedCall(1, () => popupManager.preComplete.Show());
                        break;
                    }
                    case GameState.Tutorial:
                    {
                        break;
                    }
                    case GameState.Playing:
                    {
                        if(!boostSetup)
                            StartCoroutine(SetBoosts());
                        break;
                    }
                    case GameState.PreWinAnimation:
                    {
                        WinLoseEvent?.Invoke(score.GetValue(), level.stars.GetEarnedStars(score.GetValue()), moves.GetValue(), false);
                        skipWIn.SetActive(true);
                        preWinAnimation = StartCoroutine(PreWinAnimation());
                        break;
                    }
                    case GameState.Win:
                    {
                        var stars = level.stars.GetEarnedStars(score.GetValue());
                        // Debug.Log("Win level " + levelNum + " score" + score.GetValue() + " stars " + level.stars.GetEarnedStars(score.GetValue()) + " moves rest " + moves.GetValue());
                        GameManager.Instance._mapProgressManager.SaveLevelStarsCount(levelNum, stars, score.GetValue());
                        #if PLAYFAB || GAMESPARKS
                        NetworkManager.dataManager.SetPlayerScore(levelNum, score.GetValue());
                        NetworkManager.dataManager.SetPlayerLevel(levelNum + 1);
                        NetworkManager.dataManager.SetStars(levelNum);
                        #elif EPSILON
                        NetworkManager.dataManager.DownloadPlayerData(); 
                        #endif

                        popupManager.win.Show();
                        break;
                    }
                    case GameState.Prefailed:
                    {
                        WinLoseEvent?.Invoke(score.GetValue(), level.stars.GetEarnedStars(score.GetValue()), moves.GetValue(), true);
                        popupManager.outOfMoves.Show();
                        break;
                    }
                    case GameState.GameOver:
                    {
                        // Debug.Log("Lost level " + levelNum + "  score" + score.GetValue() + " stars " + level.stars.GetEarnedStars(score.GetValue()));
                        popupManager.lose.Show();
                        break;
                    }
                }
            }
        }
        //Regenerate level if no matches
        private IEnumerator RegenLevel()
        {
            popupManager.noMatches.Show();
            yield return new WaitForSeconds(1);
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.noMatch);
            var list = Field.Instance.items.Where(i => !(i is null) && i.CompareTag("Item") && i.colorComponent.Length > 0);
            foreach (var i in list)
            {
                i.SqueezeAnimation(() => i.colorComponent[0].RandomizeColor(Array.Empty<int>()), FinishDestroy, 0.3f);
            }
            GameState = GameState.Playing;
        }
        //delayed call funtion
        public void DelayedCall(float sec, Action callback)
        {
            StartCoroutine(DelayedCallCour(sec, callback));
        }

        private IEnumerator DelayedCallCour(float sec, Action callback)
        {
            yield return new WaitForSeconds(sec);
            callback?.Invoke();
        }
        // trail prefab for win animation
        public GameObject winTrail;
        //skip win function
        public void SkipWin()
        {
            if(GameState == GameState.PreWinAnimation)
            {
                Time.timeScale = 100;
                SoundBase.Instance.MuteSoundTemporarily();
            }
        }

        //win animation, turning and exploding the items 
        private IEnumerator PreWinAnimation()
        {
            var items = field.GetRandomItems(5, true, true);
            for (var i = 0; i < items.Count; i++)
            {
                if (items?[i] == null) continue;
                var go = Instantiate(winTrail);
                go.transform.position = moves.transform.position;
                go.GetComponent<TrailEffect>().target = items[i];
                go.GetComponent<TrailEffect>().StartAnim(target =>
                {
                    moves.DecrementValue(1);
                    if (target != null && target.gameObject.activeSelf)
                    {
                        var itemTypeChanger = target.GetComponent<ItemTypeChanger>();
                        if (!(itemTypeChanger is null)) itemTypeChanger.SetBomb();
                    }
                });
                yield return new WaitForSeconds(0.3f);
            }

            moves.SetValue(0);
            yield return new WaitForSeconds(1f);

            var bombs = FindObjectsOfType<BonusItem>().Where(i => i.gameObject.activeSelf);
            foreach (var bomb in bombs)
            {
                bomb.ActivateBonus(false, null);
                FinishDestroy();
                yield return new WaitForSeconds(0.3f);
            }

            FinishDestroy();
            yield return new WaitWhile(() => Field.Instance.items.Any(i => !(i is null) && i.falling));
            if (Time.timeScale == 100)
            {
                Time.timeScale = 1;
                SoundBase.Instance.RestoreSound();
            }
            GameState = GameState.Win;
        }
        //game over calling from settings by "Home" button
        public void SetGameover()
        {
            GameState = GameState.GameOver;
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
            bonusMatch = Resources.Load<BonusMatchesSettings>("Settings/BonusMatchesSettings");
            gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
            DOTween.Init();
            levelNum = GameManager.Instance._mapProgressManager.CurrentLevel;
            var testLevel = PlayerPrefs.GetInt("OpenLevelTest", 0);
            if (testLevel > 0)
            {
                levelNum = testLevel;
                GameManager.Instance._mapProgressManager.CurrentLevel = levelNum;
                PlayerPrefs.SetInt("OpenLevelTest", 0);
                PlayerPrefs.Save();
            }
        }

        private void Start()
        {
            DebugSettings = Resources.Load<DebugSettings>("Settings/DebugSettings");
            Field.Instance.AI = DebugSettings.AI;
            popupManager = FindObjectOfType<PopupManager>();
            if (GameManager.Instance.restartedLevel > 0)
            {
                levelNum = GameManager.Instance.restartedLevel;
                GameManager.Instance.restartedLevel = 0;
            }
            bool dontLoadLevel = false;
#if UNITY_GAME_SIMULATION && SIMULATION_ENABLED
            dontLoadLevel = true;
#endif
            if(!dontLoadLevel)
                LoadLevel(levelNum);
        }

        public void LoadLevel(int num, int overrideColor = 0, int overrideMoves = 0)
        {
            level = Resources.Load<Level>("Levels/Level_" + num);
            moves.SetValue(overrideMoves==0?level.moves:overrideMoves);
            maxColors = overrideColor==0? level.colors: overrideColor;
            field = FindObjectOfType<Field>();
            field.LoadLevel(level);
            OnLevelLoaded?.Invoke();
            #if PLAYFAB
            NetworkManager.dataManager.GetPlayerScore(num);
            #endif
            levelLoaded = true;
        }

        //put purchased boosts from the menu play
        private IEnumerator SetBoosts()
        {
            boostSetup = true;
            var items = field.GetRandomItems(10, true, true);
            var list = items.Where(i => !(i is null) && i.bonus == null/* && i.overlapItem == null */&& i.GetComponent<ItemTypeChanger>() != null).Select(i => i
            .GetComponent<ItemTypeChanger>())
                .Randomize().Take(GameManager.Instance.boostTypes.Count*2).ToArray();
            for (var index = 0; index < Mathf.Min(GameManager.Instance.boostTypes.Count, list.Length); index++)
            {
                var boostType = GameManager.Instance.boostTypes[index];
                if (boostType == BoostType.Rocket)
                    list[index].SetRocket(true);
                if (boostType == BoostType.Bomb)
                    list[index].SetBomb(true);
                if (boostType == BoostType.Pinwheel) list[index].SetPinwheel(true);

                yield return new WaitForSeconds(0.2f);
            }

            GameManager.Instance.boostTypes.Clear();
        }
        //list of target GUI objects
        public List<TargetObject> targetObjects = new List<TargetObject>();
        //position of the latest touched item
        [HideInInspector] public Vector3 lastTouchPosition;
        //game settings reference
        [HideInInspector] public GameSettings gameSettings;
        //true if boost was placed on the field before start
        private bool boostSetup;
        private Coroutine preWinAnimation;
        public int moveAfterCounter;
        [FormerlySerializedAs("bonusAppearAnimate")]
        public bool blockFalling;
        public bool levelLoaded;
        private int failAttempts;
        public bool fallingAnimation;
        [HideInInspector] public bool blockInput;

        private void Update()
        {
#if UNITY_EDITOR
            
            if (Input.GetKeyDown(DebugSettings.Regen) && DebugSettings.enableShortcuts)
            {
                GameState = GameState.Regen;
            }

            if (Input.GetKeyDown(DebugSettings.Win) && DebugSettings.enableShortcuts)
            {
                GameState = GameState.PreWin;
            }

            if (Input.GetKeyDown(DebugSettings.Lose) && DebugSettings.enableShortcuts)
            {
                GameState = GameState.Prefailed;
            }

            if (Input.GetKeyDown(DebugSettings.Restart) && DebugSettings.enableShortcuts)
            {
                RestartLevel();
            }
            
            if (Input.GetKeyDown(DebugSettings.lastMove) && DebugSettings.enableShortcuts)
            {
                moves.SetValue(1);
            }
            
            if (gameState == GameState.Playing && Time.timeScale != 100)
                Time.timeScale = DebugSettings.TimeScaleItems;
            else if( Time.timeScale != 100)
                Time.timeScale = DebugSettings.TimeScaleUI;
#endif
            if(Input.GetMouseButtonDown(0) && GameState == GameState.PreWinAnimation) SkipWin();
        }

        //restart level
        public void RestartLevel()
        {
            if (GameManager.Instance.CheckLife())
            {
                GameManager.Instance.restartedLevel = levelNum;
                SceneManager.LoadScene("game");
            }
        }

        //update jelly target
        public void UpdateTargets()
        {
            targetObjects.ForEachY(i => i.UpdateTargetFromField());
        }
        /// <summary>
        /// Activate touching item
        /// </summary>
        /// <param name="itemMatching">selected item</param>
        public void ActivateItem(Item itemMatching)
        {
            OnItemTouched?.Invoke(itemMatching);
            lastTouchPosition = itemMatching.transform.position;
            if (GameManager.Instance.boostTypesGame.Any())
            {
                blockInput = true;
                var boost = GameManager.Instance.boostTypesGame.First();
                BoostButton.ProceedBoost(boost, itemMatching, () => blockInput = false);
                GameManager.Instance.boostTypesGame.Clear();
            }
            else if (moves.GetValue() > 0)
                StartCoroutine(ActivateItemAndMatch(itemMatching));
        }

        /// <summary>
        /// Activate, match and explode animation loop
        /// </summary>
        /// <param name="itemMatching">touched item</param>
        /// <returns></returns>
        IEnumerator ActivateItemAndMatch(Item itemMatching)
        {
            OnStartMove?.Invoke();
            LevelManager.Instance.fallingAnimation = true;
            Item[] matchedItems = { };
            float animTime = 0.1f;
            if (itemMatching.matchable)
            {
                foreach (var matchese in matches)
                {
                    if (matchese.items.Any(i => i == itemMatching)) matchedItems = matchese.items;
                }

                if (matchedItems.Length > 0)
                {
                    blockInput = true;
                    ShowScorePop(matchedItems.Sum(i => i.score), itemMatching.transform.position);
                    moveCounter++;
                    moves.DecrementValue(1);
                }
                else
                {
                    itemMatching.animator?.SetTrigger("wrong");
                }

                var itemMatchingBonusNum = itemMatching.bonusNum;
                if (itemMatchingBonusNum > 0)
                    blockFalling = true;
                foreach (var item in matchedItems)
                {
                    if (!item) continue;

                    if (itemMatchingBonusNum == 0)
                    {
                        item.DestroyItemStart(false,0, noEffect: false, effectName: "", destroyAround: true);
                    }
                    else
                    {
                        item.transform.DOMove(itemMatching.transform.position, animTime).OnComplete(() => { item.DestroyItemStart(false, 0,noEffect: true, "", true); });
                    }
                }

                if (itemMatchingBonusNum > 0 && itemMatchingBonusNum < bonusMatch.BonusMatches.Length)
                {
                    yield return new WaitForSeconds(animTime);
                    characterAnimator.SetTrigger("happy");
                    var prefabName = bonusMatch.BonusMatches.First(i => i.bonusNum == itemMatchingBonusNum).prefab?.name;
                    var item = Field.Instance.CreateItem(prefabName, itemMatching.index, itemMatching.pos);
                    if (item.bonus.type == BonusItemTypes.Pinwheel) item.Color = itemMatching.Color;
                    blockFalling = false;
                }

                if (matchedItems.Length == 0) yield break;
            }
            else if (itemMatching.bonus)
            {
                blockInput = true;
                bool finish = false;
                blockFalling = true;
                itemMatching.bonus.ActivateBonus(true, () => finish = true);
                yield return new WaitUntil(() => finish);
                if(itemMatching.gameObject.activeSelf)
                    itemMatching.DestroyItemFinish();
            }
            else yield break;

            int counterDelay = 0;
            for (var index = 0; index < destroyingBonusesList.Count; index++)
            {
                var item = destroyingBonusesList[index];
                if (!item) continue;
                if (item.bonus && item.gameObject.activeSelf)
                {
                    yield return new WaitForSeconds(0.2f);
                    item.bonus.ActivateBonus(false, () => counterDelay++);
                }
            }

            yield return new WaitWhile(() => destroyingBonusesList.Any(i=>i.gameObject.activeSelf));
            blockFalling = false;

            OnAfterMove?.Invoke();

            yield return new WaitForFixedUpdate();
            yield return new WaitWhile(()=> Field.Instance.items.Any(x => x?.falling??false));
            yield return new WaitWhile(() => GameObject.FindWithTag("AnimatedTarget"));
            moveAfterCounter++;
            blockInput = false;
            if(!blockFalling)
                FinishDestroy();
        }

        /// <summary>
        /// Show score animation
        /// </summary>
        /// <param name="score">score amount</param>
        /// <param name="transformPosition">score position</param>
        public void ShowScorePop(int score, Vector3 transformPosition)
        {
            var scoreObject = ObjectPooler.Instance.GetPooledObject("Score", null, false);
            scoreObject.GetComponent<TextMeshProUGUI>().text = score.ToString();
            scoreObject.transform.position = transformPosition;
            scoreObject.gameObject.SetActive(true);
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.itemExplosion);
        }

        //Check win or lose conditions
        public void CheckMovesAndTargets()
        {
            bool targetFinished = targetObjects.All(i => i.IsDone());
            if (GameState != GameState.Playing || (GameObject.FindWithTag("AnimatedTarget") && !targetFinished) || fallingAnimation) return;
            if (targetFinished)
                GameState = GameState.PreWin;
            else if (moves.GetValue() <= 0)
            {
                if (gameSettings.failAttempts > failAttempts)
                {
                    failAttempts++;
                    GameState = GameState.Prefailed;
                }
                else GameState = GameState.GameOver;
            }
        }

        //Calling after explosion animation of touching
        public void FinishDestroy()
        {
            Field.Instance.FallAndGenerateItems();
            destroyingBonusesList.Clear();
            // CheckNoMatches();
        }

        public void CheckNoMatches()
        {
            if (matches.Count == 0 && !Field.Instance.items.Any(i=>i.bonus) && gameState == GameState.Playing) GameState = GameState.Regen;
        }

        //Adding matches to array
        public void AddMatches()
        {
            var v = matches.SelectMany(i => i.items);
            if (!v.Any(i=>matchedSquares.Contains(i)))
            {
                matches.Add(new Matches {items = matchedSquares.ToArray()});
            }
        }

        /// <summary>
        /// Destroy sprite event for target counter
        /// </summary>
        /// <param name="sprite">exploded sprite</param>
        /// <param name="pos">position</param>
        /// <param name="item">destroyed item reference</param>
        /// <param name="callback"></param>
        public void DestroySpriteEvent(Sprite sprite, Vector2 pos, Item item, Action<bool> callback)
        {
            OnDestroySprite?.Invoke(sprite, pos, item, callback);
        }
    }

    [System.Serializable]
    public class Matches
    {
        public Item[] items;
    }

    public enum GameState
    {
        Prepare,
        Playing,
        GameOver,
        Win,
        Pause,
        Prefailed,
        PreWin,
        PreWinAnimation,
        Tutorial,
        Regen,
        PrepareTutorial,
        CameraAnimation,
        SwitchTutorial
    }
}