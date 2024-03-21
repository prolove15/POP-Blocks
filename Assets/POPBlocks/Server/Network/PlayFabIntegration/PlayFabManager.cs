#if PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

#if PLAYFAB
#endif

//using PlayFab.AdminModels;

namespace POPBlocks.Server.Network.PlayFabIntegration
{
	public class PlayFabManager : ILoginManager {
		public string PlayFabId;



		// Use this for initialization

		#region AUTHORIZATION

		public void LoginWithFB (string accessToken, string titleId) {
			LoginWithFacebookRequest request = new LoginWithFacebookRequest () {
				TitleId = titleId,
				CreateAccount = true,
				AccessToken = accessToken
				//  CustomId = SystemInfo.deviceUniqueIdentifier
			};

			PlayFabClientAPI.LoginWithFacebook (request, (result) => {
					PlayFabId = result.PlayFabId;
					Debug.Log ("Got PlayFabID: " + PlayFabId);
					NetworkManager.THIS.UserID = PlayFabId; //TODO: think about login lambda
					if (result.NewlyCreated) {
						Debug.Log ("(new account)");
					} else {
						Debug.Log ("(existing account)");
					}
					NetworkManager.THIS.IsLoggedIn = true;
				},
				(error) => {
					Debug.Log (error.ErrorMessage);
				});
		}


		void Login (string titleId) {
			LoginWithCustomIDRequest request = new LoginWithCustomIDRequest () {
				TitleId = titleId,
				CreateAccount = true,
				CustomId = SystemInfo.deviceUniqueIdentifier
			};

			PlayFabClientAPI.LoginWithCustomID (request, (result) => {
					PlayFabId = result.PlayFabId;
					Debug.Log ("Got PlayFabID: " + PlayFabId);

					if (result.NewlyCreated) {
						Debug.Log ("(new account)");
					} else {
						Debug.Log ("(existing account)");
					}
					NetworkManager.THIS.IsLoggedIn = true;
				},
				(error) => {
					Debug.Log (error.ErrorMessage);
				});
		}

		public void UpdateName (string userName) {
			PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest request = new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest () {
				DisplayName = userName
			};

			PlayFabClientAPI.UpdateUserTitleDisplayName (request, (result) => {
				},
				(error) => {
					Debug.Log (error.ErrorMessage);
				});

		}

		public bool IsYou (string playFabId) {
			if (playFabId == PlayFabId)
				return true;
			return false;
		}


		#endregion

	}
}

#endif