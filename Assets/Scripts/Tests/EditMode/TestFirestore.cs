using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

public class TestFirestore
{
    private const string DEV_HOST = "localhost:8080";
    private const string TEST_COLLECTION = "TestUsers";
    private const string DOCUMENT = "i2RFKVtVdNRjnw6uvMLV";
    private Firestore test;


    [Test]
    public void TestInitFirebase()
    {
        test = new Firestore(DEV_HOST, TEST_COLLECTION, DOCUMENT);
        Init();
        Assert.IsTrue(test.GetIsFirebaseEnabled());
    }

    [Test]
    public void TestSavingData()
    {
        test = new Firestore(DEV_HOST, TEST_COLLECTION, DOCUMENT);

        Init();

        Dictionary<string, object> field = new Dictionary<string, object>{
        { "Field1", "1" },
        { "Field2", "2" },
        { "Feild3", "3" }};

        Debug.Log("Saving in dic ");
        Save(field);
        GameLog.Log("TestSavingData finished");
    }

    private async void Init()
    {
        await test.InitFirebase();
    }

    private async void Save(Dictionary<string, object> field)
    {
        await test.SaveDictionary(field);
    }
}