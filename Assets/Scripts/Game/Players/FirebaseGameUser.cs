using System;
using Firebase.Firestore;

[FirestoreData]
public class FirebaseGameUser
{

    [FirestoreProperty]
    public string NAME { get; set; }
    [FirestoreProperty]
    public Double GAME_MONEY { get; set; }
    [FirestoreProperty]
    public Double GEMS { get; set; }
    [FirestoreProperty]
    public Double EXPERIENCE { get; set; }
    [FirestoreProperty]
    public int LEVEL { get; set; }
    [FirestoreProperty]
    public string LANGUAGE_CODE { get; set; }
    [FirestoreProperty]
    public string INTERNAL_ID { get; set; }
    [FirestoreProperty]
    public string FIREBASE_AUTH_ID { get; set; }
    [FirestoreProperty]
    public string EMAIL { get; set; }
    [FirestoreProperty]
    public int AUTH_TYPE { get; set; }
    [FirestoreProperty]
    public object LAST_LOGIN { get; set; }
    [FirestoreProperty]
    public object CREATED_AT { get; set; }
    // [FirestoreProperty]
    // public List<FirebaseGameObject> list;s
}