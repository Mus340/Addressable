using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCubeBar : MonoBehaviour
{
    public ColorCubeItem colorCube;
    private ObjectPool<ColorCubeItem> _cubes;
    private List<ColorCubeItem> _useCubes = new();
    

    public void SetData(LevelData data, int answerIndex)
    {
        if (_cubes == null)
        {
            _cubes = new ObjectPool<ColorCubeItem>(colorCube, 6,transform);
        }
        var color = ColorConfig.GetColor(data.id);
        var lerpColor = Color.Lerp(color, Color.white, data.color_value);
        transform.localPosition = new Vector3(0, data.id, data.id);
        for (int i = 0; i < data.cube_count; i++)
        {
            var cube = _cubes.Get();
            cube.transform.localPosition = new Vector3(i, 0, 0);
            cube.SetData(data.id, i, color);
            _useCubes.Add(cube);
        }
        _useCubes[answerIndex].SetData(data.id, answerIndex, lerpColor);
    }

    public void ResetPool()
    {
        foreach (var useCube in _useCubes)
        {
            _cubes.ReturnToPool(useCube);
        }
        _useCubes.Clear();
    }
}
