using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");

    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody.isKinematic)
        {
            return false;
        }

        float radius = 0.25f; //0.25
        float distance = 0.375f; //0.375

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction, distance, layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection) //transform = mario, other = object
    {
        Vector2 direction = other.position - transform.position; // 1 = same, -1 != same, 0 = ?
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }
}
