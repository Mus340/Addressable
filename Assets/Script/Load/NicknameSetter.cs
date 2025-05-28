using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class NicknameSetter : MonoBehaviour
{
    private static NicknameSetter _ins;
    public static NicknameSetter Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<NicknameSetter>();
            }
            return _ins;
        }
    }
    
    public TextAsset profanityFile;
    private string[] lines;
    private string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";   

    void Awake()
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
    }

    public void SetNickName(string nickname)
    {
        if (nickname.Length > 7)
        {
            ShowResult("닉네임은 7자 이하로 입력해주세요.");
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

        string cleaned = Regex.Replace(nickname, @"[^a-zA-Z0-9가-힣 ]", "");

        if (nickname.Equals(cleaned))
        {
            ShowResult("닉네임이 변경되었습니다.");
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
