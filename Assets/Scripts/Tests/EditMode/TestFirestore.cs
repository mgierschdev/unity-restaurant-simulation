using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using NUnit.Framework;
using UnityEngine;

//This test only passes if the firestore emulator suite is running locally and firestore running at 8080
public class TestFirestore
{
    private const string DEV_HOST = "localhost:8080";
    private const string TEST_COLLECTION = "Test";
    private FirebaseFirestore firestore;
    private MockUser user;

    [SetUp]
    public void SetUp()
    {
        user = new MockUser();
    }

    [Test]
    public void TestSavingData()
    {
        Dictionary<string, object> docData = new Dictionary<string, object>
        {
        { "stringExample", "Hello World" },
        { "booleanExample", false },
        { "numberExample", 3.14159265 },
        { "nullExample", null },
        { "arrayExample", new List<object>() { 5, true, "Hello" } },
        { "objectExample", new Dictionary<string, object>
            {
                { "a", 5 },
                { "b", true },
            }
        },};

        firestore = FirebaseFirestore.DefaultInstance;
        // firestore.Settings.Host is cached beteween tests 
        Debug.Log("Current firestore host " + firestore.Settings.Host);
        if (!firestore.Settings.Host.Contains(DEV_HOST))
        {
            firestore.Settings.Host = DEV_HOST;
            firestore.Settings.SslEnabled = false;
        }

        DocumentReference dataTypesReference = firestore.Collection(TEST_COLLECTION).Document("Datatypes");
        DocumentReference usersReference = firestore.Collection(TEST_COLLECTION).Document(user.ID);

        Task.Run(() => dataTypesReference.SetAsync(docData)).GetAwaiter();
        Task.Run(() => usersReference.SetAsync(user.GetUserAsMap())).GetAwaiter();

        DocumentSnapshot snapshot = null;
        Task.Run(async () =>
        {
            snapshot = await usersReference.GetSnapshotAsync();
            Debug.Log("snapshot1 ID: " + snapshot.Id);
            Assert.AreEqual(snapshot.Id, user.ID);
        }).GetAwaiter();

        usersReference.DeleteAsync().GetAwaiter();

        Task.Run(async () =>
        {
            snapshot = await usersReference.GetSnapshotAsync();
            Debug.Log("Assert false "+snapshot.Exists);
            Assert.False(snapshot.Exists);
        }).GetAwaiter();
    }
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