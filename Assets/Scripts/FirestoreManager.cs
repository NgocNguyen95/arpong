using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using UnityEngine;

public class FirestoreManager : MonoBehaviour
{
    private static FirestoreManager _instance;

    private FirebaseFirestore _db = null;


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
}
