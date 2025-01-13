using System;
using UnityEngine;

public class ShowSelectedSpot : MonoBehaviour
{
    private Transform _camera;
    private void Awake()
    {
        _camera = transform.parent.GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        if (Physics.Raycast(_camera.position, _camera.TransformDirection(Vector3.forward), out var hit, 
                5f, LayerMask.GetMask("Card", "Deck", "Table")))
        {
            switch (hit.transform.gameObject.layer) 
            {
                case 3:
                case 6:
                    transform.position = hit.transform.position + new Vector3(0, hit.transform.localScale.y, 0);
                    break;
                case 7:
                    transform.position = hit.point;
                    break;
            }

            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
}
