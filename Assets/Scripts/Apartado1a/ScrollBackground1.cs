using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScrollBackground1 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;

    [SerializeField] private Renderer backgroundOnView;
    [SerializeField] private Renderer auxBackground;
    [SerializeField] private CinemachineVirtualCamera vcam;
    
    private float _horizontalMovement;
    private Vector3 _direction;
    
    private void Update()
    {
        _direction = Vector3.left.normalized;
        transform.Translate(_direction * (Time.deltaTime * moveSpeed));

        if (backgroundOnView.transform.position.x + backgroundOnView.bounds.size.x < vcam.transform.position.x)
        {
            var newBackgroundPos = backgroundOnView.transform.position;
            newBackgroundPos.x = auxBackground.transform.position.x + auxBackground.bounds.size.x;
            backgroundOnView.transform.position = newBackgroundPos;
            (auxBackground, backgroundOnView) = (backgroundOnView, auxBackground);
        }
    }
}
