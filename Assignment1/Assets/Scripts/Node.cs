using UnityEngine;
using System.Collections;


public class Node
{
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;		//to keep track of node position
	public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public string tileType;
    public bool hasTile = false;
    public GameObject tile;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}



    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    public override string ToString()
    {
		return "node : " + gridX + " " + gridY;
    }
}