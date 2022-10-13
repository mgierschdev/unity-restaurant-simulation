using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Functions;
using NUnit.Framework;
using UnityEngine;

//This test only passes if the firestore emulator suite is running locally and firestore running at 8080
public class TestFirebase
{
    private FirebaseFirestore firestore;
    private PlayerData user;

    [SetUp]
    public void SetUp()
    {
        user = new PlayerData();
        user.SetMockUpUser();
    }

    [Test]
    public async void TestFirestoreGetDeletePost()
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
        if (!firestore.Settings.Host.Contains(Settings.FIRESTORE_HOST))
        {
            firestore.Settings.Host = Settings.FIRESTORE_HOST;
            firestore.Settings.SslEnabled = false;
        }

        // The ?.Document , ? symbol ensures that you cannot create another reference to a collection that already exists
        DocumentReference dataTypesReference = firestore.Collection(Settings.USER_COLLECTION)?.Document("Datatypes");
        DocumentReference usersReference = firestore.Collection(Settings.USER_COLLECTION)?.Document(user.EmailID);
        DocumentReference testUser = firestore.Collection(Settings.USER_COLLECTION)?.Document(Settings.TEST_USER);

        // SetOptions.MergeAll: allows Changes in the behavior of SetAsync calls to only replace the values specified in its documentData argument.
        // Docs: https://firebase.google.com/docs/reference/unity/class/firebase/firestore/set-options
        await dataTypesReference.SetAsync(docData, SetOptions.MergeAll);
        await usersReference.SetAsync(user.GetNewMockUserAsMap(), SetOptions.MergeAll);
        await testUser.SetAsync(user.GetNewMockUserAsMap(), SetOptions.MergeAll);

        DocumentSnapshot snapshot = await usersReference.GetSnapshotAsync();
        snapshot = await usersReference.GetSnapshotAsync();
        Debug.Log("snapshot1 ID: " + snapshot.Id);
        Assert.AreEqual(snapshot.Id, user.EmailID);

        // DocumentSnapshot snapshot = null;
        // Task.Run(async () =>
        // {
        //     snapshot = await usersReference.GetSnapshotAsync();
        //     Debug.Log("snapshot1 ID: " + snapshot.Id);
        //     Assert.AreEqual(snapshot.Id, user.EmailID);
        // }).GetAwaiter();

        // To clean up, Disabled during development 
        // usersReference.DeleteAsync().GetAwaiter();
        // Task.Run(async () =>
        // {
        //     snapshot = await usersReference.GetSnapshotAsync();
        //     Debug.Log("Assert false " + snapshot.Exists);
        //     Assert.False(snapshot.Exists);
        // }).GetAwaiter();
    }

    [Test]
    public void TestCloudFunction()
    {
        FirebaseFunctions functions = FirebaseFunctions.GetInstance(Settings.CLOUD_FUNCTION_HOST);
        HttpsCallableReference function = functions.GetHttpsCallable("helloWorld");
        string functionInput = "functionInput";

        function.CallAsync(functionInput).ContinueWithOnMainThread((response) =>
       {
           Debug.Log("response = " + response.Result.Data.ToString());

           if (response.IsFaulted || response.IsCanceled)
           {
               Firebase.FirebaseException e = response.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
               FunctionsErrorCode error = (FunctionsErrorCode)e.ErrorCode;

               Debug.LogError("Fault!");
               Debug.Log("FunctionsErrorCode! = " + error);
           }
           else
           {
            //    string returnedName = response.Result.Data.ToString();
            //    if (returnedName == functionInput)
            //    {
            //        //Name already exists in database
            //    }
            //    else if (string.IsNullOrEmpty(returnedName))
            //    {
            //        //Name doesn't exist in database
            //    }
           }
       });
    }

    // Waiting for future SDK support
    // [Test]
    // public void TestFiresbaseAuth()
    // {
    //     // //app.Options.DatabaseUrl = new System.Uri("localhost:9099");
    //     // FirebaseAuth auth = FirebaseAuth.GE
    //     // // Debug.Log(" "+auth.SignInWithCredentialAsync());
    //     // Task.Run(() => auth.SignInAnonymouslyAsync());
    //     // Debug.Log(auth.CurrentUser);
    // }
}
