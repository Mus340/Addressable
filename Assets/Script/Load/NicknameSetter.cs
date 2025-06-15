using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class NicknameSetter : MonoBehaviour
{
    public TextAsset profanityFile;
    private string[] lines;
    private string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";   
    private TaskCompletionSource<bool> _nicknameEntered;

    public async Task OpenNickName()
    {
        if (profanityFile != null)
        {
            lines = Regex.Split(profanityFile.text, LINE_SPLIT_RE);
        }
        else
        {
            Debug.LogWarning("비속어 파일이 연결되지 않았습니다.");
            lines = Array.Empty<string>();
        }         
        _nicknameEntered = new TaskCompletionSource<bool>();
        await _nicknameEntered.Task;
    }

    public async void SetNickName(string nickName, Action<string,bool> result)
    {
        await Check(nickName, result);
    }
    
    private async Task Check(string nickname, Action<string,bool> result)
    {
        if (nickname.Length > 10)
        {
            result?.Invoke("닉네임은 10자 이하로 입력해주세요.",false);
            return;
        }
        
        foreach (string word in lines)
        {
            if (!string.IsNullOrWhiteSpace(word) && nickname.Contains(word))
            {
                result?.Invoke("비속어는 사용할 수 없습니다.",false);
                return;
            }
        }
        
        string cleaned = Regex.Replace(nickname, @"[^a-zA-Z0-9가-힣 ]", "");

        if (nickname.Equals(cleaned))
        {       
            if (await Main.Ins.MainData.NameData.CheckNickNameAsync(nickname))
            {
                result?.Invoke("중복된 닉네임입니다",false);
                return;
            }
            result?.Invoke("닉네임이 변경되었습니다.",true);
            
            await Main.Ins.MainData.UserData.SaveName(nickname);
            await Main.Ins.MainData.NameData.Save(nickname);
            
            _nicknameEntered.TrySetResult(true);
            Destroy(this.gameObject);
        }
        else
        {
            result?.Invoke("특수문자는 사용할 수 없습니다.",false);
        }
    }
}
