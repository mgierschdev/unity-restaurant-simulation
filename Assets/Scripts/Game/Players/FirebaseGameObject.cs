using Firebase.Firestore;

[FirestoreData]
public class FirebaseGameObject
{
    [FirestoreProperty]
    public int ID { get; set; }
    [FirestoreProperty]
    public int[] POSITION { get; set; }
    [FirestoreProperty]
    public bool IS_STORED { get; set; }
    [FirestoreProperty]
    public int ROTATION { get; set; }
}