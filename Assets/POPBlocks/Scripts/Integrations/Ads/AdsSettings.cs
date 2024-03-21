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
using Malee.List;
using UnityEngine;

namespace POPBlocks.Scripts.Integrations.Ads
{
    public class AdsSettings : ScriptableObject
    {
        [Reorderable(expandByDefault = true, elementNameProperty = "adNetwork.network")]
        public AdItems ads;

//         private void OnValidate()
//         {
// #if UNITY_EDITOR
//             foreach (var adItem in ads)
//             {
//                 if (adItem.enable)
//                     DefineSymbolsUtils.AddSymbol(GetAdSymbol(adItem.adNetwork.network));
//                 else
//                     DefineSymbolsUtils.DeleteSymbol(GetAdSymbol(adItem.adNetwork.network));
//             }
// #endif
//         }

        private string GetAdSymbol(AdNetworks adItemAdNetworks)
        {
            switch (adItemAdNetworks)
            {
                case AdNetworks.UnityAds:
                    return "UNITY_ADS";
                case AdNetworks.Admob:
                    return "ADMOB";
                default:
                    throw new ArgumentOutOfRangeException(nameof(adItemAdNetworks), adItemAdNetworks, null);
            }
        }
    }

    [Serializable]
    public class AdItems : ReorderableArray<AdItem>
    {
    }

    [Serializable]
    public class AdItem
    {
        public AdNetwork adNetwork;

        public bool enable;

        // [InspectorButton("OnButtonClicked", ButtonWidth = 100)] public bool settings;
        public bool showRewardedAds = true;
        [Reorderable] public AdsId adsId;

        [Reorderable(expandByDefault = true, elementNameProperty = "trigger")]
        public AdTriggers adTriggers;
    }

    [Serializable]
    public class AdTriggers : ReorderableArray<AdTrigger>
    {
    }

    [Serializable]
    public class AdsId : ReorderableArray<IDs>
    {
    }

    [Serializable]
    public class AdTrigger
    {
        public AdType type;
        [Header("Popup or event")] public GameState trigger;

        [Header("Show on (0 - disabled, 1 - show ad every time)"),
         Tooltip("An ad will be shown after an event will be trigger X times, if value 10 - means ad will be shown after event appear 10 times")]
        public int frequency = 1;
    }

    public enum AdType
    {
        Banner,
        Interstitial,
        RewardedAd
    }

    [Serializable]
    public class AdNetwork
    {
        public AdNetworks network;
    }

    public enum AdNetworks
    {
        UnityAds,
        Admob,
        Ironsource
    }

    public enum Platforms
    {
        Android,
        iOS
    }

    [Serializable]
    public class IDs
    {
        public AdType type;
        public Platforms platform;
        public string id;
    }
}