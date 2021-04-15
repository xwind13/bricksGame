﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Sprite sprite;
    public int[,] Grid;
    int Vertical, Horizontal, Columns, Rows;

// Start is called before the first frame update
void Start()
    {
        Vertical = (int)Camera.main.orthographicSize;
        Horizontal = Vertical * (Screen.width / Screen.height);
        Columns = Horizontal * 2;
        Rows = Vertical * 2;

        Grid = new int[Columns, Rows];
        for (int i = 0; i< Columns; i++)
        {
            for (int j=0; j<Rows; j++)
            {
                Grid[i, j] = Random.Range(0, 10);
                //SpawnTile(i, j, Grid[i, j]);
            }
        }
    }

    private void SpawnTile(int x, int y, int value)
    {
        GameObject g = new GameObject($"x: {x}, y: {y}");
        g.transform.position = new Vector3(x - (Horizontal - 0.5f), y - (Vertical - 0.5f));
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = sprite;
        s.color = new Color((float)value / 10, (float)value / 10, (float)value / 10);


        var bounds = s.sprite.bounds;
    }
}