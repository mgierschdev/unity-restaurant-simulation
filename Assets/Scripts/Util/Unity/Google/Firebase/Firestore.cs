// Problem: Provide Firestore access helpers and test operations.
// Goal: Initialize Firestore and perform read/write operations.
// Approach: Wrap FirebaseFirestore APIs for common actions.
// Time: O(1) per call plus network time.
// Space: O(1).
// using System.Threading.Tasks;
// using Firebase;
// using Firebase.Firestore;
//
// public static class Firestore
// {
//     private static FirebaseFirestore firestore;
//     private static FirebaseApp app;
//
//     public static void Init(bool editor)
//     {
//         firestore = FirebaseFirestore.DefaultInstance;
//
//         if (editor)
//         {
//             // In case the config has been cached between scene loads
//             if (!firestore.Settings.Host.Contains(Settings.FIRESTORE_HOST))
//             {
//                 firestore.Settings.Host = Settings.FIRESTORE_HOST;
//                 firestore.Settings.SslEnabled = false;
//             }
//         }
//     }
//
//     // Only during the first login
//     public static Task<DocumentSnapshot> GetUserData(string UID)
//     {
//         DocumentReference userData = null;
//
//         if (Settings.IsFirebaseEmulatorEnabled)
//         {
//             userData = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION).Document(UID);
//         }
//         else
//         {
//             userData = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION).Document(UID);
//         }
//
//         return userData.GetSnapshotAsync() ?? null;
//     }
//
//     public static Task SaveUser()
//     {
//         if (PlayerData.GetFirebaseGameUser() == null)
//         {
//             return null;
//         }
//
//         DocumentReference testUser = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION)
//             ?.Document(PlayerData.GetFirebaseGameUser().FIREBASE_AUTH_ID);
//         return testUser.SetAsync(PlayerData.GetFirebaseGameUser(), SetOptions.MergeAll);
//     }
// }
