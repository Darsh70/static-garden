using UnityEngine;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public int width = 12;
    public int height = 8;
    public float spacing = 1.2f;

    [Header("Gameplay Balance")]
    public float spreadTickRate = 1.5f;
    public float randomFungusChance = 0.02f; // Chance per tick for a new fungus to appear
    public float plantDensity = 0.25f;       // 25% of grid is plants

    [Header("Spread Chances")]
    [Range(0, 1)] public float plantInfectionChance = 0.3f; // 30% chance to start rotting a plant
    [Range(0, 1)] public float emptySpreadChance = 0.05f;  // 5% chance to grow into empty space

    private Cell[,] _grid;

    void Start()
    {
        GenerateGrid();
        InvokeRepeating("TickInfection", spreadTickRate, spreadTickRate);
    }

    void GenerateGrid()
    {
        _grid = new Cell[width, height];
        Vector3 offset = new((width - 1) * spacing / 2f, (height - 1) * spacing / 2f, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0) - offset;
                Cell c = Instantiate(cellPrefab, pos, Quaternion.identity, transform).GetComponent<Cell>();
                c.x = x; c.y = y;
                
                // Mostly empty, some plants
                if (Random.value < plantDensity) c.SetType(CellType.Plant);
                else c.SetType(CellType.Empty);

                _grid[x, y] = c;
            }
        }
    }

    void TickInfection()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = _grid[x, y];

                // Fungus pop-up
                if (cell.type == CellType.Empty && Random.value < randomFungusChance)
                {
                    cell.SetType(CellType.Fungus);
                }

                // Spread
                if (cell.type == CellType.Fungus)
                {
                    SpreadFrom(x, y);
                }
            }
        }
    }

    void SpreadFrom(int x, int y)
    {
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                Cell neighbor = _grid[nx, ny];
                
                if (neighbor.type == CellType.Plant && !neighbor.isStruggling)
                {
                    if (Random.value < plantInfectionChance) neighbor.StartInfection();
                }
                
                if (neighbor.type == CellType.Empty)
                {
                    if (Random.value < emptySpreadChance) neighbor.SetType(CellType.Fungus);
                }
            }
        }
    }
}