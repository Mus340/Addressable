using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCubeBar : MonoBehaviour
{
    public ColorCubeItem colorCube;
    private ObjectPool<ColorCubeItem> _cubes;
    private List<ColorCubeItem> _useCubes;

    public void Initialize()
    {
        _cubes = new ObjectPool<ColorCubeItem>(colorCube, 4,transform);
        _useCubes = new List<ColorCubeItem>();
    }
    
    public void SetData(LevelData data, int answerIndex)
    {
        var color = ColorConfig.GetColor(data.id);
        var fadeFactor = Mathf.Lerp(1f, 0.05f, Mathf.Log(data.id, 50));
        var lerpColor = Color.Lerp(color, Color.white, fadeFactor);
        
        transform.localPosition = new Vector3(0, data.id, data.id);
        for (int i = 0; i < data.block_count; i++)
        {
            var cube = _cubes.Get();
            cube.transform.localPosition = new Vector3(i, 0, 0);
            cube.SetColor(color);
            _useCubes.Add(cube);
        }
        _useCubes[answerIndex].SetColor(lerpColor);
    }

    public void ResetPool()
    {
        foreach (var useCube in _useCubes)
        {
            _cubes.ReturnToPool(useCube);
        }
    }
}
