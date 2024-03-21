﻿//#if PLAYFAB || GAMESPARKS || EPSILON
using System;
using System.Collections.Generic;
#if PLAYFAB
using PlayFab;
using POPBlocks.Server.Network.PlayFabIntegration;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
#if EPSILON
using EpsilonServer;
#endif

#if GAMESPARKS
#endif
#if PLAYFAB

#endif
namespace POPBlocks.Server.Network
{

//using PlayFab.AdminModels;
	/// <summary>
	/// Network manager. Initialization, Authorization processing
	/// </summary>
	public class NetworkManager : MonoBehaviour
	{
		public delegate void NetworkEvents();

		public static event NetworkEvents OnLoginEvent; 
		public static event NetworkEvents OnLogoutEvent;
		public static event NetworkEvents OnFriendsOnMapLoaded;
		public static event NetworkEvents OnPlayerPictureLoaded;
		public static event NetworkEvents OnLevelLeadboardLoaded;
		public static NetworkManager THIS;
		//Facebook profile picture
		public static Sprite profilePic;
		public static ILoginManager loginManger;
		private bool isLoggedIn;

		public bool IsLoggedIn
		{
			get
			{
				return isLoggedIn;
			}

			set
			{
				isLoggedIn = value;
				if (value && OnLoginEvent != null)
					OnLoginEvent();
				else if (!value && OnLogoutEvent != null)
					OnLogoutEvent();
			}
		}
	
		// Use this for initialization
		void Awake()
		{
			if(THIS != null) Destroy(gameObject);
			else
				THIS = this;
			// DontDestroyOnLoad(this);
#if PLAYFAB || GAMESPARKS || EPSILON

			//#if ((UNITY_PS4 || UNITY_XBOXONE) && !UNITY_EDITOR) || GS_FORCE_NATIVE_PLATFORM
#if GS_FORCE_NATIVE_PLATFORM
this.gameObject.AddComponent<NativePlatform>();
#elif UNITY_WEBGL && !UNITY_EDITOR
// this.gameObject.AddComponent<WebGLPlatform>();
// #elif !PLAYFAB
// 		this.gameObject.AddComponent<DefaultPlatform> ();//2.1.4
#endif
#if PLAYFAB
			if(PlayFabSettings.TitleId == "")
				PlayFabSettings.TitleId = titleId;
			loginManger = new PlayFabManager ();
#elif GAMESPARKS
			//		new GamesparksConfiguration (this);
			loginManger = new GamesparksLogin();
#elif EPSILON
			loginManger = new EpsilonManager();
#endif
			//PlayFabSettings.DeveloperSecretKey = DeveloperSecretKey;
			currencyManager = new NetworkCurrencyManager();
			friendsManager = new NetworkFriendsManager();
			dataManager = new NetworkDataManager();

			//Login(titleId);
#endif
		}

#if PLAYFAB || GAMESPARKS || EPSILON

		public static NetworkCurrencyManager currencyManager;
		public static NetworkDataManager dataManager;
		public static NetworkFriendsManager friendsManager;
		[SerializeField]
		private string userID;

		public string UserID
		{
			get
			{
				return userID;
			}
			set
			{
				if (value != PlayerPrefs.GetString("UserID") && PlayerPrefs.GetString("UserID") != "" && userID != "" && userID != null)
				{
					PlayerPrefs.DeleteAll();
					// LevelsMap._instance.Reset();
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				userID = value;
				PlayerPrefs.SetString("UserID", userID);
				PlayerPrefs.Save();
			}
		}

		public string titleId;
		//public string DeveloperSecretKey;

		public List<LeadboardPlayerData> leadboardList = new List<LeadboardPlayerData>();
		public string facebookUserID;

		#region AUTHORIZATION

		public void LoginWithFB(string accessToken)
		{
			loginManger.LoginWithFB(accessToken, titleId);
		}

		public void UpdateName(string userName)
		{
			loginManger.UpdateName(userName);
		}

		public bool IsYou(string playFabId)
		{
			return loginManger.IsYou(playFabId);
		}

		#endregion

		#region EVENTS

		public static void LevelLeadboardLoaded()
		{
			OnLevelLeadboardLoaded?.Invoke();
		}

		public static void PlayerPictureLoaded()
		{
			OnPlayerPictureLoaded?.Invoke();
		}

		public static void FriendsOnMapLoaded()
		{
			OnFriendsOnMapLoaded?.Invoke();
		}

		#endregion

#endif

	}

	[Serializable]
	public class LeadboardPlayerData
	{
		public string Name;
		public string userID;
		public int position;
		public int score;
		public Sprite picture;
		public FriendData friendData;
	}
}