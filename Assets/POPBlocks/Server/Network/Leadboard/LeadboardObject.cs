using System.Collections;
using System.Collections.Generic;
#if EPSILON
using EpsilonServer.EpsilonClientAPI.EpsilonLives;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace POPBlocks.Server.Network.Leadboard
{
    /// <summary>
    /// Player icon with score and name on the leadboard
    /// </summary>
    public class LeadboardObject : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI place;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI score;
#if PLAYFAB || GAMESPARKS || EPSILON
        private LeadboardPlayerData playerData;

        public List<string> sendLifeList = new List<string>();
        
        public LeadboardPlayerData PlayerData
        {
            get { return playerData; }

            set
            {
                playerData = value;
                SetupIcon();
            }
        }


        void SetupIcon()
        {
            StartCoroutine(WaitForPicture());
        }

        IEnumerator WaitForPicture()
        {
            print("wait for picture");
            yield return new WaitUntil(() => PlayerData != null);
            yield return new WaitUntil(() => PlayerData.friendData != null);
            transform.GetChild(0).gameObject.SetActive(true);
            #if FACEBOOK
#if GAMESPARKS || PLAYFAB
			if (PlayerData.friendData.picture == null) {
				FacebookManager.THIS.LoggedSuccefull ();
				FacebookManager.THIS.GetFriendsPicture ();
			}
			yield return new WaitUntil (() => PlayerData.friendData.picture != null);
			PlayerData.picture = PlayerData.friendData.picture;
			icon.sprite = PlayerData.picture;
			place.text = "" + PlayerData.position;
			playerName.text = PlayerData.Name;
			score.text = "" + PlayerData.score;
			if (NetworkManager.THIS.IsYou (PlayerData.userID)) {
				playerName.text = "YOU";
				playerName.color = Color.red;
				//if (LevelManager.This.gameStatus == GameState.Win) {
				//    score.text = "" + PlayerPrefs.GetInt("Score" + LevelManager.This.currentLevel);
				//   }
			}

#elif EPSILON
            if (PlayerData.friendData.pictureSprite == null)
            {
                FacebookManager.THIS.LoggedSuccefull();
                FacebookManager.THIS.GetFriendsPicture();
            }

            yield return new WaitUntil(() => PlayerData.friendData.pictureSprite != null);
            PlayerData.picture = PlayerData.friendData.pictureSprite;
            icon.sprite = PlayerData.picture;
            place.text = "" + PlayerData.position;
            playerName.text = PlayerData.Name;
            score.text = "" + PlayerData.score;
            if (NetworkManager.THIS.IsYou(PlayerData.userID))
            {
                playerName.text = "YOU";
                playerName.color = new Color(255f/255, 202f/255, 76f/255);
                //if (LevelManager.This.gameStatus == GameState.Win) {
                //    score.text = "" + PlayerPrefs.GetInt("Score" + LevelManager.This.currentLevel);
                //   }
            }
#endif
#endif
        }

        public void SendLifeToFriend()
        {
	        #if EPSILON
	        sendLifeList.Add(PlayerData.friendData.id);
	        
	        EpsilonLifesManager.SendLifeToFriend(sendLifeList);
	        #endif
	        
	        // clear everytime after send life
	        sendLifeList.Clear();
        }

#endif
    }
}