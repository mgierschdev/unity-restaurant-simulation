using System.Collections.Generic;
using Firebase.Firestore;
using NUnit.Framework;
using UnityEngine;

public class TestFirestore
{
    private const string DEV_HOST = "localhost:8080";
    private const string TEST_COLLECTION = "TestUsers";
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
        await firestore.Collection(TEST_COLLECTION).Document("User").SetAsync(docData);
    }

    [Test]
    public async void TestSavingMockUser()
    {
        FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
        Debug.Log(user.ID);
        firestore.Settings.Host = DEV_HOST;
        firestore.Settings.SslEnabled = false;
        await firestore.Collection(TEST_COLLECTION).Document(user.ID).SetAsync(user.GetUserAsMap());
    }

    // [Test]
    // public async void TestReadMockUser()
    // {

    // }
}