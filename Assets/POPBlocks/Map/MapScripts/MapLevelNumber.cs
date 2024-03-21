using TMPro;
using UnityEngine;

namespace POPBlocks.MapScripts
{
    /// <summary>
    /// Level number handler on the map
    /// </summary>
    public class MapLevelNumber : MonoBehaviour
    {
        private GameObject canvasMap;
        MapLevel mapLevel;
        Transform pin;

        private TextMeshProUGUI text;

        // Use this for initialization
        void OnEnable()
        {
            canvasMap = GameObject.Find("WorldCanvas");
            mapLevel = transform.parent.parent.GetComponent<MapLevel>();
            if (transform.parent.gameObject == canvasMap) return;
            int num = mapLevel.Number;
            text = GetComponent<TextMeshProUGUI>();
            text.text = "" + num;
            pin = transform.parent;
            // transform.SetParent(canvasMap.transform, true);
        }

        // Update is called once per frame
        void Update()
        {
            if (mapLevel != null && mapLevel.IsLocked && gameObject.activeSelf) text.enabled = false;
            else text.enabled = true;
        }

    }
}
