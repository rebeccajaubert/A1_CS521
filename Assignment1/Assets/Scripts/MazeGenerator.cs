
using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();

    }

    void Start()
    {
        CreateMaze();
        
    }

    void CreateMaze()
    {
        Node startNode = grid.GetGridPosition(0, 0);
        Node targetNode = grid.GetGridPosition((int) (grid.gridWorldSize.x/(2*grid.nodeRadius)) - 1, (int)(grid.gridWorldSize.y / (2 * grid.nodeRadius)) -1); //last cell of grid
        int count = 0;
        int gridSize = (targetNode.gridX+1) * (targetNode.gridX+1);

        List<Node> visitedNodes = new List<Node>();
        List<Node> withTiles = new List<Node>();
        System.Random rd = new System.Random();


        visitedNodes.Add(startNode);

        while (withTiles.Count != gridSize)
        {
            while (visitedNodes.Count > 0)
            {
                Node currentNode = visitedNodes[0];


                List<Node> neighbours = grid.GetNeighbours(currentNode);
                Node neighbourNode = null;

                while (neighbours.Count > 0) //search for random empty cell adjacent 
                {
                    int random = rd.Next(0, neighbours.Count);
                    neighbourNode = neighbours[random];
                    if (!visitedNodes.Contains(neighbourNode) && neighbourNode.hasTile == false) //check if no tile already present
                    {
                        neighbourNode.parent = currentNode;
                        grid.ChooseTile(currentNode, neighbourNode);
                        if(!withTiles.Contains(currentNode)) withTiles.Add(currentNode);
                        visitedNodes.Add(neighbourNode);

                        if (neighbourNode == targetNode)
                        {
                            grid.ChooseTile(neighbourNode, null);
                            withTiles.Add(neighbourNode);
                            break;//return; 
                        }

                        break;    
                    }
                    neighbours.Remove(neighbourNode); //keep looping to search for valid neighbour
                    if (count > 1000) { Debug.Log("BREAK"); break; }
                }

                visitedNodes.Remove(currentNode); //if reach here then no empty cell adjacent

                count++; if (count > 1000) { Debug.Log("break"); break; }

               
            }

            if (withTiles.Count != gridSize) //if here then we need to restart the search because the maze is not full yet but there is no more adjacent node empty
            {
                bool hasEmptyAdjacent = false;
                while (!hasEmptyAdjacent)
                {
                    int r = rd.Next(0, withTiles.Count);
                    Node randomTile = withTiles[r];                 //choose a random tile and see if any empty adjacent
                    Node newStart = null;
                    List<Node> neighbours = grid.GetNeighbours(randomTile);

                    foreach (Node ngb in neighbours)
                    {
                        if (ngb.hasTile == false)
                        {
                            hasEmptyAdjacent = true;
                            newStart = ngb;
                            break;
                        }
                    }
                    if (hasEmptyAdjacent)
                    {

                          Destroy(randomTile.tile);
                          grid.InstantiateTile(randomTile, "");
                        grid.InstantiateTile(newStart, "");
                        withTiles.Add(newStart);
                        newStart.parent = randomTile;
                        visitedNodes.Add(newStart);

                    }
                    count++;
                    if (count > 1000) { Debug.Log("bbbbbbb"); break; }
                }
            }
            count++;
            if (count > 1000) { Debug.Log("da"); break; }
        }
        




    }








}
