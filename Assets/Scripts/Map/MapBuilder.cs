using UnityEngine;

// Generates a simple ship layout with walls, rooms, and a floor.
// All done in code so we don't need to manually place 50+ objects in the Editor.
// In Phase 4 we'll replace this with a proper Tilemap, but this is great for prototyping.
public class MapBuilder : MonoBehaviour
{
    // Colors for the map elements.
    private Color wallColor = new Color(0.35f, 0.25f, 0.15f);   // Dark brown (wood)
    private Color floorColor = new Color(0.55f, 0.40f, 0.25f);  // Light brown (deck)
    private Color bgColor = new Color(0.15f, 0.35f, 0.55f);     // Ocean blue

    // Ship dimensions.
    private const float SHIP_WIDTH = 24f;
    private const float SHIP_HEIGHT = 14f;
    private const float WALL_THICKNESS = 0.4f;

    void Start()
    {
        BuildShip();
    }

    private void BuildShip()
    {
        // Create a parent object to keep the hierarchy clean.
        GameObject ship = new GameObject("Ship");

        // --- OCEAN BACKGROUND ---
        // A large blue square behind everything.
        CreateSprite("Ocean", new Vector2(0, 0), new Vector2(50, 50), bgColor, ship.transform, -2);

        // --- FLOOR ---
        // The deck of the ship — slightly smaller than the walls.
        CreateSprite("Floor", new Vector2(0, 0), new Vector2(SHIP_WIDTH, SHIP_HEIGHT), floorColor, ship.transform, -1);

        // --- OUTER WALLS ---
        // Four walls forming the ship's hull. Each wall is a stretched sprite
        // with a BoxCollider2D so the player can't walk through.
        float halfW = SHIP_WIDTH / 2f;
        float halfH = SHIP_HEIGHT / 2f;

        // Top wall
        CreateWall("Wall_Top", new Vector2(0, halfH), new Vector2(SHIP_WIDTH + WALL_THICKNESS, WALL_THICKNESS), ship.transform);
        // Bottom wall
        CreateWall("Wall_Bottom", new Vector2(0, -halfH), new Vector2(SHIP_WIDTH + WALL_THICKNESS, WALL_THICKNESS), ship.transform);
        // Left wall
        CreateWall("Wall_Left", new Vector2(-halfW, 0), new Vector2(WALL_THICKNESS, SHIP_HEIGHT + WALL_THICKNESS), ship.transform);
        // Right wall
        CreateWall("Wall_Right", new Vector2(halfW, 0), new Vector2(WALL_THICKNESS, SHIP_HEIGHT + WALL_THICKNESS), ship.transform);

        // --- INTERIOR WALLS ---
        // Vertical wall dividing left and right halves.
        // Gap in the middle for a doorway (corridor).
        // Top section of center wall
        CreateWall("Wall_Center_Top", new Vector2(0, 4.5f), new Vector2(WALL_THICKNESS, 5f), ship.transform);
        // Bottom section of center wall
        CreateWall("Wall_Center_Bottom", new Vector2(0, -4.5f), new Vector2(WALL_THICKNESS, 5f), ship.transform);

        // Horizontal wall dividing top and bottom rooms — left side.
        // Gap for doorway into corridor.
        CreateWall("Wall_Horiz_Left_L", new Vector2(-8.5f, 0), new Vector2(7f, WALL_THICKNESS), ship.transform);
        CreateWall("Wall_Horiz_Left_R", new Vector2(-3.5f, 0), new Vector2(3f, WALL_THICKNESS), ship.transform);

        // Horizontal wall dividing top and bottom rooms — right side.
        CreateWall("Wall_Horiz_Right_L", new Vector2(3.5f, 0), new Vector2(3f, WALL_THICKNESS), ship.transform);
        CreateWall("Wall_Horiz_Right_R", new Vector2(8.5f, 0), new Vector2(7f, WALL_THICKNESS), ship.transform);

        // --- ROOM LABELS ---
        // Simple text labels so you can tell rooms apart during testing.
        CreateLabel("Upper Left Room", new Vector2(-6f, 4f), ship.transform);
        CreateLabel("Upper Right Room", new Vector2(6f, 4f), ship.transform);
        CreateLabel("Lower Left Room", new Vector2(-6f, -4f), ship.transform);
        CreateLabel("Lower Right Room", new Vector2(6f, -4f), ship.transform);
        CreateLabel("Corridor", new Vector2(0f, 0f), ship.transform);
    }

    // Creates a wall: a colored sprite with a BoxCollider2D.
    private void CreateWall(string name, Vector2 position, Vector2 size, Transform parent)
    {
        GameObject wall = CreateSprite(name, position, size, wallColor, parent, 0);

        // Add a BoxCollider2D so the player's Rigidbody2D can't pass through.
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        // The collider auto-sizes to the sprite, so no extra config needed.
    }

    // Creates a colored sprite (used for floors, walls, background).
    private GameObject CreateSprite(string name, Vector2 position, Vector2 size, Color color, Transform parent, int sortingOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = parent;
        obj.transform.position = position;

        // SpriteRenderer draws the visual. We use Unity's built-in white square
        // sprite and tint it with color — this way we don't need any art assets.
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSquareSprite();
        sr.color = color;
        sr.sortingOrder = sortingOrder;

        // Scale the object to the desired size.
        obj.transform.localScale = new Vector3(size.x, size.y, 1f);

        return obj;
    }

    // Creates a text label in the world (for debugging/prototyping).
    private void CreateLabel(string text, Vector2 position, Transform parent)
    {
        GameObject obj = new GameObject($"Label_{text}");
        obj.transform.parent = parent;
        obj.transform.position = position;

        // TextMesh is a simple built-in text component (no TMP dependency needed here).
        TextMesh tm = obj.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 24;
        tm.characterSize = 0.15f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 1f, 0.3f); // Subtle white text
    }

    // Creates a 1x1 white sprite at runtime. We cache it so we only make one.
    private static Sprite cachedSprite;
    private Sprite MakeSquareSprite()
    {
        if (cachedSprite != null) return cachedSprite;

        // Create a tiny 4x4 white texture and convert it to a sprite.
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();

        cachedSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
        return cachedSprite;
    }
}
