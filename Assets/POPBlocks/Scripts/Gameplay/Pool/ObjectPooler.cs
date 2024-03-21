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
using POPBlocks.Scripts.Items;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace POPBlocks.Scripts.Pool
{

    /// <summary>
    /// object pool. Uses to keep and activate items and effects
    /// </summary>
    [Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public string poolName;
        public int amountToPool;
        public bool shouldExpand = true;
        public bool inEditor = true;
    }

    // [ExecuteInEditMode]
    public class ObjectPooler : MonoBehaviour
    {
        public const string DefaultRootObjectPoolName = "Pooled Objects";

        public static ObjectPooler Instance;
        public string rootPoolName = DefaultRootObjectPoolName;
        public List<PoolBehaviour> pooledObjects = new List<PoolBehaviour>();
        private List<ObjectPoolItem> itemsToPool;
        private PoolerScriptable PoolSettings;


        void OnEnable()
        {
            LoadFromScriptable();
            Instance = this;
            if (!Application.isPlaying) return;
            ClearNullElements();

            foreach (var item in itemsToPool)
            {
                CreateObject(item);
            }
        }

        private void LoadFromScriptable()
        {
            PoolSettings = Resources.Load("Settings/PoolSettings") as PoolerScriptable;
            var items = Resources.LoadAll<PoolBehaviour>("Blocks").Select(i=>i.ConvertToObjectPoolItem());
            var items1 = Resources.LoadAll<PoolBehaviour>("Prefabs").Select(i=>i.ConvertToObjectPoolItem());
            itemsToPool = PoolSettings.itemsToPool.Concat(items).Concat(items1).ToList();

        }

        private void CreateObject(ObjectPoolItem item)
        {
            if (item == null) return;
            if (item.objectToPool == null) return;
            var pooledCount = pooledObjects.Count(i => i.itemName == item.objectToPool.name);
            for (int i = 0; i < item.amountToPool - pooledCount; i++)
            {
                CreatePooledObject(item);
            }
        }

        private void ClearNullElements()
        {
            pooledObjects.RemoveAll(i => i == null);
        }

        private GameObject GetParentPoolObject(string objectPoolName)
        {
            // Use the root object pool name if no name was specified
            // if (string.IsNullOrEmpty(objectPoolName))
            //     objectPoolName = rootPoolName;

            // if (GameObject.Find(rootPoolName) == null) new GameObject { name = rootPoolName };
            GameObject parentObject = GameObject.Find(objectPoolName);
            // Create the parent object if necessary
            if (parentObject == null)
            {
                parentObject = new GameObject();
                parentObject.name = objectPoolName;

                // Add sub pools to the root object pool if necessary
                if (objectPoolName != rootPoolName)
                    parentObject.transform.parent = transform;
            }

            return parentObject;
        }

        public void HideObjects(string tag)
        {
            var objects = GameObject.FindObjectsOfType<PoolBehaviour>().Where(i=>i.itemName == tag);
            foreach (var item in objects)
                item.gameObject.SetActive(false);
        }

        public void PutBack(GameObject obj)
        {
            obj.SetActive(false);
        }

        public GameObject GetPooledObject(string name, Object activatedBy=null, bool active = true, bool canBeActive = false)
        {
            ClearNullElements();
            PoolBehaviour obj = null;
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i] == null) continue;
                if ((!pooledObjects[i].gameObject.activeSelf || canBeActive) && pooledObjects[i].itemName == name)
                {
                    Item item = pooledObjects[i].GetComponent<Item>();
                    if(item && item.canBePooled )
                        obj = pooledObjects[i];
                    else if(!item )
                        obj = pooledObjects[i];
                    if(obj) break;
                }
            }

            if (itemsToPool == null) LoadFromScriptable();
            if (!obj)
            {
                foreach (var item in itemsToPool)
                {
                    if (item != null && item.objectToPool == null) continue;
                    if (item.objectToPool.name == name)
                    {
                        if (item.shouldExpand)
                        {
                            obj = CreatePooledObject(item);
                            break;
                        }
                    }
                }
            }

            if (obj != null)
            {
                obj.gameObject.SetActive(active);
                return obj.gameObject;
            }

            return null;
        }

        private PoolBehaviour CreatePooledObject(ObjectPoolItem item)
        {
            GameObject obj = Instantiate(item.objectToPool);
            // Get the parent for this pooled object and assign the new object to it
            var parentPoolObject = GetParentPoolObject(item.poolName);
            obj.transform.SetParent( parentPoolObject.transform);
            obj.name = item.objectToPool.name;
            var poolBehaviour = obj.GetComponent<PoolBehaviour>();
            if(poolBehaviour == null)
                poolBehaviour = obj.AddComponent<PoolBehaviour>();
            poolBehaviour.itemName = item.objectToPool.name;
            if(poolBehaviour.parentPath != null && poolBehaviour.parentPath != String.Empty)
            {
                obj.transform.SetParent(GameObject.Find(poolBehaviour.parentPath).transform);
                obj.transform.localScale = item.objectToPool.transform.localScale;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                PrefabUtility.RevertPrefabInstance(obj, InteractionMode.AutomatedAction);
            }
#endif

            obj.SetActive(false);
            pooledObjects.Add(poolBehaviour);


            return poolBehaviour;
        }

        public void DestroyObjects(string tag)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i].itemName == tag)
                {
                    DestroyImmediate(pooledObjects[i]);
                }
            }
            ClearNullElements();
        }
    }
}