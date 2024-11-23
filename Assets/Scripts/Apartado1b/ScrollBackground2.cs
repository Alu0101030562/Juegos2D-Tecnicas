using Cinemachine;
using UnityEngine;

public class ScrollBackground2 : MonoBehaviour
{
    [SerializeField] private Renderer backgroundOnView;
    [SerializeField] private Renderer auxBackground;
    [SerializeField] private CinemachineVirtualCamera vcam;
    
    private void Update()
    {
        if (backgroundOnView.transform.position.x + backgroundOnView.bounds.size.x < vcam.transform.position.x)
        {
            var newBackgroundPos = backgroundOnView.transform.position;
            newBackgroundPos.x = auxBackground.transform.position.x + auxBackground.bounds.size.x;
            backgroundOnView.transform.position = newBackgroundPos;
            (auxBackground, backgroundOnView) = (backgroundOnView, auxBackground);
        }
    }
}
