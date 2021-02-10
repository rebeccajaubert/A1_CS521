using UnityEngine;
using System;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;
    Vector3 worldBottomLeft;

    public Transform player;

    public List<Node> path;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //how many nodes can we fit into grid
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
       // createPath();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //gives position of left corner

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); //true if there is not a collision 
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node) //not diagonal neighbours
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0) || (Math.Abs(x) == 1 && Math.Abs(y)==1) )
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) //convert a position on terrain into a grid coordinate
    {
        
        float percentX = ( worldPosition.x - worldBottomLeft.x ) / gridWorldSize.x;
        float percentY = (worldPosition.z - worldBottomLeft.z) / gridWorldSize.y;
       
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Node GetGridPosition(int x, int y )
    {
        return grid[x, y];
    }
 


    public void createPath(Color color)
    {
        foreach (Node n in grid)
        {
            if (path != null)
            {
                if (path.Contains(n))
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = n.worldPosition;
                    cube.GetComponent<Renderer>().material.color = color;
                    cube.transform.localScale = new Vector3(nodeDiameter, 0.1f, nodeDiameter);
                    cube.GetComponent<BoxCollider>().enabled = false;

                }
            }
        }

    }

    public GameObject projectile;
    public void SpawnProjectiles(List<Node> path) //spawn 15
    {
        List<int> positionWithProj = new List<int>();
        if (path != null)
        {
            System.Random rd = new System.Random();
            for (int i = 0; i < 15; i++)
            {
                int randomPosition = rd.Next(10, path.Count);
                if (positionWithProj.Contains(randomPosition)) continue;

                Node randomNodeOnPath = path[randomPosition];

                GameObject p = Instantiate(projectile);
                p.SetActive(true);
                Vector3 offset = new Vector3(0, 0.1f, 0);
                p.transform.position = randomNodeOnPath.worldPosition + offset;
                positionWithProj.Add(randomPosition);
            }
        }
    }

    public void ChooseTile(Node n, Node directionNode)
    {
        List<string> possibleTypes = new List<string>();
        Node lastNode = GetGridPosition((int)(gridWorldSize.x / nodeDiameter) - 1, (int)(gridWorldSize.y / nodeDiameter) - 1); //last cell of grid
       
        if (n == grid[0, 0])
        { 
            if (directionNode.gridX == 1) { InstantiateTile(n, "horiz"); }
            else { InstantiateTile(n, "turn0"); }
            return;
        }

        //lastnode must be an exit
        if (n == lastNode)
        {
            if (n.parent.gridY == lastNode.gridY) { InstantiateTile(n, "horiz"); }
            else { InstantiateTile(n, "turn2"); }
            return;
        }

        else 
        {
            if (n.parent == null || directionNode == null) { Debug.Log("p " + n.parent.ToString()); Debug.Log(" n " + n.ToString()); Debug.Log( " dire " + directionNode.ToString()); }//TODO
            if(n.parent.gridX == directionNode.gridX)
            {
                possibleTypes.Add("verti");
            }
            else if (n.parent.gridY == directionNode.gridY)
            {
                possibleTypes.Add("horiz");
            }

            else if(n.parent.gridX < directionNode.gridX)
            {
                if (n.parent.gridY < directionNode.gridY)
                {
                    if (n.parent.gridY == n.gridY) possibleTypes.Add("turn0");
                    else if (directionNode.gridY == n.gridY) possibleTypes.Add("turn2");
                }
                else if (n.parent.gridY > directionNode.gridY)
                {
                    if (n.parent.gridY == n.gridY) possibleTypes.Add("turn3");
                    else if (directionNode.gridY == n.gridY) possibleTypes.Add("turn1");
                } 
                
            }
            else if (n.parent.gridX > directionNode.gridX)
            {
                if (n.parent.gridY < directionNode.gridY)
                {
                    if (n.parent.gridX == n.gridX) possibleTypes.Add("turn3");
                    else if (directionNode.gridX == n.gridX) possibleTypes.Add("turn1");
                }
                else if (n.parent.gridY > directionNode.gridY)
                {
                    if (n.parent.gridX == n.gridX) possibleTypes.Add("turn0");
                    else if (directionNode.gridX == n.gridX) possibleTypes.Add("turn2");
                }
            }
        }


     //   Debug.Log(" count " + possibleTypes.Count +" parent "+ n.parent.ToString()+ " node " + n.ToString() + " direction " + directionNode.ToString());
        string currentType = possibleTypes[0];
        InstantiateTile(n, currentType);

    }

    public GameObject horizTile;
    public GameObject turnupTile;
    public Material floorMaze;
    public GameObject wall;
    public LayerMask maze;
    public void InstantiateTile(Node n, string type) //types : horiz,verti and different turns up,down,left,right
    {
        n.hasTile = true;
        switch (type)
        {
            case "horiz":
                GameObject tile = Instantiate(horizTile);
              //  tile.layer = maze;
                tile.SetActive(true);
                tile.transform.position = n.worldPosition;
                n.tileType = "horiz";
                n.tile = tile;
                break;

            case "verti":
                GameObject vertiTile = Instantiate(horizTile);
             //   vertiTile.layer = maze;
                vertiTile.SetActive(true);
                vertiTile.transform.position = n.worldPosition;
                vertiTile.transform.rotation = Quaternion.Euler(0f, 90, 0f);
                n.tileType = "verti";
                n.tile = vertiTile;
                break;
            case "turn0":
                GameObject upTile = Instantiate(turnupTile);
               // upTile.layer = maze;
                upTile.SetActive(true);
                upTile.transform.position = n.worldPosition;
                n.tileType = "turn0";
                n.tile = upTile;
                break;
            case "turn1":
                GameObject turn1 = Instantiate(turnupTile);
             //   turn1.layer = maze;
                turn1.SetActive(true);
                turn1.transform.position = n.worldPosition;
                turn1.transform.rotation = Quaternion.Euler(0f, 90, 0f);
                n.tileType = "turn1";
                n.tile = turn1;
                break;
            case "turn2":
                GameObject turn2 = Instantiate(turnupTile);
             //   turn2.layer = maze;
                turn2.SetActive(true);
                turn2.transform.position = n.worldPosition;
                turn2.transform.rotation = Quaternion.Euler(0f, 180, 0f);
                n.tileType = "turn2";
                n.tile = turn2;
                break;
            case "turn3":
                GameObject turn3 = Instantiate(turnupTile);
             //   turn3.layer = maze;
                turn3.SetActive(true);
                turn3.transform.position = n.worldPosition;
                turn3.transform.rotation = Quaternion.Euler(0f, 270, 0f);
                n.tileType = "turn3";
                n.tile = turn3;
                break;
            default:
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  cube.layer = maze;
                GameObject w = Instantiate(wall);
                w.SetActive(true);
                Vector3 offset = new Vector3(0, 5, 0);
                w.transform.position = n.worldPosition +offset;
                cube.transform.position = n.worldPosition;
                cube.GetComponent<Renderer>().material = floorMaze;
                cube.transform.localScale = new Vector3(nodeDiameter-10, 0.1f, nodeDiameter-10);
                cube.GetComponent<BoxCollider>().enabled = true;
                n.tile = cube;
                w.transform.SetParent(cube.transform, true);
                break;

        } 
        
    }
    


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y)); //define the zone of the map

    }
}

