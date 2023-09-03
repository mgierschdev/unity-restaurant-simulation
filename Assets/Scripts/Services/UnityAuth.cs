using System;
using System.Threading.Tasks;
using Game.Players;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Util;

// using Services.Util;
// using Unity.Services.CloudCode;
// using Unity.Services.Core.Environments;
// using System.Collections.Generic;

namespace Services
{
    public static class UnityAuth
    {
        private static bool _authFailed;
        private static bool _retrying;

        public static void InitUnityServices()
        {
            GameLog.Log("Connecting ");

            if (Settings.DisableNetwork)
            {
                PlayerData.InitUser();
            }

            // if (!Util.IsInternetReachable() || _retrying)
            // {
            //     return;
            // }
            //
            // try
            // {
            //     _retrying = true;
            //     var options = new InitializationOptions();
            //     options.SetEnvironmentName(Settings.UnityServicesDev);
            //     await UnityServices.InitializeAsync(options);
            //     // InitAnalytics();
            //
            //     if (!AuthenticationService.Instance.IsSignedIn)
            //     {
            //         await SignInAnonymouslyAsync();
            //     }
            //
            //     // In case we could not connect or do auth
            //     if (!AuthenticationService.Instance.IsSignedIn || _authFailed)
            //     {
            //         return;
            //     }
            //
            //     CloudCodeResult response =
            //         await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>(
            //             CloudFunctions.CloudCodeGetPlayerData, null);
            //
            //     Dictionary<string, object> parameters = new Dictionary<string, object>()
            //     {
            //         { AnalyticsEvents.CloudCodeGetPlayerDataResponse, (int)CloudCodeGetPlayerDataResponse.NewPlayerSaved }
            //     };
            //
            //     //UnityAnalytics.PublishEvent(AnalyticsEvents.CloudCodeGetPlayerData, parameters);
            //
            //     GameLog.Log("Auth user id: " + AuthenticationService.Instance.PlayerId);
            //     GameLog.Log("CloudCodeGetPlayerData: " + response.Key + " " + response.Value);
            //     PlayerData.InitUser();
            //
            //     _authFailed = false;
            //     _retrying = false;
            // }
            // catch (Exception e)
            // {
            //     GameLog.LogWarning(e.ToString());
            //     _authFailed = true;
            //     _retrying = false;
            // }
        }

        private static async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                GameLog.Log(e.ToString());
                _authFailed = true;
                _retrying = false;
            }
        }

        private static bool AreUnityServicesLoaded()
        {
            return Settings.DisableNetwork || (
                //!Util.IsInternetReachable() &&
                UnityServices.State == ServicesInitializationState.Initialized &&
                AuthenticationService.Instance.IsSignedIn &&
                !_authFailed &&
                !_retrying);
        }
    }
}