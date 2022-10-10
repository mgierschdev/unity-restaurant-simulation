using System.Threading.Tasks;  // Needed for the Unwrap extension method
// using Firebase.Auth;
// using Firebase.Firestore;
using NUnit.Framework;
using UnityEngine;
// using DependencyStatus = Firebase.DependencyStatus;


public class TestFireBase
{
    // private Firebase.LogLevel logLevel = Firebase.LogLevel.Info;
    // private FirebaseFirestore db;
    // private const string DEV_HOST = "localhost:8080";
    // private const string TEST_COLLECTION = "TestGameData";
    // private FirebaseAuth auth;
    // private bool isFirebaseInit;
    // private bool isUserSignedIn;

    // [Test]
    // public async void TestInitFireBase()
    // {
    //     //Init to check if the app has all the dependencies to use Firebase
    //     await InitFireBase();
    //     //Init Auth then present other options without friction
    //     await InitAuthAnonymosly();
    //     Assert.IsTrue(isFirebaseInit);
    //     // FirestoreInit();
    //     // Assert.NotNull(firestore);
    //     // // Other tests dev
    //     // Task<Query> task = firestore.GetNamedQueryAsync("");
    //     // Debug.Log(firestore.App.Name);
    //     // CollectionReference collectionTest = firestore.Collection(TEST_COLLECTION);
    //     // DocumentReference documentReference = collectionTest.Document();
    //     // Debug.Log(documentReference.ToString());
    // }


    // private void FirestoreInit()
    // {
    //     FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
    //     firestore.Settings.Host = DEV_HOST;
    //     firestore.Settings.SslEnabled = false;
    // }

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

    // private Task InitFireBase()
    // {
    //     return Firebase.FirebaseApp.CheckDependenciesAsync().ContinueWith(checkTask =>
    //      {
    //          DependencyStatus status = checkTask.Result;
    //          if (status != DependencyStatus.Available)
    //          {
    //              return Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(t =>
    //              {
    //                  return Firebase.FirebaseApp.CheckDependenciesAsync();
    //              }).Unwrap();
    //          }
    //          else
    //          {
    //              return checkTask;
    //          }
    //      }).Unwrap().ContinueWith(task =>
    //      {
    //          DependencyStatus dependencyStatus = task.Result;
    //          if (dependencyStatus == DependencyStatus.Available)
    //          {
    //              Debug.Log("1.- Firebase can be used");
    //              isFirebaseInit = true;
    //          }
    //          else
    //          {
    //              Debug.LogError(
    //                "Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
    //              isFirebaseInit = false;
    //          }
    //      });
    // }
}