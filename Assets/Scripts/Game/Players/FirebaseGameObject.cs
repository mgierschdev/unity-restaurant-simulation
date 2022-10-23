using Firebase.Firestore;

[FirestoreData]
public class FirebaseGameObject
{
    [FirestoreProperty]
    public string Name {get; set;}
    [FirestoreProperty]
    public int[] Position {get; set;}
}