using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Node
{
    public bool validIndex = false;
    public Vector2Int position;
    public Vector2Int gridPos;
   public float f;
   public float g;
   public float h;
    public Node previous;
}

[RequireComponent(typeof(Tilemap))]
public class AIPathMaker : MonoBehaviour
{
    // Start is called before the first frame update

    Tilemap tilemap;

    List<Vector2> drawPos = new List<Vector2>();

    //All tiles with position / create at start
    //Check for valid index here
    List<List<Node>> grid;

    public static AIPathMaker Instance;

    const float MAX = 100000;

    int width;
    int height;

    Vector2 min;

    Vector2Int currentPos = new Vector2Int();
    Vector3Int tilemapPos = new Vector3Int();
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        grid = new List<List<Node>>();
        tilemap =GetComponent<Tilemap>();

        min = tilemap.localBounds.min;


        tilemapPos.z = 0;
        for (int x = 0; x < tilemap.size.x; x++ )
        {
            grid.Add(new List<Node>());
            for (int y = 0; y < tilemap.size.y; y++)
            {
                currentPos.x = x + (int)min.x;
                tilemapPos.x = x + (int)min.x;
                currentPos.y = y + (int)min.y;
                tilemapPos.y = y + (int)min.y;
                Node n = new Node();
                if (tilemap.HasTile(tilemapPos))
                {
                    n.validIndex = true;
                }
                n.position = currentPos;
                n.previous = null;
                n.gridPos = new Vector2Int(x, y);
                grid[x].Add(n);
            }
        }

        Debug.Log(grid[0][0].position);
        
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    Vector2 offset = new Vector2(0.5f, 0.5f);


    public List<Vector2> GetPath(Vector2Int start, Vector2Int end)
    {

        foreach(List<Node> col in grid)
        {
            foreach(Node row in col)
            {
                row.previous = null;
                row.f = 0;
                row.g = 0;
                row.h = 0;
            }
        }
        int startXoffSet = start.x - (int)min.x;
        int startYoffSet = start.y - (int)min.y;

        if (!(startXoffSet >= 0 && startXoffSet < grid.Count) || !(startYoffSet >= 0 && startYoffSet< grid[startXoffSet].Count))
        {
            Debug.LogError("OUT OF BOUNDS"); 
            return null;
        }
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        List<Node> path = new List<Node>();

        Node current = grid[startXoffSet][startYoffSet];

        
        current.f = 0;
        current.g = 0;
        current.h = 0;



        openSet.Add(current);

        while (openSet.Count != 0)
        {
            int winner = 0;
            for(int i = 0; i < openSet.Count; i++)
            {
                if(openSet[i].f < openSet[winner].f)
                {
                    winner = i;
                }
                else if(openSet[i].f == openSet[winner].f)
                {
                    if(openSet[i].h < openSet[winner].h)
                    {
                        winner = i;
                    }
                }
            }

            current = openSet[winner];

            path = new List<Node>();

            Node temp = current;
            while (temp.previous != null)
            {
                path.Add(temp.previous);
                temp = temp.previous;
            }

            if(current.position == end)
            {
                break;
            }
            openSet.RemoveAt(winner);
            closedSet.Add(current);

            int xIndex = current.gridPos.x;
            int yIndex = current.gridPos.y;

            for(int x = -1; x <=1; x++)
            {
                xIndex = current.gridPos.x + x;
                for (int y = -1; y <= 1; y++)
                {
                    yIndex = current.gridPos.y + y;

                    if ((xIndex >=0 && xIndex < grid.Count) && (yIndex  >= 0 && yIndex  < grid[xIndex].Count))
                    {
                        Node n = grid[xIndex][yIndex];
                        if (!closedSet.Contains(n) && n.validIndex)
                        {
                            float tempG = current.g + 1;
                            bool newPath = false;
                            if(openSet.Contains(n))
                            {
                                if(tempG < n.g)
                                {
                                    n.g = tempG;
                                    newPath = true;
                                }
                            }
                            else
                            {
                                n.g = tempG;
                                newPath = true;
                                openSet.Add(n);
                            }
                            if(newPath)
                            {
                                n.h = GetHeuristic(n.position,  end);
                                n.f = n.g + n.h;
                                n.previous = current;
                            }

                        }
                    }
                }
            }
        }
        List<Vector2> returnPath = new List<Vector2>();

        foreach(Node n in path)
        {
            returnPath.Add(n.position + half);
        }


        return returnPath;
    }

    Vector2 half = new Vector2(0.5f, 0.5f);

    float GetHeuristic(Vector2 start, Vector2 end)
    {
       return Mathf.Sqrt((start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y));
    }
}
