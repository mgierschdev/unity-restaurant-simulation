using System.Collections.Generic;
using Firebase.Firestore;
using NUnit.Framework;
using UnityEngine;

//This test only passes if the firestore emulator suite is running locally and firestore running at 8080
public class TestFirestore
{
    private const string DEV_HOST = "localhost:8080";
    private const string TEST_COLLECTION = "Test";
    private Firestore test;
    private MockUser user;

    [SetUp]
    public void SetUp()
    {
        user = new MockUser();
    }

    [Test]
    public async void TestSavingData()
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

        FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
        firestore.Settings.Host = DEV_HOST;
        firestore.Settings.SslEnabled = false;

        DocumentReference dataTypesReference = firestore.Collection(TEST_COLLECTION).Document("Datatypes");
        DocumentReference usersReference = firestore.Collection(TEST_COLLECTION).Document(user.ID);

        await dataTypesReference.SetAsync(docData);
        await usersReference.SetAsync(user.GetUserAsMap());

        DocumentSnapshot snapshot = await usersReference.GetSnapshotAsync();
        Debug.Log("snapshot 1 "+snapshot.Id);
        if (snapshot.Exists)
        {
            Assert.AreEqual(snapshot.Id, user.ID);
        }
        else
        {
            throw new FirestoreException(FirestoreError.NotFound);//not found
        }

        // await usersReference.DeleteAsync();
        // await dataTypesReference.DeleteAsync();
        // DocumentSnapshot snapshot2 = await usersReference.GetSnapshotAsync();
        // Assert.False(snapshot2.Exists);
        // DocumentSnapshot snapshot3 = await dataTypesReference.GetSnapshotAsync();
        // Assert.False(snapshot3.Exists);
    }
}