using System;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int mapDimensions = 5;
    [SerializeField] int roomDimensions = 8; 
    [SerializeField] GameObject tilemapPrefab;
    [SerializeField] Color32 guaranteeSquareColor;
    [SerializeField] Color32 highProbabilityColor;
    [SerializeField] Color32 lowProbabilityColor;
    [SerializeField] Color32 noSquareColor;
    [SerializeField] Color32 entryExitColor;
    [SerializeField] Color32 spikeOrBlockColor;
    [SerializeField] Color32 spikeOrSpaceColor;
    [SerializeField] Color32 falseFloorColor; 
    [SerializeField] Color32 flamethrowerColor;   

    // Tilemap version
    [SerializeField] Grid grid;
    [SerializeField] RuleTile tile;
    [SerializeField] TileBase door;
    [SerializeField] TileBase spikes;
    [SerializeField] TileBase falseFloor;
    [SerializeField] TileBase flamethrower;
    [SerializeField] Tilemap colliderTilemap;
    [SerializeField] Tilemap nonColliderTilemap;
    Sprite[] filledRoom;
    Sprite[] room0s;
    Sprite[] room1s;
    Sprite[] room2s;
    Sprite[] room3s;
    Sprite[] room4s;
    Sprite template;

    int entranceCol;
    int exitCol;

    
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

    enum ROOM_QUALITY
    {
        STARTING,
        ENDING,
        REGULAR
    }
    void Awake()
    {
        // Seems to be an issue with loading outside of specified folder; need to look into it
        filledRoom = Resources.LoadAll<Sprite>("Rooms/Filled Room");
        room0s = Resources.LoadAll<Sprite>("Rooms/Room Style 0");
        room1s = Resources.LoadAll<Sprite>("Rooms/Room Style 1");
        room2s = Resources.LoadAll<Sprite>("Rooms/Room Style 2");
        room3s = Resources.LoadAll<Sprite>("Rooms/Room Style 3");
        room4s = Resources.LoadAll<Sprite>("Rooms/Room Style 4");
        randy = new System.Random();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new int[mapDimensions, mapDimensions];
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
        entranceCol = randy.Next(mapDimensions);
        col = entranceCol;
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
            // store exit column
            exitCol = col;
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
        bool isStartingRoom = false;
        bool isEndingRoom = false;
        // +2 comes from extra top and bottom rows
        for (int row = -1; row < map.GetLength(0) + 1; row++)
        {
            for (int col = -1; col < map.GetLength(1) + 1; col++)
            {
                // check for starting and ending rooms
                isStartingRoom = row == 0 && col == entranceCol;
                isEndingRoom = row == (mapDimensions - 1) && col == exitCol;
                // top and bottom rows are all filled, as are the leftmost and rightmost columns
                if (row == -1 || row == map.GetLength(0) || col == -1 || col == map.GetLength(0))
                {
                    InstantiateRoom(filledRoom, x, y, isStartingRoom, isEndingRoom);
                } else // normal room creation
                {
                   int roomNum = map[row,col];
                    switch (roomNum)
                    {
                        case 1: InstantiateRoom(room1s, x, y, isStartingRoom, isEndingRoom); break;
                        case 2: InstantiateRoom(room2s, x, y, isStartingRoom, isEndingRoom); break;
                        case 3: InstantiateRoom(room3s, x, y, isStartingRoom, isEndingRoom); break;
                        case 4: InstantiateRoom(room4s, x, y, isStartingRoom, isEndingRoom); break;
                        default: InstantiateRoom(room0s, x, y, isStartingRoom, isEndingRoom); break;
                    } 
                }
                
                x += roomDimensions;
            }
            x = 0;
            y -= roomDimensions;
        }
    }

    void InstantiateRoom(Sprite[] rooms, int x, int y, bool isStartingRoom, bool isEndingRoom)
    {
        ROOM_QUALITY room_quality = ROOM_QUALITY.REGULAR;
        if (isStartingRoom) { room_quality = ROOM_QUALITY.STARTING; }
        if (isEndingRoom) { room_quality = ROOM_QUALITY.ENDING; }
        // GameObject tileMap = Instantiate(tilemapPrefab, new Vector3(x, y, 0), Quaternion.identity, grid.transform);
        // Tilemap tilemap = tileMap.GetComponent<Tilemap>();
        template = rooms[randy.Next(rooms.Length)];
        Color32[] pixels = ConvertSpriteToPixelArray(template);
        int[] room = TranslateColorsToProbabilities(pixels, room_quality);
        GenerateRoom(room, x, y);
    }

    Color32[] ConvertSpriteToPixelArray(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        if (!texture.isReadable)
        {
            Debug.Log("That's an Error! The provided sprite template does not have Read/Write enabled.");
            return null;
        }
        // array starts at bottom left, moves right
        Color32[] pixels = texture.GetPixels32();
        return pixels;


    }

    int[] TranslateColorsToProbabilities(Color32[] pixels, ROOM_QUALITY room_quality)
    {
        int[] roomProbs = new int[roomDimensions * roomDimensions];
        for (int row = 0; row < roomDimensions; row++)
        {
            for (int col = 0; col < roomDimensions; col++)
            {
                Color32 color = pixels[row * roomDimensions + col];
                // maybe switch statements hate me. who knows?
                if (color.Equals(guaranteeSquareColor))
                {
                    roomProbs[row * roomDimensions + col] = 100;
                } else if (color.Equals(highProbabilityColor))
                {
                    roomProbs[row * roomDimensions + col] = 75;
                } else if (color.Equals(lowProbabilityColor))
                {
                    roomProbs[row * roomDimensions + col] = 25;
                } else if (color.Equals(spikeOrBlockColor))
                {
                    roomProbs[row * roomDimensions + col] = -75;
                } else if (color.Equals(spikeOrSpaceColor))
                {
                    roomProbs[row * roomDimensions + col] = -25;
                } else if (color.Equals(falseFloorColor))
                {
                    roomProbs[row * roomDimensions + col] = -88;
                } else if (color.Equals(flamethrowerColor))
                {
                    roomProbs[row * roomDimensions + col] = -55;
                } else if (color.Equals(entryExitColor))
                {
                    if (room_quality == ROOM_QUALITY.STARTING || room_quality == ROOM_QUALITY.ENDING)
                    {
                        roomProbs[row * roomDimensions + col] = -99;
                    } else {
                        roomProbs[row * roomDimensions + col] = 0;
                    }
                }
                else if (color.Equals(noSquareColor))
                {
                    roomProbs[row * roomDimensions + col] = 0;

                } else
                {
                    Debug.Log("That's an Error! The provided sprite template uses colors not specified by probabilities");
                    Debug.Log("Was: " + color);
                    return null;
                }
            }
        }
        return roomProbs;
    }


    void GenerateRoom(int[] room, int xOffset, int yOffset)
    {
        for (int row = 0; row < roomDimensions; row++)
        {
            for (int col = 0; col < roomDimensions; col++)
            {
                int xCoord = col + xOffset;
                int yCoord = row + yOffset;
                int roomProbability = room[row * roomDimensions + col];
                // check for special value indicating a door
                if (roomProbability == -99)
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), door);
                }
                // check for special value indicating spikes
                else if (roomProbability == -75 || roomProbability == -25){
                    if (randy.Next(0,100) < 25)
                    {
                        nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), spikes);
                    } else if (roomProbability == -75)
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), tile);
                    }
                }
                // check for special value indicating false floor
                if (roomProbability == -88)
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), falseFloor);
                }
                // check for special value indicating flamethrower
                else if (roomProbability == -55){
                    if (randy.Next(0,100) < 25)
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), flamethrower);
                    } else
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), tile);
                    }
                }
                else if (randy.Next(0,100) < roomProbability)
                {
                    colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), tile);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
