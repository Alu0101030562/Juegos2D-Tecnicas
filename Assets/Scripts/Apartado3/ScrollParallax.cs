using UnityEngine;

public class ScrollParallax : MonoBehaviour
{
    [SerializeField] private float[] layerSpeeds; // Velocidad específica para cada capa

    private Renderer _renderer;
    private Material[] _parallaxLayers;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        // Para evitar modificar los materiales compartidos, creamos instancias
        _parallaxLayers = new Material[_renderer.materials.Length];
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _parallaxLayers[i] = new Material(_renderer.materials[i]); // Crear instancias únicas de cada material
        }

        // Asignar de nuevo los materiales instanciados para trabajar con ellos
        _renderer.materials = _parallaxLayers;
    }

    private void Update()
    {
        for (int i = 0; i < _parallaxLayers.Length; i++)
        {
            // Calcula el desplazamiento basado en el tiempo transcurrido
            float offset = (Time.time * layerSpeeds[i]) % 1.0f;
            Vector2 textureOffset = new Vector2(offset, 0);

            // Aplica el nuevo offset de la textura al material correspondiente
            _parallaxLayers[i].SetTextureOffset(MainTex, textureOffset);
        }
    }
}

