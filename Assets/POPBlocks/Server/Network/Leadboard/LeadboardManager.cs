using System.Collections;
using System.Collections.Generic;
#if FACEBOOK
using Facebook.Unity;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Server.Network.Leadboard
{
    /// <summary>
    /// Leadboard manager. Creates player icons on the leadboard
    /// </summary>
    public class LeadboardManager : MonoBehaviour
    {
        public LeadboardObject playerIconPrefab;
        public List<LeadboardObject> playerIconsList = new List<LeadboardObject>();
        public GameObject loginButton;
        public int level;

        void OnEnable()
        {
#if FACEBOOK
      
#if PLAYFAB || GAMESPARKS
            NetworkManager.friendsManager.GetLeadboardOnLevel();

            NetworkManager.THIS.leadboardList.Clear();
            StartCoroutine(WaitForLeadboard());
#elif EPSILON
            if(!string.IsNullOrEmpty(NetworkManager.THIS.UserID))
                loginButton.SetActive(false);
            StartCoroutine(WaitForLeadboard());
#endif
#endif
        }


        void OnDisable()
        {
#if PLAYFAB || GAMESPARKS || EPSILON
            //PlayFabManager.OnLevelLeadboardLoaded -= ShowLeadboard;
#endif
            ResetLeadboard();
        }

        void ResetLeadboard()
        {
            // transform.localPosition = new Vector3(0, -40f, 0);
        }
#if FACEBOOK
        
#if PLAYFAB || GAMESPARKS || EPSILON
        IEnumerator WaitForLeadboard()
        {
            yield return new WaitForSeconds(0.5f);
            // yield return new WaitUntil(() => FB.IsLoggedIn);
            // loginButton.SetActive(false);
            yield return new WaitUntil(() => FacebookManager.THIS.Friends.Count > 0);
            loginButton.SetActive(false);
            StartCoroutine(NetworkManager.friendsManager.GetLeadboardCor(level));
            yield return new WaitUntil(() => NetworkManager.THIS.leadboardList.Count > 0);
            GetComponent<Image>().enabled = true;
            ShowLeadboard();
        }

        void ShowLeadboard()
        {
            NetworkManager.THIS.leadboardList.Sort(CompareByScore);
            // Debug.Log("leadboard players count ===>> " + NetworkManager.THIS.leadboardList.Count);
#if EPSILON || PLAYFAB
            for (int j = 0; j < Mathf.Clamp(NetworkManager.THIS.leadboardList.Count, 0, 5); j++)
            {
                var item = NetworkManager.THIS.leadboardList[j];
                // if (item.score <= 0)
                    // continue;
                LeadboardObject gm = Instantiate (playerIconPrefab, transform, true);
                gm.transform.localScale = Vector3.one * 0.816f;
                gm.PlayerData = item;

                // Debug.Log("leadboard player data " + item);
                //			playerIconsList.Add (lo);
                //			gm.GetComponent<RectTransform> ().anchoredPosition = leftPosition + Vector2.right * (width * i);
            }
#elif GAMESPARKS || PLAYFAB
            for (int j = 0; j < NetworkManager.THIS.leadboardList.Count; j++)
            {
                var item = NetworkManager.THIS.leadboardList[j];
                if (item.score <= 0)
                    continue;
                //			GameObject gm = Instantiate (playerIconPrefab) as GameObject;
                LeadboardObject lo = playerIconsList[j];
                lo.gameObject.SetActive(true);
                item.position = j + 1;
                lo.PlayerData = item;
                Debug.Log("leadboard player data " + item);
                //			playerIconsList.Add (lo);
                //			gm.transform.SetParent (transform);
                //			gm.transform.localScale = Vector3.one;
                //			gm.GetComponent<RectTransform> ().anchoredPosition = leftPosition + Vector2.right * (width * i);
            }
#endif
        }


        private int CompareByScore(LeadboardPlayerData x, LeadboardPlayerData y)
        {
            int retval = y.score.CompareTo(x.score);

            if (retval != 0)
            {
                return retval;
            }

            return y.score.CompareTo(x.score);
        }
#endif
#endif
    }
}