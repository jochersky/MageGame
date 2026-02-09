using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] int dimensions = 8;
    [SerializeField] Sprite template;
    [SerializeField] Grid grid;
    [SerializeField] Color32 guaranteeSquareColor;
    [SerializeField] Color32 highProbabilityColor;
    [SerializeField] Color32 lowProbabilityColor;
    [SerializeField] Color32 noSquareColor;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile tile;
    System.Random randy;
    int[] room;
    void Awake()
    {
        randy = new System.Random();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color32[] pixels = ConvertSpriteToPixelArray(template);
        room = TranslateColorsToProbailities(pixels);
        GenerateRoom();
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

    int[] TranslateColorsToProbailities(Color32[] pixels)
    {
        int[] roomProbs = new int[dimensions * dimensions];
        for (int row = 0; row < dimensions; row++)
        {
            for (int col = 0; col < dimensions; col++)
            {
                Color32 color = pixels[row * dimensions + col];
                // maybe switch statements hate me. who knows?
                if (color.Equals(guaranteeSquareColor))
                {
                    roomProbs[row * dimensions + col] = 100;
                } else if (color.Equals(highProbabilityColor))
                {
                    roomProbs[row * dimensions + col] = 75;
                } else if (color.Equals(lowProbabilityColor))
                {
                    roomProbs[row * dimensions + col] = 25;
                } else if (color.Equals(noSquareColor))
                {
                    roomProbs[row * dimensions + col] = 0;
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

    void GenerateRoom()
    {
        for (int row = 0; row < dimensions; row++)
        {
            for (int col = 0; col < dimensions; col++)
            {
                if (randy.Next(0,100) < room[row * dimensions + col])
                {
                    tilemap.SetTile(new Vector3Int(col, row, 0), tile);
                }
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
