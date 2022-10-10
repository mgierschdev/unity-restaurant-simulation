using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestFirestore
{
    private const string DEV_HOST = "localhost:8080";
    private const string TEST_COLLECTION = "TestUsers";
    private const string DOCUMENT = "i2RFKVtVdNRjnw6uvMLV";
    private Firestore test;


    [Test]
    public async void TestInitFirebase()
    {
        test = new Firestore(DEV_HOST, TEST_COLLECTION, DOCUMENT);
        await test.InitFirebase();
        Assert.IsTrue(test.GetIsFirebaseEnabled());
    }

    [Test]
    public void TestSavingData()
    {
        test = new Firestore(DEV_HOST, TEST_COLLECTION, DOCUMENT);
        FirebaseQueue queue = new FirebaseQueue();

        Dictionary<string, object> field = new Dictionary<string, object>{
        { "Field1", "1" },
        { "Field2", "2" },
        { "Feild3", "3" }};

        test.InitFirebase();
        
    }
}