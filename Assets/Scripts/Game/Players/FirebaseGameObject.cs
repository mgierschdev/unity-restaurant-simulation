using Firebase.Firestore;

[FirestoreData]
public class FirebaseGameObject
{
    [FirestoreProperty]
    public int ID { get; set; } //StoreItemType
    [FirestoreProperty]
    public int[] POSITION { get; set; }
    [FirestoreProperty]
    public bool IS_STORED { get; set; }
    [FirestoreProperty]
    public int ROTATION { get; set; }
    [FirestoreProperty]
    public int ID_TOP_ITEM { get; set; } //StoreItemType, top container item type

    public override string ToString()
    {
        return ID + "-" + POSITION + "-" + IS_STORED + "-" + ROTATION+"-"+ID_TOP_ITEM;
    }
}