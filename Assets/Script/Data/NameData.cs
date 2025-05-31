using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UniRx;
using UnityEngine;

public class NameData : MonoBehaviour
{
    private DatabaseReference _reference;
    
    public void Initialize(FirebaseDatabase reference)
    {
        _reference = reference.GetReference("Name");
    }
    
    public async Task<bool> CheckNickNameAsync(string userName)
    {
        try
        {
            var snapshot = await _reference.Child(userName).GetValueAsync();
            return snapshot.Exists;
        }
        catch (Exception e)
        {
            Debug.LogError($"닉네임 중복 확인 중 오류: {e}");
            return false;
        }
    }

    public void Save(string userName)
    {
        _reference.Child(userName).SetValueAsync(true)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompletedSuccessfully)
                {
                    Debug.LogError($"저장 실패: {task.Exception}");
                }
            });
    }
}
