using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerGridMover : MonoBehaviour
{
    public float moveSpeed = 3f;
    public LayerMask groundMask;
    public Vector3 visualOffset = new Vector3(0f, 0f, 0f);
    public float yOffset = 0.0f;

    private GridManager grid;
    private Camera cam;
    private Vector3 targetPos;
    private bool moving;

    void Awake()
    {
        grid = FindObjectOfType<GridManager>();
        cam = Camera.main;
        targetPos = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TrySetTargetFromClick();

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if ((transform.position - targetPos).sqrMagnitude < 0.0004f)
                moving = false;
        }
    }

    void TrySetTargetFromClick()
    {
        if (!cam || !grid) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector2Int cell = grid.WorldToCell(hit.point);
            targetPos = grid.CellCenter(cell, hit.point.y) + visualOffset + new Vector3(0f, yOffset, 0f);
            moving = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(targetPos, 0.1f);
    }
}
