using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.LightTransport.PostProcessing;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int mapDimensions = 5;
    [SerializeField] int roomDimensions = 8; 
    [SerializeField] int level = 1;
    [SerializeField] int seed = 0;
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
    [SerializeField] Color32 decorationColor;
    [SerializeField] Color32 torchColor;
    [SerializeField] Color32 chestColor;
    [SerializeField] Color32 specialEnemyColor;
    [SerializeField] Color32 specialItemColor;
    [SerializeField] Color32 NPCSpawnColor;

    [SerializeField] Color32 enemyColor;
    [SerializeField] GameObject enemy;   

    // Tilemap version
    [SerializeField] Grid grid;

    [SerializeField] TileBase entryDoor;
    [SerializeField] TileBase flamethrower;
    [SerializeField] TileBase torch;
    [SerializeField] TileBase chest;
    [SerializeField] ConsumableConfig rope;
    [SerializeField] Tilemap colliderTilemap;
    [SerializeField] Tilemap nonColliderTilemap;
    [SerializeField] LevelData levelData;
    
    Sprite[] filledRoom;
    Sprite[] chestRoom;
    Sprite[] room0s;
    Sprite[] room1s;
    Sprite[] room2s;
    Sprite[] room3s;
    Sprite[] room4s;
    Sprite template;
    Dictionary<string, Sprite> specialRooms;

    // NPC fields
    NPC[] NPCPrefabs;
    List<NPC> NPCInstances = new();
    readonly string NPCpath = "Characters/";
    List<(int x, int y)> emptyFloorSpaces = new();
    List<(int x, int y)> emptyCeilingSpaces = new();

    int entranceCol;
    int exitCol;

    // Reference to player
    public GameObject player;
    [SerializeField] float startingPositionOffset;
    Vector2 startingPosition;
    bool startingPositionAssigned = false;
    Vector2 exitPosition;

    int numChestRooms = 3;
    List<(int x, int y)> specialRoomCoords = new();
    List<Vector2> ropeCoords = new();

    public delegate void PlayerPlaced(GameObject player);
    public event PlayerPlaced OnPlayerPlaced;

    
    // this is apparently how you do multidimensional arrays
    MapRoom[,] map;
    int row = 0;
    int col = 0;
    // there is also a UnityEngine.Random
    System.Random randy;
    enum MOVING_TO
    {
        BELOW,
        LEFT,
        RIGHT,
        UP
    }

    enum ROOM_QUALITY
    {
        STARTING,
        ENDING,
        REGULAR,
        CHEST,
        NPC
    }


    
    // Room type 0 has no guaranteed exits
    // Room type 1 has exits on the left and right guaranteed
    // Room type 2 has exits on the left, right, and bottom (we love the Oxford comma)
    // Room type 3 has exits on the left, right, and top
    // Room type 4 has exits on the left, right, top, and bottom
    // The recent designation identifies it as having been placed as a part of the current path, as opposed to another path
    enum ROOM_STYLE
    {
        UNSPECIFIED,
        LEFT_RIGHT,
        BOTTOM_LEFT_RIGHT,
        TOP_LEFT_RIGHT,
        CROSS
    }

    public Tilemap getColliderMap()
    {
        return colliderTilemap;
    }
    void Awake()
    {
        // Seems to be an issue with loading outside of specified folder; need to look into it
        filledRoom = Resources.LoadAll<Sprite>("Rooms/Filled Room");
        chestRoom = Resources.LoadAll<Sprite>("Rooms/Chest Room");
        room0s = Resources.LoadAll<Sprite>("Rooms/Room Style 0");
        room1s = Resources.LoadAll<Sprite>("Rooms/Room Style 1");
        room2s = Resources.LoadAll<Sprite>("Rooms/Room Style 2");
        room3s = Resources.LoadAll<Sprite>("Rooms/Room Style 3");
        room4s = Resources.LoadAll<Sprite>("Rooms/Room Style 4");
        NPCPrefabs = Resources.LoadAll<NPC>(NPCpath);
        if (seed == 0)
        {
            randy = new System.Random();
            seed = randy.Next(int.MinValue, int.MaxValue);
            randy = new System.Random(seed);
            
        } else
        {
            randy = new System.Random(seed);
        }
        Debug.Log("Generation using seed: " + seed);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new MapRoom[mapDimensions, mapDimensions];
        SpawnEntities();
        GenRoomPaths();
        PlaceMap();
        colliderTilemap.CompressBounds();
        nonColliderTilemap.CompressBounds();
        EventBus.Instance.HandleTileMapChanged();
        PlaceEntities();
        // teleport player to starting position
        player.transform.position = startingPosition;
        StartCoroutine(DelayedStart());
        
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        PlaceRopes();
        player.transform.position = startingPosition;
        OnPlayerPlaced?.Invoke(player);
    }

    void SpawnEntities()
    {
        foreach (NPC npc in NPCPrefabs)
        {
            NPCInstances.Add(Instantiate(npc, transform));
        }
    }


    private void GenRoomPaths()
    {
        // All rooms start out as 0s, meaning not on the solution path
        // the Length property is all elements, get length is one dimension
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
               map[row,col] = new();
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
            foundExit = Pathfind();
        }
        AgeMap();
        PlaceSpecialRooms();
        Debug.Log("Generation complete!");

        // Potentially print array here for debugging porpoises
        DebugPrintMap();
    }

    private bool Pathfind()
    {
        // Randomly pick 1-5
        // 1,2 means left. 3,4 means right. 5 means down
        int direction = randy.Next(1,6);
        if (direction == 1 || direction == 2)
        {
            // if left is not the edge of the map AND we haven't been there yet, we are good
            if (col - 1 >= 0 && map[row, col-1].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {
                LabelRoomNum(MOVING_TO.LEFT);
                col -= 1;
                return false;
            }
            // if left is an edge or already visited, we fall through and go down (lol)
        } 
        if (direction == 3 || direction == 4)
        {
            // if right is not the edge of the map AND we haven't been there yet, we are good
            if (col + 1 < map.GetLength(0) && map[row, col+1].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {
                LabelRoomNum(MOVING_TO.RIGHT);
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
            LabelRoomNum(MOVING_TO.LEFT);
            // store exit column
            exitCol = col;
            return true;
        } else
        {
          // standard case, actually move down a floor
          LabelRoomNum(MOVING_TO.BELOW);
          row += 1;
          return false;  
        }
    }

    private void PlaceSpecialRooms()
    {

        
        // place chest rooms
        for (int chestRoom = 0; chestRoom < numChestRooms; chestRoom++)
        {
            // scan map for empty rooms
            List<Vector2Int> unlabeled_rooms = GetUnlabeledRooms();
            // select a random unlabeled room as a chest room
            if (unlabeled_rooms.Count <= 0)
            {
                Debug.Log("Error: No space for all chest rooms");
                return;
            }
            int randomIndex = randy.Next(unlabeled_rooms.Count);
            Vector2Int roomCoords = unlabeled_rooms[randomIndex];
            unlabeled_rooms.RemoveAt(randomIndex);
            row = roomCoords.x;
            col = roomCoords.y;
            MapRoom room = map[row, col];
            room.roomQuality = ROOM_QUALITY.CHEST;
            // random walk a path from that room to any room labeled not by this process
            bool connected = false;
            while (!connected)
            {
                List<int> directions = new();
                for (int i = 0; i < 4; i++)
                {
                    directions.Add(i);
                }
                connected = ConnectedPathfind(directions);
            }
            AgeMap();
        }
         // place NPC rooms
        for (int npcIdx = 0; npcIdx < NPCInstances.Count; npcIdx++)
        {
            List<Vector2Int> unlabeled_rooms = GetUnlabeledRooms();
            // select a random unlabeled room as an NPC room
            if (unlabeled_rooms.Count <= 0)
            {
                Debug.Log("Error: No space for all NPC rooms");
                return;
            }
            int randomIndex = randy.Next(unlabeled_rooms.Count);
            Vector2Int roomCoords = unlabeled_rooms[randomIndex];
            unlabeled_rooms.RemoveAt(randomIndex);
            row = roomCoords.x;
            col = roomCoords.y;
            MapRoom room = map[row, col];
            room.roomQuality = ROOM_QUALITY.NPC;
            room.NPCIdx = npcIdx;
            // random walk a path from that room to any room labeled not by this process
            bool connected = false;
            while (!connected)
            {
                List<int> directions = new();
                for (int i = 0; i < 4; i++)
                {
                    directions.Add(i);
                }
                connected = ConnectedPathfind(directions);
            }
            AgeMap();
        }
    }

    private List<Vector2Int> GetUnlabeledRooms()
    {
        List<Vector2Int> unlabeled_rooms = new();
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col].roomStyle == ROOM_STYLE.UNSPECIFIED)
                {
                    unlabeled_rooms.Add(new Vector2Int(row, col));
                }
               
            }
        }
        return unlabeled_rooms;
    }

    // a variation of pathfinding adapted for branches rather than the main path
    private bool ConnectedPathfind(List<int> directions)
    {
        // Randomly pick 0-3
        // 0 means up, 1 means right, 2 means down, 3 means left
        int random_index = randy.Next(directions.Count);
        int direction = directions[random_index];
        if (direction == 0)
        {
            // if up is off the map or already a part of this path, remove direction and retry
            if (row - 1 < 0 || map[row - 1, col].recent)
            {
                directions.RemoveAt(random_index);
                return ConnectedPathfind(directions);
            }
            // we're going up
            LabelRoomNum(MOVING_TO.UP);
            row -= 1;
            // if up is an unlabeled room, label it and continue
            if (map[row, col].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {

                return false;
            }
            // if up is an already placed path, we have found our destination
            // so ensure that the destination knows to change itself
            LabelRoomNum(MOVING_TO.BELOW);
            return true;
        }
        else if (direction == 2)
        {
            // if below is off the map or already a part of this path, remove direction and retry
            if (row + 1 >= mapDimensions || map[row + 1, col].recent)
            {
                directions.RemoveAt(random_index);
                return ConnectedPathfind(directions);
            }
            // we're going down
            LabelRoomNum(MOVING_TO.BELOW);
            row += 1;
            // if up is an unlabeled room, label it and continue
            if (map[row, col].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {
                return false;
            }
            // if down is an already placed path, we have found our destination
            // so ensure that the destination knows to change itself
            LabelRoomNum(MOVING_TO.UP);
            return true;
        } 
        else if (direction == 1)
        {
            // if right is off the map or already a part of this path, remove direction and retry
            if (col + 1 >= mapDimensions || map[row, col + 1].recent)
            {
                directions.RemoveAt(random_index);
                return ConnectedPathfind(directions);
            }
            // we're going right
            LabelRoomNum(MOVING_TO.RIGHT);
            col += 1;
            // if right is an unlabeled room, label it and continue
            if (map[row, col].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {
                return false;
            }
            // if right is an already placed path, we have found our destination
            // so ensure that the destination knows to change itself
            LabelRoomNum(MOVING_TO.LEFT);
            return true;
        } 
        else if (direction == 3)
        {
            // if left is off the map or already a part of this path, remove direction and retry
            if (col - 1 < 0 || map[row, col - 1].recent)
            {
                directions.RemoveAt(random_index);
                return ConnectedPathfind(directions);
            }
            // we're going left
            LabelRoomNum(MOVING_TO.LEFT);
            col -= 1;
            // if left is an unlabeled room, label it and continue
            if (map[row, col].roomStyle == ROOM_STYLE.UNSPECIFIED)
            {
                return false;
            }
            // if left is an already placed path, we have found our destination
            // so ensure that the destination knows to change itself
            LabelRoomNum(MOVING_TO.RIGHT);
            return true;
        } else
        {
            Debug.Log("ERROR: Non-orthogonal direction chosen");
            return true;
        }
        
    }

    // Room type 0 has no guaranteed exits
    // Room type 1 has exits on the left and right guaranteed
    // Room type 2 has exits on the left, right, and bottom (we love the Oxford comma)
    // Room type 3 has exits on the left, right, and top
    // Room type 4 has exits on the left, right, top, and bottom
    private void LabelRoomNum(MOVING_TO direction)
    {
        // if we are moving up, then we are either type 3 or 4
        if (direction == MOVING_TO.UP)
        {
            if (row + 1 >= mapDimensions)
            {
                map[row, col].roomStyle = ROOM_STYLE.TOP_LEFT_RIGHT;
                return;
            }
            MapRoom room = map[row + 1, col];
            // if below us is offscreen, or a room of type 0, 1 or 2, then we are 3
            if (room.roomStyle == ROOM_STYLE.UNSPECIFIED || room.roomStyle == ROOM_STYLE.LEFT_RIGHT || room.roomStyle == ROOM_STYLE.BOTTOM_LEFT_RIGHT)
            {
                map[row, col].roomStyle = ROOM_STYLE.TOP_LEFT_RIGHT;
            } else
            {
                map[row, col].roomStyle = ROOM_STYLE.CROSS;
            }
        }
        // if we are moving left or right, then we are either 1, 2 or 3.
        // if the room above us is offscreen, or is of type 0, 1, or 3, then we are type 1 or 2.
        //  if the room below is type 3 or 4, we are type 2, otherwise type 1
        // otherwise the room above us is type 2 or 4 which means we need a top exit and are type 3.
        if (direction == MOVING_TO.LEFT || direction == MOVING_TO.RIGHT)
        {
            MapRoom room;
            // nothing above us
            if (row - 1 < 0)
            {
                // check below. Shouldn't need to bounds check
                room = map[row + 1, col];
                // connection below, we are type 2
                if (room.roomStyle == ROOM_STYLE.TOP_LEFT_RIGHT || room.roomStyle == ROOM_STYLE.CROSS)
                {
                    map[row, col].roomStyle = ROOM_STYLE.BOTTOM_LEFT_RIGHT;
                } else
                {
                    map[row, col].roomStyle = ROOM_STYLE.LEFT_RIGHT;
                }
                return;
            }
            // there is a room above
            room = map[row - 1, col];
            if (room.roomStyle == ROOM_STYLE.UNSPECIFIED  || room.roomStyle == ROOM_STYLE.LEFT_RIGHT || room.roomStyle == ROOM_STYLE.TOP_LEFT_RIGHT)
            {
                if (row + 1 >= mapDimensions)
                {
                    map[row, col].roomStyle = ROOM_STYLE.LEFT_RIGHT;
                } else
                {
                    room = map[row + 1, col];
                    if (room.roomStyle == ROOM_STYLE.TOP_LEFT_RIGHT || room.roomStyle == ROOM_STYLE.CROSS)
                    {
                        map[row, col].roomStyle = ROOM_STYLE.BOTTOM_LEFT_RIGHT;
                    } else
                    {
                        map[row, col].roomStyle = ROOM_STYLE.LEFT_RIGHT;
                    }
                }
                
            } else
            {
                map[row, col].roomStyle = ROOM_STYLE.TOP_LEFT_RIGHT;
            }

        }
        // otherwise, we are moving down
        // if the room above us is offscreen, or is of type 0, 1, or 3, then we are type 2.
        // otherwise the room above us is type 2 or 4 which means we need a top exit and are type 4.
        else
        {
            if (row - 1 < 0)
            {
                map[row, col].roomStyle = ROOM_STYLE.BOTTOM_LEFT_RIGHT;
                return;
            }
            MapRoom room = map[row - 1, col];
            if (room.roomStyle == ROOM_STYLE.UNSPECIFIED || room.roomStyle == ROOM_STYLE.LEFT_RIGHT || room.roomStyle == ROOM_STYLE.TOP_LEFT_RIGHT)
            {
                map[row, col].roomStyle = ROOM_STYLE.BOTTOM_LEFT_RIGHT;
            } else
            {
                map[row, col].roomStyle = ROOM_STYLE.CROSS;
            }
        }
    }

    // scans map and removes the recent status from all rooms
    // to be called after a path is finished being created
    private void AgeMap()
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
               if (map[row, col].recent)
                {
                    map[row, col].recent = false;
                }
            }
        }
    }

    
    private void PlaceMap()
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
                    InstantiateRoom(filledRoom, x, y, ROOM_QUALITY.REGULAR, -1);
                } else // normal room creation
                {
                    MapRoom room = map[row,col];
                    // check for starting and ending rooms
                    if (row == 0 && col == entranceCol) room.roomQuality = ROOM_QUALITY.STARTING;
                    if (row == (mapDimensions - 1) && col == exitCol) room.roomQuality = ROOM_QUALITY.ENDING;
                    if (room.roomQuality == ROOM_QUALITY.NPC)
                    {
                        // hack fix since InstantiateRoom expects an array
                        Sprite[] roomArray = new Sprite[1];
                        // may have to cast
                        roomArray[0] = NPCInstances[room.NPCIdx].room;
                        InstantiateRoom(roomArray, x, y, room.roomQuality, room.NPCIdx);
                    } else
                    {
                        ROOM_STYLE roomNum = room.roomStyle;
                        switch (roomNum)
                        {
                            case ROOM_STYLE.LEFT_RIGHT: InstantiateRoom(room1s, x, y, room.roomQuality, -1); break;
                            case ROOM_STYLE.BOTTOM_LEFT_RIGHT: InstantiateRoom(room2s, x, y, room.roomQuality, -1); break;
                            case ROOM_STYLE.TOP_LEFT_RIGHT: InstantiateRoom(room3s, x, y, room.roomQuality, -1); break;
                            case ROOM_STYLE.CROSS: InstantiateRoom(room4s, x, y, room.roomQuality, -1); break;
                            //case 5: specialRoomCoords.Add((x,y)); break;
                            default: InstantiateRoom(room0s, x, y, room.roomQuality, -1); break;
                        } 
                    }
                    
                }
                
                x += roomDimensions;
            }
            x = 0;
            y -= roomDimensions;
        }
        //InstantiateSpecialRooms(numChestRooms);
    }

    // private void InstantiateSpecialRooms(int numChestRooms)
    // {
    //     // instantiate chest rooms
    //     for (int room = 0; room < numChestRooms; room++)
    //     {
    //         int randIdx = randy.Next(0, specialRoomCoords.Count);
    //         (int x, int y) = specialRoomCoords[randIdx];
    //         specialRoomCoords.RemoveAt(randIdx);
    //         InstantiateRoom(chestRoom, x,  y, false, false, -1);
    //     }
    //     // instantiate other special rooms
    //     for (int npc_idx = 0; npc_idx < NPCInstances.Count; npc_idx++)
    //     {
    //         int randIdx = randy.Next(0, specialRoomCoords.Count);
    //         // BUG: randIdx is out of bounds sometimes
    //         (int x, int y) = specialRoomCoords[randIdx];
    //         specialRoomCoords.RemoveAt(randIdx);
    //         Sprite[] temp_arr = new Sprite[1];
    //         // may have to cast
    //         temp_arr[0] = NPCInstances[npc_idx].room;
    //         InstantiateRoom(temp_arr, x,  y, false, false, npc_idx);
    //     }
    //     // instantiate all remaining marked rooms as 0s
    //     for (int room = 0; room < specialRoomCoords.Count; room++)
    //     {
    //         (int x, int y) = specialRoomCoords[room];
    //         InstantiateRoom(room0s, x,  y, false, false, -1);
    //     }
    // }

    void InstantiateRoom(Sprite[] rooms, int x, int y, ROOM_QUALITY room_quality, int specialIdx)
    {
        template = rooms[randy.Next(rooms.Length)];
        Color32[] pixels = ConvertSpriteToPixelArray(template);
        int[] room = TranslateColorsToProbabilities(pixels, room_quality);
        GenerateRoom(room, x, y, room_quality, specialIdx);
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
                //I wonder if we can convert the color into a unique integer
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
                } else if (color.Equals(decorationColor))
                {
                    roomProbs[row * roomDimensions + col] = -44;
                } else if (color.Equals(torchColor))
                {
                    roomProbs[row * roomDimensions + col] = -77;
                } else if (color.Equals(chestColor))
                {
                    roomProbs[row * roomDimensions + col] = -66;
                } else if (color.Equals(enemyColor))
                {
                    roomProbs[row * roomDimensions + col] = -33;
                } else if (color.Equals(specialEnemyColor))
                {
                    roomProbs[row * roomDimensions + col] = -34;
                } else if (color.Equals(specialItemColor))
                {
                    roomProbs[row * roomDimensions + col] = -35;
                } else if (color.Equals(NPCSpawnColor))
                {
                    roomProbs[row * roomDimensions + col] = -36;
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


    void GenerateRoom(int[] room, int xOffset, int yOffset, ROOM_QUALITY roomQuality, int specialIdx)
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
                    // this can be simplified I believe
                    
                    if (!startingPositionAssigned)
                    {
                        nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), entryDoor);
                        startingPosition = new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset);
                        startingPositionAssigned = true;
                    } else
                    {
                        nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.exitDoor);
                        exitPosition = new Vector2(xCoord, yCoord);
                    }
                }
                // check for special value indicating spikes
                else if (roomProbability == -75 || roomProbability == -25){
                    if (randy.Next(0,100) < 25)
                    {
                        nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.spikes);
                    } else if (roomProbability == -75)
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.ruleTile);
                    }
                }
                // check for special value indicating a rope
                else if (roomProbability == -44)
                {
                    //rope.strategy.UsePlaceableConsumable(nonColliderTilemap.transform, new Vector3(xCoord + startingPositionOffset, yCoord + startingPositionOffset, 0));
                    ropeCoords.Add(new Vector2(xCoord, yCoord));
                }
                 // check for special value indicating a chest
                else if (roomProbability == -66)
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), chest);
                    //TODO: Investigate why this is necessary
                    emptyFloorSpaces.Remove((xCoord, yCoord));
                }
                // check for special value indicating a torch
                else if (roomProbability == -77)
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), torch);
                }
                // check for special value indicating an enemy spawn
                if (roomProbability == -33)
                {
                    if (randy.Next(0,100) < 33)
                    {
                        Instantiate(enemy, new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset), Quaternion.identity);
                    }
                }
                else if (roomProbability == -34)
                {
                    GameObject instance = Instantiate(NPCInstances[specialIdx].specialEnemy, new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset), Quaternion.identity);
                    NPCInstances[specialIdx].specialEnemies.Add(instance);
                }
                //special items (nonCollider Tiles)
                else if (roomProbability == -35)
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), NPCInstances[specialIdx].specialObject);
                    //Instantiate(NPCInstances[specialIdx].specialObject, new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset), Quaternion.identity);
                }
                // NPC Spawn value
                else if (roomProbability == -36)
                {
                    NPCInstances[specialIdx].spawnPoint = new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset);
                }
                // check for special value indicating false floor
                else if (roomProbability == -88)
                {
                    colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.falseFloor);
                }
                // check for special value indicating flamethrower
                else if (roomProbability == -55){
                    if (randy.Next(0,100) < 25)
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), flamethrower);
                    } else
                    {
                        colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.ruleTile);
                    }
                }
                else if (randy.Next(0,100) < roomProbability)
                {
                    colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.ruleTile);
                }
            }
        }
        if (roomQuality == ROOM_QUALITY.CHEST)
        {
            //Debug.Log("Looking for chest spot...");
            List<Vector3Int> possibleChestSpots = new();
            // ignore bottom row
            for (int row = 0; row < roomDimensions - 1; row++)
            {
                for (int col = 0; col < roomDimensions; col++)
                {
                    
                    int xCoord = col + xOffset;
                    int yCoord = row + yOffset;
                    Vector3Int coords = new(xCoord, yCoord, 0);
                    bool self = colliderTilemap.HasTile(coords); 
                    bool below = colliderTilemap.HasTile(new(xCoord, yCoord - 1, 0));
                    if (!self && below)
                    {
                        possibleChestSpots.Add(coords);
                    }
                }
            }
            if (possibleChestSpots.Count <= 0)
            {
                Debug.Log("Error: No space for a chest");
            } else
            {
                Vector3Int randCoords = possibleChestSpots[randy.Next(possibleChestSpots.Count)];
                nonColliderTilemap.SetTile(randCoords, chest);
                //Debug.Log("Placing Chest at " + randCoords);
            }
        }
    }

    void PlaceRopes()
    {
        foreach (Vector2 ropeCoord in ropeCoords)
        {
            rope.strategy.UsePlaceableConsumable(nonColliderTilemap.transform, new Vector3(ropeCoord.x, ropeCoord.y, 0));
        }
    }

    void GatherTrapTileInfo(Trap trap)
    {
        foreach (Vector3Int tileCoords in colliderTilemap.cellBounds.allPositionsWithin)
        {
            TileBase currTile = colliderTilemap.GetTile(tileCoords);
            if (trap.CheckIfValidPosition(currTile, tileCoords, colliderTilemap, nonColliderTilemap))
            {
                trap.spawnPositions.Add(new (tileCoords.x, tileCoords.y));
            }
        }
    }

    void GatherEnemyTileInfo(EnemyInfo enemy)
    {
        foreach (Vector3Int tileCoords in colliderTilemap.cellBounds.allPositionsWithin)
        {
            TileBase currTile = colliderTilemap.GetTile(tileCoords);
            if (enemy.CheckSpawnPosition(currTile, tileCoords, colliderTilemap, nonColliderTilemap))
            {
                //Vector3 pos = colliderTilemap.CellToWorld(tileCoords);
                //print("World Pos: " + pos.x + " " + pos.y + " Cell Pos: " + tileCoords.x + " " + tileCoords.y);
                enemy.spawnPositions.Add(new (tileCoords.x, tileCoords.y));
            }
        }
    }

    void GatherDecorationTileInfo()
    {
        foreach (Vector3Int tileCoords in colliderTilemap.cellBounds.allPositionsWithin)
        {
            // We ignore the top row of tiles for obtaining floor tiles
            if (tileCoords.y != 0)
            {
                Vector3Int below = new(tileCoords.x, tileCoords.y - 1, 0); 
                // if the space is blank and there is a solid tile beneath it, record it as a floor tile
                if (!colliderTilemap.HasTile(tileCoords) && !nonColliderTilemap.HasTile(tileCoords) && colliderTilemap.HasTile(below))
                {
                    emptyFloorSpaces.Add(new (tileCoords.x, tileCoords.y));
                }
            }
            // We ignore the bottom row of tiles for obtaining ceiling tiles
            if (tileCoords.y != colliderTilemap.cellBounds.yMax)
            {
                Vector3Int above = new(tileCoords.x, tileCoords.y + 1, 0); 
                // if the space is blank and there is a solid tile above it, record it as a ceiling tile
                if (!colliderTilemap.HasTile(tileCoords) && !nonColliderTilemap.HasTile(tileCoords) && colliderTilemap.HasTile(above))
                {
                    emptyCeilingSpaces.Add(new (tileCoords.x, tileCoords.y));
                }
            }
        }
    }

    void PlaceEntities()
    {
        // sanity check
        // if (emptyFloorSpaces.Capacity == 0 || emptyCeilingSpaces.Capacity == 0)
        // {
        //     print("ERROR:No floor spaces found");
        //     return;
        // }
        foreach (Trap trap in levelData.traps)
        {
            GatherTrapTileInfo(trap);
            // -1 since arrays are 0-based and our levels are 1-based
            int numTraps = Mathf.FloorToInt(trap.spawnPositions.Count * trap.levelSpawnRates[level - 1]);
            for (int trapNum = 0; trapNum < numTraps; trapNum++)
            {
                int randIdx = randy.Next(0, trap.spawnPositions.Count);
                (int xCoord, int yCoord) = trap.spawnPositions[randIdx];
                // ensures no repeats
                trap.spawnPositions.RemoveAt(randIdx);
                if (trap.isCollider)
                {
                    colliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), trap.trapTile);
                } else
                {
                    nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), trap.trapTile);
                }
                
            }
        }
        // after traps have been placed, scan again
        GatherDecorationTileInfo();
        //Loop through NPCS. and place them at their spawn point. If none, spawn at a random floor tile.
        for (int npcIdx = 0; npcIdx < NPCInstances.Count; npcIdx++)
        {
            NPC npc = NPCInstances[npcIdx];
            if (npc.spawnPoint.x == 0 && npc.spawnPoint.y == 0)
            {
                int randIdx = randy.Next(0, emptyFloorSpaces.Count);
                (int xCoord, int yCoord) = emptyFloorSpaces[randIdx];
                emptyFloorSpaces.RemoveAt(randIdx);
                npc.transform.position = new Vector2(xCoord + startingPositionOffset, yCoord + startingPositionOffset);
            } else
            {
                npc.transform.position = npc.spawnPoint;
            }
        }
        // place enemies
        for (int enemyIdx = 0; enemyIdx < levelData.enemies.Count; enemyIdx++)
        {
            EnemyInfo enemy = levelData.enemies[enemyIdx];
            GatherEnemyTileInfo(enemy);
            for (int numberSpawned = 0; numberSpawned < levelData.enemySpawnCounts[enemyIdx]; numberSpawned++)
            {
                // if there are valid positions to spawn at
                if (enemy.spawnPositions.Count > 0)
                {
                    int randIdx = randy.Next(0, enemy.spawnPositions.Count);
                    //StartCoroutine(SpawnEnemy(enemy, randIdx));
                    Instantiate(enemy.enemyPrefab, new Vector2(enemy.spawnPositions[randIdx].x  + startingPositionOffset, enemy.spawnPositions[randIdx].y  + startingPositionOffset),
                                quaternion.identity);
                    enemy.spawnPositions.RemoveAt(randIdx);
                } else
                {
                    // pretend all have been spawned
                    numberSpawned = levelData.enemySpawnCounts[enemyIdx];
                }
            }
        }
        // place decorations
        for (int decNum = 0; decNum < levelData.numDecorations; decNum++)
        {
            // 75% of decorations will be floor decorations, the rest will be ceiling decor
            if (randy.Next(0,100) < 75)
            {
                int randIdx = randy.Next(0, emptyFloorSpaces.Count);
                (int xCoord, int yCoord) = emptyFloorSpaces[randIdx];
                // ensures no repeats
                emptyFloorSpaces.RemoveAt(randIdx);
                nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.GetRandomDecoration(false));
            } else
            {
                int randIdx = randy.Next(0, emptyCeilingSpaces.Count);
                (int xCoord, int yCoord) = emptyCeilingSpaces[randIdx];
                // ensures no repeats
                emptyCeilingSpaces.RemoveAt(randIdx);
                nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), levelData.GetRandomDecoration(true));
            }
            
        }
        
    }

    IEnumerator SpawnEnemy(EnemyInfo enemy, int randIdx)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Instantiate(enemy.enemyPrefab, new Vector2(enemy.spawnPositions[randIdx].x, enemy.spawnPositions[randIdx].y), quaternion.identity);
    }

    void DebugPrintMap()
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            StringBuilder stringBuilder = new();
            for (int col = 0; col < map.GetLength(1); col++)
            {
                string room_symbol = "";
                ROOM_STYLE mapRoomStyle = map[row, col].roomStyle;
                
                if (map[row, col].roomQuality == ROOM_QUALITY.STARTING)
                {
                    room_symbol = "S";
                } else if (map[row, col].roomQuality == ROOM_QUALITY.ENDING)
                {
                    room_symbol = "E";
                }
                
                else if (map[row, col].roomQuality == ROOM_QUALITY.NPC) {
                    room_symbol = "N";
                }
                
                else
                {
                    switch (mapRoomStyle)
                    {
                        case ROOM_STYLE.UNSPECIFIED: room_symbol = "0"; break;
                        case ROOM_STYLE.LEFT_RIGHT: room_symbol = "-"; break;
                        case ROOM_STYLE.BOTTOM_LEFT_RIGHT: room_symbol = "T"; break;
                        case ROOM_STYLE.TOP_LEFT_RIGHT: room_symbol = "-L"; break;
                        case ROOM_STYLE.CROSS: room_symbol = "+"; break;
                    }
                }
                stringBuilder.Append(" [ " + room_symbol  +" ] ");
            }
            Debug.Log(stringBuilder);
        }
    }

    class MapRoom
    {
        public ROOM_QUALITY roomQuality = ROOM_QUALITY.REGULAR;
        public ROOM_STYLE roomStyle = ROOM_STYLE.UNSPECIFIED;
        public bool recent = true;
        public int NPCIdx = -1;

        public MapRoom()
        {
            roomQuality = ROOM_QUALITY.REGULAR;
            roomStyle = ROOM_STYLE.UNSPECIFIED;
            recent = true;
            NPCIdx = -1;
        }
    }
}
