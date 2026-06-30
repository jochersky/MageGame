using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int mapDimensions = 5;
    [SerializeField] int roomDimensions = 8; 
    [SerializeField] int level = 1;
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
    [SerializeField] RuleTile tile;
    [SerializeField] TileBase entryDoor;
    [SerializeField] TileBase exitDoor;
    [SerializeField] TileBase spikes;
    [SerializeField] TileBase falseFloor;
    [SerializeField] TileBase flamethrower;
    [SerializeField] TileBase torch;
    [SerializeField] TileBase chest;
    [SerializeField] Tilemap colliderTilemap;
    [SerializeField] Tilemap nonColliderTilemap;

    [SerializeField] Decorations decorations;
    [SerializeField] int numDecorations = 100;
    Trap[] trapPrefabs;
    readonly string trapPath = "Traps/";
    
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
    [SerializeField] GameObject player;
    [SerializeField] float startingPositionOffset;
    Vector2 startingPosition;
    bool startingPositionAssigned = false;
    Vector2 exitPosition;

    int numChestRooms = 3;
    List<(int x, int y)> specialRoomCoords = new();

    
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
        chestRoom = Resources.LoadAll<Sprite>("Rooms/Chest Room");
        room0s = Resources.LoadAll<Sprite>("Rooms/Room Style 0");
        room1s = Resources.LoadAll<Sprite>("Rooms/Room Style 1");
        room2s = Resources.LoadAll<Sprite>("Rooms/Room Style 2");
        room3s = Resources.LoadAll<Sprite>("Rooms/Room Style 3");
        room4s = Resources.LoadAll<Sprite>("Rooms/Room Style 4");
        NPCPrefabs = Resources.LoadAll<NPC>(NPCpath);
        trapPrefabs = Resources.LoadAll<Trap>(trapPath);
        randy = new System.Random();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new int[mapDimensions, mapDimensions];
        SpawnEntities();
        GenRoomPaths();
        PlaceMap();
        PlaceEntities();
        // teleport player to starting position
        player.transform.position = startingPosition;
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
            foundExit = Pathfind();
        }    
        Debug.Log("Generation complete!");
    
        // Potentially print array here for debugging porpoises

    }

    private bool Pathfind()
    {
        // Randomly pick 1-5
        // 1,2 means left. 3,4 means right. 5 means down
        int direction = randy.Next(1,6);
        if (direction == 1 || direction == 2)
        {
            // if left is not the edge of the map AND we haven't been there yet, we are good
            if (col - 1 >= 0 && map[row, col-1] == 0)
            {
                LabelRoomNum(MOVING_TO.LEFT);
                MarkPotentialChestRooms(MOVING_TO.LEFT);
                col -= 1;
                return false;
            }
            // if left is an edge or already visited, we fall through and go down (lol)
        } 
        if (direction == 3 || direction == 4)
        {
            // if right is not the edge of the map AND we haven't been there yet, we are good
            if (col + 1 < map.GetLength(0) && map[row, col+1] == 0)
            {
                LabelRoomNum(MOVING_TO.RIGHT);
                MarkPotentialChestRooms(MOVING_TO.RIGHT);
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
            // for the purposes of chest rooms, we are moving down
            MarkPotentialChestRooms(MOVING_TO.BELOW);
            // store exit column
            exitCol = col;
            return true;
        } else
        {
          // standard case, actually move down a floor
          LabelRoomNum(MOVING_TO.BELOW);
          MarkPotentialChestRooms(MOVING_TO.BELOW);
          row += 1;
          return false;  
        }
    }

    private void MarkPotentialChestRooms(MOVING_TO direction)
    {
        // if we are moving left or down
        if (direction == MOVING_TO.LEFT || direction == MOVING_TO.BELOW)
        {
            // and there exists a room to our right that is unvisited
            if (col + 1 < map.GetLength(0) && map[row, col+1] == 0)
            {
                // mark it as a potential chest room
                map[row, col + 1] = 5;
            }
            
        }
        // if we are moving right or down
        if (direction == MOVING_TO.RIGHT || direction == MOVING_TO.BELOW)
        {
            // and there exists a room to our left that is unvisited
            if (col - 1 >= 0 && map[row, col-1] == 0)
            {
                // mark it as a potential chest room
                map[row, col - 1] = 5;
            }
            
        }
    }

    // Room type 0 has no guaranteed exits
    // Room type 1 has exits on the left and right guaranteed
    // Room type 2 has exits on the left, right, and bottom (we love the Oxford comma)
    // Room type 3 has exits on the left, right, and top
    // Room type 4 has exits on the left, right, top, and bottom
    private void LabelRoomNum(MOVING_TO direction)
    {
        // if we are moving left or right, then we are either 1 or 3.
        // if the room above us is offscreen, or is of type 0, 1, or 3, then we are type 1.
        // otherwise the room above us is type 2 or 4 which means we need a top exit and are type 3.
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
        // otherwise the room above us is type 2 or 4 which means we need a top exit and are type 4.
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

    
    private void PlaceMap()
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
                    InstantiateRoom(filledRoom, x, y, isStartingRoom, isEndingRoom, -1);
                } else // normal room creation
                {
                   int roomNum = map[row,col];
                    switch (roomNum)
                    {
                        case 1: InstantiateRoom(room1s, x, y, isStartingRoom, isEndingRoom, -1); break;
                        case 2: InstantiateRoom(room2s, x, y, isStartingRoom, isEndingRoom, -1); break;
                        case 3: InstantiateRoom(room3s, x, y, isStartingRoom, isEndingRoom, -1); break;
                        case 4: InstantiateRoom(room4s, x, y, isStartingRoom, isEndingRoom, -1); break;
                        case 5: specialRoomCoords.Add((x,y)); break;
                        default: InstantiateRoom(room0s, x, y, isStartingRoom, isEndingRoom, -1); break;
                    } 
                }
                
                x += roomDimensions;
            }
            x = 0;
            y -= roomDimensions;
        }
        InstantiateSpecialRooms(numChestRooms);
    }

    private void InstantiateSpecialRooms(int numChestRooms)
    {
        // instantiate chest rooms
        for (int room = 0; room < numChestRooms; room++)
        {
            int randIdx = randy.Next(0, specialRoomCoords.Count);
            (int x, int y) = specialRoomCoords[randIdx];
            specialRoomCoords.RemoveAt(randIdx);
            InstantiateRoom(chestRoom, x,  y, false, false, -1);
        }
        // instantiate other special rooms
        for (int npc_idx = 0; npc_idx < NPCInstances.Count; npc_idx++)
        {
            int randIdx = randy.Next(0, specialRoomCoords.Count);
            // BUG: randIdx is out of bounds sometimes
            (int x, int y) = specialRoomCoords[randIdx];
            specialRoomCoords.RemoveAt(randIdx);
            Sprite[] temp_arr = new Sprite[1];
            // may have to cast
            temp_arr[0] = NPCInstances[npc_idx].room;
            InstantiateRoom(temp_arr, x,  y, false, false, npc_idx);
        }
        // instantiate all remaining marked rooms as 0s
        for (int room = 0; room < specialRoomCoords.Count; room++)
        {
            (int x, int y) = specialRoomCoords[room];
            InstantiateRoom(room0s, x,  y, false, false, -1);
        }
    }

    void InstantiateRoom(Sprite[] rooms, int x, int y, bool isStartingRoom, bool isEndingRoom, int specialIdx)
    {
        ROOM_QUALITY room_quality = ROOM_QUALITY.REGULAR;
        if (isStartingRoom) { room_quality = ROOM_QUALITY.STARTING; }
        if (isEndingRoom) { room_quality = ROOM_QUALITY.ENDING; }
        template = rooms[randy.Next(rooms.Length)];
        Color32[] pixels = ConvertSpriteToPixelArray(template);
        int[] room = TranslateColorsToProbabilities(pixels, room_quality);
        GenerateRoom(room, x, y, specialIdx);
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


    void GenerateRoom(int[] room, int xOffset, int yOffset, int specialIdx)
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
                        nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), exitDoor);
                        exitPosition = new Vector2(xCoord, yCoord);
                    }
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
                // check for special value indicating a decoration
                else if (roomProbability == -44)
                {
                    // TODO: Clean Up
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
                    print(NPCInstances[specialIdx].name + "'s special tile spawned at (" + Convert.ToString(xCoord + startingPositionOffset) + ", " + Convert.ToString(yCoord + startingPositionOffset));
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
        foreach (Trap trap in trapPrefabs)
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
        // after traps have been place, scan again
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
        // place decorations
        for (int decNum = 0; decNum < numDecorations; decNum++)
        {
            // 75% of decorations will be floor decorations, the rest will be ceiling decor
            if (randy.Next(0,100) < 75)
            {
                int randIdx = randy.Next(0, emptyFloorSpaces.Count);
                (int xCoord, int yCoord) = emptyFloorSpaces[randIdx];
                // ensures no repeats
                emptyFloorSpaces.RemoveAt(randIdx);
                nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), decorations.GetRandomDecoration(level, false));
            } else
            {
                int randIdx = randy.Next(0, emptyCeilingSpaces.Count);
                (int xCoord, int yCoord) = emptyCeilingSpaces[randIdx];
                // ensures no repeats
                emptyCeilingSpaces.RemoveAt(randIdx);
                nonColliderTilemap.SetTile(new Vector3Int(xCoord, yCoord, 0), decorations.GetRandomDecoration(level, true));
            }
            
        }
        
    }
}
