using System.Collections.Generic;
using System.Threading.Tasks;  // Needed for the Unwrap extension method
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using DependencyStatus = Firebase.DependencyStatus;

public class Firestore
{
    private bool isFirebaseEnabled;
    private Firebase.LogLevel logLevel = Firebase.LogLevel.Info;
    private FirebaseFirestore firestore;
    private DocumentReference docReference;
    private string collectionName;
    private string document;
    private AppOptions appOptions;
    private FirebaseApp app;

    public Firestore()
    {
        firestore = FirebaseFirestore.DefaultInstance;
        firestore.Settings.SslEnabled = true;
        InitFirebase();
    }

    public Firestore(string devHost, string collectionName, string document)
    {
        firestore = FirebaseFirestore.DefaultInstance;
        firestore.Settings.SslEnabled = true;
        firestore.Settings.Host = devHost;
        this.collectionName = collectionName;
        this.document = document;
        docReference = firestore.Collection(collectionName).Document(document);
    }

    public Task SaveDictionary(Dictionary<string, object> dictionary)
    {
        Debug.Log("Saving dic ");

        return docReference.SetAsync(dictionary).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Data added");
        });
    }

    public Task InitFirebase()
    {
        return FirebaseApp.CheckDependenciesAsync().ContinueWith(checkTask =>
         {
             DependencyStatus status = checkTask.Result;
             if (status != DependencyStatus.Available)
             {
                 return FirebaseApp.FixDependenciesAsync().ContinueWith(t =>
                 {
                     return FirebaseApp.CheckDependenciesAsync();
                 }).Unwrap();
             }
             else
             {
                 return checkTask;
             }
         }).Unwrap().ContinueWith(task =>
         {
             DependencyStatus dependencyStatus = task.Result;
             if (dependencyStatus == DependencyStatus.Available)
             {
                 GameLog.Log("Firebase is loaded");
                 isFirebaseEnabled = true;
             }
             else
             {
                 GameLog.LogError("Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
                 isFirebaseEnabled = false;
             }
         });
    }

    public bool GetIsFirebaseEnabled()
    {
        return isFirebaseEnabled;
    }
}