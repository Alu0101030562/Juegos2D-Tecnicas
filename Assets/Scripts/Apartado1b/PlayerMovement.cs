using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private float _horizontalMove;
    private Vector3 _direction;
    
    private void Update()
    {
        _direction = Vector3.right.normalized;
        transform.Translate(_direction * (Time.deltaTime * movementSpeed));
    }
}
