using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorConfig
{
    public static Color GetColor(int level)
    {
        var color = Color.white;
        switch ((level-1) % 7)
        {
            case 0: color = new Color(1f, 0.4f, 0.4f); break;
            case 1: color = new Color(1f, 0.6f, 0.3f); break;
            case 2: color = new Color(1f, 1f, 0.5f); break;
            case 3: color = new Color(0.5f, 1f, 0.5f); break;
            case 4: color = new Color(0.5f, 0.7f, 1f); break;
            case 5: color = new Color(0.6f, 0.5f, 1f); break;
            case 6: color = new Color(0.8f, 0.5f, 1f); break;
        }
        return color;
    }
}
