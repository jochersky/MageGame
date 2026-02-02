using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int dimensions = 5;
    [SerializeField] int prefabResolution = 8; 
    // TODO: Delete this and properly randomize room 0s
    [SerializeField] GameObject room0Prefab;
    [SerializeField] GameObject room1Prefab;
    [SerializeField] GameObject room2Prefab;
    [SerializeField] GameObject room3Prefab;
    [SerializeField] GameObject room4Prefab;

    // Tilemap version
    [SerializeField] Grid grid;
    GameObject[] room0s;
    GameObject[] room1s;
    GameObject[] room2s;
    GameObject[] room3s;
    GameObject[] room4s;

    
    // this is apparently how you do multidimensional arrays
    int[,] map;
    int row = 0;
    int col = 0;
    // there is also a UnityEngine.Random
    System.Random randy;
    enum MOVING_TO
    {
        BELOW,
        LEFT,
        RIGHT
    }
    void Awake()
    {
        // Seems to be an issue with loading outside of specified folder; need to look into it
        room0s = Resources.LoadAll<GameObject>("Rooms/Room Type 0");
        room1s = Resources.LoadAll<GameObject>("Rooms/Room Type 1");
        room2s = Resources.LoadAll<GameObject>("Rooms/Room Type 2");
        room3s = Resources.LoadAll<GameObject>("Rooms/Room Type 3");
        room4s = Resources.LoadAll<GameObject>("Rooms/Room Type 4");
        randy = new System.Random();
        // room0Prefab = room0s[randy.Next(room0s.Length)];
        // room1Prefab = room1s[randy.Next(room1s.Length)];
        // room2Prefab = room2s[randy.Next(room2s.Length)];
        // room3Prefab = room3s[randy.Next(room3s.Length)];
        // room4Prefab = room4s[randy.Next(room4s.Length)];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new int[dimensions, dimensions];
        genRoomPaths();
        placeMap();
    }


    private void genRoomPaths()
    {
        // All rooms start out as 0s, meaning not on the solution path
        // the Length property is all elements, get length is one dimension
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
               map[row,col] = 0;
            }
        }
        // pick a starting room from the top row
        col = randy.Next(3);
        // 1 is tunnel style room: open on the left and right
        bool foundExit = false;
        Debug.Log("Generating...");
        while (!foundExit)
        {
            foundExit = pathfind();
        }    
        Debug.Log("Generation complete!");
        // Potentially print array here for debugging porpoises

    }

    private bool pathfind()
    {
        // Randomly pick 1-5
        // 1,2 means left. 3,4 means right. 5 means down
        int direction = randy.Next(1,6);
        if (direction == 1 || direction == 2)
        {
            // if left is not the edge of the map AND we haven't been there yet, we are good
            if (col - 1 >= 0 && map[row, col-1] == 0)
            {
                labelRoomNum(MOVING_TO.LEFT);
                col -= 1;
                return false;
            }
            // if left is an edge or already visited, we fall through and go down (lol)
        } 
        if (direction == 3 || direction == 4)
        {
            // if left is not the edge of the map AND we haven't been there yet, we are good
            if (col + 1 < map.GetLength(0) && map[row, col+1] == 0)
            {
                labelRoomNum(MOVING_TO.RIGHT);
                col += 1;
                return false;
            }
            // fall through (aaaaaahhh)
        }
        // the case where we go down, either because of a 5 or an illegal left/right
        // first, if we are on the bottom floor, just label the room and return true to indicate the exit was found
        if (row + 1 == map.GetLength(0))
        {
            // we are lying here, because we aren't actually moving anywhere, but want room types 1 and 3 anyway. Should probably fix.
            labelRoomNum(MOVING_TO.LEFT);
            return true;
        } else
        {
          // standard case, actually move down a floor
          labelRoomNum(MOVING_TO.BELOW);
          row += 1;
          return false;  
        }
    }

    // Room type 0 has no guaranteed exits
    // Room type 1 has exits on the left and right guaranteed
    // Room type 2 has exits on the left, right, and bottom (we love the Oxford comma)
    // Room type 3 has exits on the left, right, and top
    // Room type 4 has exits on the left, right, top, and bottom
    private void labelRoomNum(MOVING_TO direction)
    {
        // if we are moving left or right, then we are either 1 or 3.
        // if the room above us is offscreen, or is of type 0, 1, or 3, then we are type 1.
        // otherwise the room above us is type 2 or 3 which means we need a top exit and are type 3.
        if (direction == MOVING_TO.LEFT || direction == MOVING_TO.RIGHT)
        {
            if (row - 1 < 0 || map[row - 1, col] == 0 || map[row - 1, col] == 1 || map[row - 1, col] == 3)
            {
                map[row, col] = 1;
            } else
            {
                map[row, col] = 3;
            }
        }
        // otherwise, we are moving down
        // if the room above us is offscreen, or is of type 0, 1, or 3, then we are type 2.
        // otherwise the room above us is type 2 or 3 which means we need a top exit and are type 4.
        else
        {
            if (row - 1 < 0 || map[row - 1, col] == 0 || map[row - 1, col] == 1 || map[row - 1, col] == 3)
            {
                map[row, col] = 2;
            } else
            {
                map[row, col] = 4;
            }
        }
    }

    
    private void placeMap()
    {
        int x = 0;
        int y = 0;
        // +2 comes from extra top and bottom rows
        for (int row = -1; row < map.GetLength(0) + 1; row++)
        {
            for (int col = -1; col < map.GetLength(1) + 1; col++)
            {
                // top and bottom rows are all filled, as are the leftmost and rightmost columns
                if (row == -1 || row == map.GetLength(0) || col == -1 || col == map.GetLength(0))
                {
                    Instantiate(room0s[0], new Vector3(x, y, 0), Quaternion.identity, grid.transform);
                } else // normal room creation
                {
                   int roomNum = map[row,col];
                    switch (roomNum)
                    {
                        case 1: Instantiate(room1s[randy.Next(room1s.Length)], new Vector3(x, y, 0), Quaternion.identity, grid.transform); break;
                        case 2: Instantiate(room2s[randy.Next(room2s.Length)], new Vector3(x, y, 0), Quaternion.identity, grid.transform); break;
                        case 3: Instantiate(room3s[randy.Next(room3s.Length)], new Vector3(x, y, 0), Quaternion.identity, grid.transform); break;
                        case 4: Instantiate(room4s[randy.Next(room4s.Length)], new Vector3(x, y, 0), Quaternion.identity, grid.transform); break;
                        default: Instantiate(room0s[randy.Next(room0s.Length)], new Vector3(x, y, 0), Quaternion.identity, grid.transform); break;
                } 
                }
                
                x += prefabResolution;
            }
            x = 0;
            y -= prefabResolution;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
