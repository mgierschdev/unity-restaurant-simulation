using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

// Source: https://docs.unity.com/authentication/PlatformSignInGoogle.html
public static class UnityAuth
{
    // CloudCodeResult structure is the serialized response from the RollDice script in Cloud Code
    private class CloudCodeResult
    {
        public string key;
        public string value;
    }

    public static async void InitUnityServices()
    {
        //TODO: current unity services env development
        var options = new InitializationOptions();
        options.SetEnvironmentName(Settings.UnityServicesDev);
        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await SignInAnonymouslyAsync();
        }

        CloudCodeResult response = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>(CloudFunctions.CloudCodeGetPlayerData, null);

        Dictionary<string, object> parameters = new Dictionary<string, object>(){
            {AnalyticsEvents.CloudCodeGetPlayerDataResponse, (int) CloudCodeGetPlayerDataResponse.NEW_PLAYER_SAVED}
        };

        UnityAnalytics.PublishEvent(AnalyticsEvents.CloudCodeGetPlayerData, parameters);
        GameLog.LogService("Auth user id: " + AuthenticationService.Instance.PlayerId);
        GameLog.LogService("CloudCodeGetPlayerData: " + response.key + " " + response.value);
    }

    public static async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            // Shows how to get the playerID
            // GameLog.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
    }

    public static async Task SignInWithGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
            // GameLog.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
    }

    //In order to link an anon user to a google account
    public static async Task LinkWithGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
            GameLog.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            // Prompt the player with an error message.
            GameLog.Log("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            GameLog.LogWarning(ex.ToString());
        }
    }

    public static bool IsUnityServiceInitialized()
    {
        return UnityServices.State == ServicesInitializationState.Initialized;
    }
}