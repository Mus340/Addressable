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

    public async void SetNickName(string nickName)
    {
        await Check(nickName);
    }
    
    public async Task Check(string nickname)
    {
        if (nickname.Length > 10)
        {
            ShowResult("닉네임은 10자 이하로 입력해주세요.");
            return;
        }
        
        foreach (string word in lines)
        {
            if (!string.IsNullOrWhiteSpace(word) && nickname.Contains(word))
            {
                ShowResult("비속어는 사용할 수 없습니다.");
                return;
            }
        }

        if (await Main.Ins.MainData.NameData.CheckNickNameAsync(nickname))
        {
            ShowResult("중복된 닉네임입니다");
            return;
        }
        
        string cleaned = Regex.Replace(nickname, @"[^a-zA-Z0-9가-힣 ]", "");

        if (nickname.Equals(cleaned))
        {
            ShowResult("닉네임이 변경되었습니다.");
            
            Main.Ins.MainData.UserData.SaveName(nickname);
            Main.Ins.MainData.NameData.Save(nickname);
            Main.Ins.MainData.RankData.SaveInfo(nickname);
            
            _nicknameEntered.TrySetResult(true);
            Destroy(this.gameObject);
        }
        else
        {
            ShowResult("특수문자는 사용할 수 없습니다.");
        }
    }
    
    private void ShowResult(string message)
    {
        Debug.Log(message);
    }
}
