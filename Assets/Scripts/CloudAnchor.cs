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

    public override string ToString()
    {
        string str = string.Format("{0}: {1}, {2}: {3}, {4}: {5}",
            nameof(CloudAnchorId), CloudAnchorId,
            nameof(CreateTime), CreateTime.ToString(),
            nameof(ExpireTime), ExpireTime.ToString());

        return str;
    }
}
