using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    [Header("Floor Detection")]
    public float floorCheckRadius; // how large the detection for the floors is
    public float bottomOffset; // offset from player center

    [Header("Roof detection")]
    public float roofCheckRadius; // the amount we check before standing up
    public float upOffset; //offset upwards

    [Header("Detection Layers")]
    public LayerMask floorLayers; //what layer player can stand on
    public LayerMask roofLayers; //what layers cannot stand up under

    public bool CheckFloor(Vector3 direction)
    {
        Vector3 pos = transform.position + (direction * bottomOffset);
        Collider[] colliderHit = Physics.OverlapSphere(pos, floorCheckRadius, floorLayers);
        if (colliderHit.Length > 0)
        {
            // ground below player
            return true;
        }

        return false;
    }

    public bool CheckRoof(Vector3 direction)
    {
        Vector3 pos = transform.position + (direction * upOffset);
        Collider[] colliderHit = Physics.OverlapSphere(pos, roofCheckRadius, roofLayers);
        if (colliderHit.Length > 0)
        {
            // roof above player
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Floor
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(position, floorCheckRadius);

        // Roof
        Gizmos.color = Color.cyan;
        position = transform.position + (transform.up * upOffset);
        Gizmos.DrawSphere(position, roofCheckRadius);
    }
}
