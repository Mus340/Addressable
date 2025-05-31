using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

public class MainData : MonoBehaviour
{
    public FirebaseDatabase Reference { get; private set; }
    public UserData UserData { get; private set; }
    public RankData RankData { get; private set; }
    public NameData NameData { get; private set; }

    private void Awake()
    {
        UserData = FindObjectOfType<UserData>();
        RankData = FindObjectOfType<RankData>();
        NameData = FindObjectOfType<NameData>();
    }

    public async Task Initialize()
    {
        Reference = FirebaseDatabase.DefaultInstance;
        
        await UserData.Initialize(Reference);
        await RankData.Initialize(Reference);
    }
}
