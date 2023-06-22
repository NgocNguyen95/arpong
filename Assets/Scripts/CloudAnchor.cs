using Firebase.Firestore;
using System;

[FirestoreData]
public class CloudAnchor
{
    [FirestoreProperty]
    public string CloudAnchorId { get; set; }

    [FirestoreProperty]
    public DateTime CreateTime { get; set; }

    [FirestoreProperty]
    public DateTime ExpireTime { get; set; }
}
