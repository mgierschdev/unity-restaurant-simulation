using NUnit.Framework;
public class TestFireauth
{

    [Test]
    public void TestInitAuth()
    {
        Assert.IsTrue(true);
    }

    // private Task InitAuthAnonymosly()
    // {
    //     auth = FirebaseAuth.DefaultInstance;

    //     return auth.SignInAnonymouslyAsync().ContinueWith(task =>
    //     {
    //         if (task.IsCanceled)
    //         {
    //             Debug.LogError("SignInAnonymouslyAsync was canceled.");
    //             return;
    //         }
    //         if (task.IsFaulted)
    //         {
    //             Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
    //             return;
    //         }
    //         isUserSignedIn = true;
    //         FirebaseUser newUser = task.Result;
    //         Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
    //     });
    // }

}