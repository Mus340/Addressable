using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class NameData : MonoBehaviour, Data
{
    private DatabaseReference _reference;
    private List<string> _nameList;
    
    public async Task Initialize(FirebaseDatabase reference)
    {
        _reference = reference.GetReference("Name");
        await Load();
    }

    private async Task Load()
    {
        var snapshot = await _reference.GetValueAsync();
        
        //TODO
        if (snapshot.Exists)
        {
            _nameList = new();
            foreach (var child in snapshot.Children)
            {
                _nameList.Add(child.Key);
            }
        }
    }
}
