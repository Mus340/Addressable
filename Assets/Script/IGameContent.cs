using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameContent
{
    public void OnInitialized();
    public void OnEnd();
    public void OnBegin();
    public void OnSubscribe();
}
