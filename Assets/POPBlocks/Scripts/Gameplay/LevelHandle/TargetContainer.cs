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
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle
{
    // [RequireComponent(typeof(LayoutGroup))]
    public class TargetContainer : MonoBehaviour
    {
        public GameObject targetPrefab;
        public bool tempGUI;

        private void OnEnable()
        {
            StartCoroutine(PrepareTargets());
        }

        private IEnumerator PrepareTargets()
        {
            List<TargetSprite> targets = new List<TargetSprite>();

            if (LevelManager.Instance == null)
            {
                var level = Resources.Load<Level>("Levels/Level_" + GameManager.Instance._mapProgressManager.CurrentLevel);
                foreach (var target in level.targets)
                {
                    var targetSprite = new TargetSprite();
                    for (var i = 0; i < target.target.targetSprites.Count; i++)
                    {
                        if (target.count.values[i] == 0 && !target.target.countFromField) continue;
                        var spr = target.target.targetSprites[i];
                        targetSprite.sprites.Add(spr.icon);
                        if (spr.uiSprite)
                        {
                            targetSprite.count = target.count.values[i];
                            targetSprite.countFromField = target.target.countFromField;
                            targetSprite.sprites.AddRange(target.target.targetSprites.Where(x => !x.uiSprite).Select(x => x.icon).ToList());
                            targetSprite.prefab = target.target.prefab;
                            targets.Add(targetSprite);
                            targetSprite = new TargetSprite();
                        }
                    }
                }
                AddTargets(targets);
            }
            else
            {
                yield return new WaitUntil(() => Field.Instance);
                yield return new WaitUntil(() => Field.Instance.targets.Any());
                AddTargets(Field.Instance.targets);
            }
        }

        private void AddTargets(List<TargetSprite> instanceTargets)
        {
            foreach (var target in instanceTargets)
            {
                var g = Instantiate(targetPrefab, transform, true);
                g.transform.localPosition = Vector3.zero;
                g.transform.localScale = Vector3.one;
                var targetObject = g.GetComponent<TargetObject>();
                if (targetObject != null)
                {
                    targetObject.TargetBind = target;
                    targetObject.image.sprite = target.sprites[0];
                    // targetObject.image.SetNativeSize();
                    if (!tempGUI)
                    {
                        LevelManager.Instance?.targetObjects.Add(targetObject);
                        if (target.prefab != null)
                        {
                            // targetObject.image.transform.localPosition += Vector3.up * 20;
                            targetObject.image.transform.localScale = Vector3.one * 1.0f;
                        }
                    }
                }
            }
        }
    }
}