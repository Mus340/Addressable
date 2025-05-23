using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTime : MonoBehaviour
{
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }
}
