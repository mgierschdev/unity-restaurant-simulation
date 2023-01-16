using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

// Source: https://docs.unity.com/authentication/PlatformSignInGoogle.html
public static class UnityAuth
{
    private static bool AuthFailed = false;
    private static bool Retrying = false;

    // CloudCodeResult structure is the serialized response from the RollDice script in Cloud Code
    private class CloudCodeResult
    {
        public string key;
        public string value;
    }

    public static async void InitUnityServices()
    {
        GameLog.Log("Connecting ");

        if (Settings.DisableNetwork)
        {
            PlayerData.InitUser();
            return;
        }

        if (!Util.IsInternetReachable() || Retrying)
        {
            return;
        }

        try
        {
            Retrying = true;
            var options = new InitializationOptions();
            options.SetEnvironmentName(Settings.UnityServicesDev); //TODO: current unity services env development
            await UnityServices.InitializeAsync(options);
            // InitAnalytics();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await SignInAnonymouslyAsync();
            }

            // In case we could not connect or do auth
            if (!AuthenticationService.Instance.IsSignedIn || AuthFailed)
            {
                return;
            }

            CloudCodeResult response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>(CloudFunctions.CloudCodeGetPlayerData, null);

            Dictionary<string, object> parameters = new Dictionary<string, object>(){
            {AnalyticsEvents.CloudCodeGetPlayerDataResponse, (int) CloudCodeGetPlayerDataResponse.NEW_PLAYER_SAVED}
        };

            //UnityAnalytics.PublishEvent(AnalyticsEvents.CloudCodeGetPlayerData, parameters);

            GameLog.Log("Auth user id: " + AuthenticationService.Instance.PlayerId);
            GameLog.Log("CloudCodeGetPlayerData: " + response.key + " " + response.value);
            PlayerData.InitUser();

            AuthFailed = false;
            Retrying = false;

        }
        catch (Exception e)
        {
            GameLog.LogWarning(e.ToString());
            AuthFailed = true;
            Retrying = false;
        }
    }

    // public static async void InitAnalytics()
    // {
    //     try
    //     {
    //         List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
    //     }
    //     catch (ConsentCheckException e)
    //     {
    //         // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
    //     }
    // }

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
            AuthFailed = true;
            Retrying = false;
        }
    }

    // public static async Task SignInWithGoogleAsync(string idToken)
    // {
    //     try
    //     {
    //         await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
    //         // GameLog.Log("SignIn is successful.");
    //     }
    //     catch (AuthenticationException ex)
    //     {
    //         // Compare error code to AuthenticationErrorCodes
    //         // Notify the player with the proper error message
    //         GameLog.LogWarning(ex.ToString());
    //     }
    //     catch (RequestFailedException ex)
    //     {
    //         // Compare error code to CommonErrorCodes
    //         // Notify the player with the proper error message
    //         GameLog.LogWarning(ex.ToString());
    //     }
    // }

    // //In order to link an anon user to a google account
    // public static async Task LinkWithGoogleAsync(string idToken)
    // {
    //     try
    //     {
    //         await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
    //         GameLog.Log("Link is successful.");
    //     }
    //     catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
    //     {
    //         // Prompt the player with an error message.
    //         GameLog.Log("This user is already linked with another account. Log in instead.");
    //     }

    //     catch (AuthenticationException ex)
    //     {
    //         // Compare error code to AuthenticationErrorCodes
    //         // Notify the player with the proper error message
    //         GameLog.LogWarning(ex.ToString());
    //     }
    //     catch (RequestFailedException ex)
    //     {
    //         // Compare error code to CommonErrorCodes
    //         // Notify the player with the proper error message
    //         GameLog.LogWarning(ex.ToString());
    //     }
    // }

    private static bool AreUnityServicesLoaded()
    {
        return Settings.DisableNetwork || (
            !Util.IsInternetReachable() &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn &&
            !AuthFailed &&
            !Retrying);
    }
}