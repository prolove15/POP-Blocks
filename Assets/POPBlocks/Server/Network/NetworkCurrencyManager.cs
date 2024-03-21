using UnityEngine;
#if EPSILON
using EpsilonServer;
#endif

#if PLAYFAB || GAMESPARKS || EPSILON
#if GAMESPARKS
#endif
#if GAMESPARKS
#endif

#if PLAYFAB
using POPBlocks.Server.Network.PlayFabIntegration;
using UnityEngine;

#endif

namespace POPBlocks.Server.Network
{
    /// <summary>
    /// Currency manager
    /// </summary>
    public class NetworkCurrencyManager
    {
        public static int currentBalance;
        ICurrencyManager currencyMananager;

        public NetworkCurrencyManager()
        {
            NetworkManager.OnLoginEvent += GetBalance;
            NetworkManager.OnLogoutEvent += Logout;
#if PLAYFAB
		currencyMananager = new PlayFabCurrencyManager ();
#elif GAMESPARKS
			currencyMananager = new GamesparksCurrencyManager ();
#elif EPSILON
            currencyMananager = new EpsilonCurrencyManager();
#endif
        }

        void Logout()
        {
            NetworkManager.OnLoginEvent -= GetBalance;
            NetworkManager.OnLogoutEvent -= Logout;
        }

        /// <summary>
        /// Increment the balance
        /// </summary>
        /// <param name="amount"></param>
        public void IncBalance(int amount)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;


            if (currencyMananager != null)
                currencyMananager.IncBalance(amount);
        }

        /// <summary>
        /// Decrement the balance
        /// </summary>
        /// <param name="amount"></param>
        public void DecBalance(int amount)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;


            if (currencyMananager != null)
                currencyMananager.DecBalance(amount);
        }

        /// <summary>
        /// define the balance
        /// </summary>
        /// <param name="newbalance"></param>
        public void SetBalance(int newbalance)
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

            //		GetBalance ();

            if (currencyMananager != null)
                currencyMananager.SetBalance(newbalance);
        }

        /// <summary>
        /// Get the balance
        /// </summary>
        public void GetBalance()
        {
            if (!NetworkManager.THIS.IsLoggedIn)
                return;

			if (currencyMananager != null) {
				currencyMananager.GetBalance (balance => {
					Debug.Log (balance);
					currentBalance = balance;
					if (currentBalance >= PlayerPrefs.GetInt("Gems"))
						OnResetBalance?.Invoke(balance);
					else
						SetBalance (PlayerPrefs.GetInt("Gems"));
				
				});

			}
		}

		public delegate void CurrencyEvents(int count);

		public event CurrencyEvents OnResetBalance;

	}
}

#endif