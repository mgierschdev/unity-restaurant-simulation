using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

// Source: https://docs.unity.com/authentication/PlatformSignInGoogle.html

public static class UnityAuth
{
    public static async void InitUnityServices()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymouslyAsync();
        GameLog.Log("Init Unity services state " + UnityServices.State);
    }

    public static async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            GameLog.Log("Sign in anonymously succeeded!");
            // Shows how to get the playerID
            GameLog.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

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
            // GameLog.LogWarning(ex.ToString());
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // GameLog.LogWarning(ex.ToString());
        }
    }

    //In order to link an anon user to a google account
    public static async Task LinkWithGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
            // Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            // Prompt the player with an error message.
            // Debug.LogError("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
        }
    }

    public static bool IsUnityServiceInitialized()
    {
        return UnityServices.State == ServicesInitializationState.Initialized;
    }
}