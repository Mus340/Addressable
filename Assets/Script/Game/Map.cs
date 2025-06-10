using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform barParent;
    public ColorCubeBar cubeBar;
    private ObjectPool<ColorCubeBar> _cubeBarPool;
    private Queue<ColorCubeBar> _useCubeQueue;

    private const int CUBE_RANGE = 30;
    
    public void Initialize()
    {
    }

    public void Next()
    {
        
    }
}
