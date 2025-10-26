using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 1f;
    public Color gridColor = Color.green;
    public float gridHeight = 0.01f; // slightly above ground

    /* 
    [Header("Tile Highlighting")]
    public GameObject hoverHighlightPrefab;       // Prefab for hovering highlight
    public GameObject clickHighlightPrefab;       // Prefab for clicked destination highlight
    private GameObject hoverHighlightInstance;
    private GameObject clickHighlightInstance;
    */

    private Material lineMaterial;
    // private Camera cam;

    void Awake()
    {
        // cam = Camera.main;
    }

    void Update()
    {
        // HandleMouseHover();
        // HandleMouseClick();
    }

    /* ------------------- HIGHLIGHT LOGIC -------------------
    void HandleMouseHover()
    {
        if (!hoverHighlightPrefab || !cam) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2Int cell = WorldToCell(hit.point);
            Vector3 cellCenter = CellCenter(cell, gridHeight);

            // Create or move the hover highlight
            if (!hoverHighlightInstance)
            {
                hoverHighlightInstance = Instantiate(
                    hoverHighlightPrefab,
                    cellCenter,
                    Quaternion.Euler(90f, 0f, 0f)
                );
                hoverHighlightInstance.transform.localScale = Vector3.one * cellSize;
            }
            else
            {
                hoverHighlightInstance.transform.position = cellCenter;
            }
        }
    }

    void HandleMouseClick()
    {
        if (!clickHighlightPrefab || !cam) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2Int cell = WorldToCell(hit.point);
                Vector3 cellCenter = CellCenter(cell, gridHeight);

                // Create or move the click highlight
                if (!clickHighlightInstance)
                {
                    clickHighlightInstance = Instantiate(
                        clickHighlightPrefab,
                        cellCenter,
                        Quaternion.Euler(90f, 0f, 0f)
                    );
                    clickHighlightInstance.transform.localScale = Vector3.one * cellSize;
                }
                else
                {
                    clickHighlightInstance.transform.position = cellCenter;
                }
            }
        }
    }
    ------------------------------------------------------- */

    // ------------------- GRID MATH -------------------
    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - transform.position;
        int ix = Mathf.FloorToInt(local.x / cellSize);
        int iy = Mathf.FloorToInt(local.z / cellSize);
        return new Vector2Int(ix, iy);
    }

    public Vector3 CellCenter(Vector2Int cell, float y = 0f)
    {
        float cx = (cell.x + 0.5f) * cellSize + transform.position.x;
        float cz = (cell.y + 0.5f) * cellSize + transform.position.z;
        return new Vector3(cx, y, cz);
    }

    // ------------------- GRID RENDER -------------------
    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(gridColor);

        float y = gridHeight;

        for (int x = 0; x <= width; x++)
        {
            float worldX = x * cellSize;
            GL.Vertex3(worldX, y, 0f);
            GL.Vertex3(worldX, y, height * cellSize);
        }

        for (int z = 0; z <= height; z++)
        {
            float worldZ = z * cellSize;
            GL.Vertex3(0f, y, worldZ);
            GL.Vertex3(width * cellSize, y, worldZ);
        }

        GL.End();
        GL.PopMatrix();
    }
}
