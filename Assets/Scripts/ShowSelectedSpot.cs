using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowSelectedSpot : MonoBehaviour
{
    private Transform _camera;
    private PlayerInput _playerInput;
    private void Awake()
    {
        _camera = transform.parent.GetComponentInChildren<Camera>().transform;
        _playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (Physics.Raycast(_camera.position, _camera.TransformDirection(Vector3.forward), out var hit, 
                5f, LayerMask.GetMask("Card", "Deck", "Table")))
        {
            switch (hit.transform.gameObject.layer) 
            {
                case 3: // decl
                case 6: // carl
                    transform.position = hit.transform.position + new Vector3(0, hit.transform.localScale.y, 0);
                    break;
                case 7: // tabl
                    transform.position = hit.point;
                    break;
            }

            var playerYRotation = transform.parent.localEulerAngles.y;
            float[] angles = { 0, 90, 180, 270 };
            var cardYRotation = angles.OrderBy(angle => Mathf.Abs(Mathf.DeltaAngle(playerYRotation, angle))).First();
            transform.rotation = Quaternion.Euler(90, hit.transform.gameObject.layer == 7 && _playerInput.actions["ShiftDown"].inProgress ? playerYRotation : cardYRotation, 0);
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
}
