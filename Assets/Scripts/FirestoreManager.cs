using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirestoreManager : MonoBehaviour
{
    private static FirestoreManager _instance;

    private FirebaseFirestore _db = null;
    private List<CloudAnchor> _cloudAnchors;

    public static FirestoreManager Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        _instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitCloudFirestore();
        _cloudAnchors = new List<CloudAnchor>();
    }


    private void InitCloudFirestore()
    {
#if UNITY_ANDROID
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _db = FirebaseFirestore.DefaultInstance;
            }
            else
            {
                Debug.LogError(string.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
#else
        _db = FirebaseFirestore.DefaultInstance;
#endif
    }


    public void AddCloudAnchor(string cloudAnchorId)
    {
        CollectionReference colRef = _db.Collection("anchors");
        CloudAnchor cloudAnchor = new CloudAnchor
        {
            CloudAnchorId = cloudAnchorId,
            CreateTime = DateTime.Now,
            ExpireTime = DateTime.Now.AddDays(1)
        };
        colRef.AddAsync(cloudAnchor).ContinueWithOnMainThread(task =>
        {
            DocumentReference addedDocRef = task.Result;
            Debug.Log($"[{nameof(FirestoreManager)}] {nameof(AddCloudAnchor)} added document Id: {addedDocRef.Id}");
        });
    }


    public void GetCloudAnchors(Action<List<CloudAnchor>> getCloudAnchorsAction)
    {
        if (_db == null)
        {
            Debug.Log($"[{nameof(FirestoreManager)}] {nameof(GetCloudAnchors)} Database is null");
            return;
        }

        _cloudAnchors.Clear();

        Query cloudAnchorQuery = _db.Collection("anchors");

        cloudAnchorQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot cloudAnchorQuerySnap = task.Result;

            foreach(DocumentSnapshot docSnap in cloudAnchorQuerySnap.Documents)
            {
                CloudAnchor cloudAnchor = docSnap.ConvertTo<CloudAnchor>();
                Debug.Log($"[{nameof(FirestoreManager)}] {nameof(GetCloudAnchors)} {cloudAnchor}");
                _cloudAnchors.Add(cloudAnchor);
            }

            getCloudAnchorsAction(_cloudAnchors);
        });
    }
}
