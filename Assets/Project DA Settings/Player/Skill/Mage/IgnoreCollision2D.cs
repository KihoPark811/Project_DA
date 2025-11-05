using UnityEngine;

public class IgnoreCollision2D : MonoBehaviour
{
    [Header("Ignore These Layers")]
    public LayerMask ignoreMask;

    void Start()
    {
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol == null) return;

        // 모든 Collider2D 찾아서 비교
        Collider2D[] allCols = FindObjectsOfType<Collider2D>();
        foreach (var col in allCols)
        {
            if (((1 << col.gameObject.layer) & ignoreMask) != 0)
            {
                Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }
}
