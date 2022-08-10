using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    BoxCollider2D Box;
    Vector2 Size;
    public LayerMask GroundMask;

    private void Awake()
    {
        Box = GetComponent<BoxCollider2D>();
        Size = Box.size;
        Destroy(Box);
    }

    public bool CheckForGround()
    {
        return Physics2D.OverlapBox(transform.position, Size, 0, GroundMask);
    }
}
