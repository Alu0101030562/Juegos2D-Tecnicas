# Juegos2D - Tecnicas

## 1. Aplicar un fondo con scroll a tu escena utilizando la siguientes técnicas:

### a.  La cámara está fija, el fondo se va desplazando en cada frame. Se usan dos fondos. Uno de ellos lo va viendo la cámara en todo momento, el otro está preparado para el momento en que se ha avanzado hasta el punto en el que la vista de la cámara ya no abarcaría el fondo inicial. Por tanto, se va actualizando en todo momento la posición de los dos fondos, haciéndolos avanzar hacia la izquierda. Cuando la cámara alcanza el límite, se debe intercambiar el rol de los fondos.

Primero tenemos que añadir un GameObject de tipo **Quad** en la escena que representará nuestro fondo. Para ello presionamos boton derecho en el inspector y, dentro de 3D Object, pulsamos Quad. Trás eso, creamos un material y cambiaremos el tipo de shader a **Unlit/Texture** para poder asignar la imagen que queremos que tenga el fondo como textura.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/1.%20MaterialFondo.png)

Para terminar, arrastramos el material del objeto de tipo **Quad**

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/2.%20Background%20con%20material.png)

También habrá que añadir un segundo fondo para poder hacer la lógica de scroll.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/3.%20Background%20con%202%20fondos.png)

Una vez creado el fondo, debemos hacer el script que tenga una referencia para cada fondo, para que la cámara empleada en el juego haga que el fondo se mueva.

```c#
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
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/4.%20Ejecucion%20apartado%20a.gif)

### b. La cámara se desplaza a la derecha y el fondo está estático. Existe nuevamente un fondo de reserva, que pasa a verse cuando el avance de la cámara sobrepasa el límite. El fondo anterior deb ubicarse a continuación del otro para que esté preparado.

Para este apartado haremos otro script, siendo similar al hecho anteriormente, pero con la diferencia de que en vez de mover los fondos a la izquierda se eliminará la lógica de su movimiento, por lo que permanecerán estáticos y lo único que harán sera reposicionarse cuando la cámara enfoque el siguiente fondo.

```c#
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
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/5.%20Ejecucion%20apartado%20b.gif)

## 2. Aplicar un fondo a tu escena aplicando la técnica del desplazamiento de textura.

Lo primero que debemos hacer es cambiar el Wrap mode de nuestra textura a **repeat**

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/6.Cambio%20del%20wrap%20mode%20a%20repeat.png)

Entonces, en vez de desplazar el fondo o la cámara, se desplazará la textura.

```c#
public class RepeatBackground : MonoBehaviour
{
    [SerializeField] private float _scrollSpeedX = 0.5f;
    
    private Renderer _renderer; 
    private Vector2 _offset = Vector2.right;
    
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _offset.x += _scrollSpeedX * Time.deltaTime;
        _renderer.material.SetTextureOffset(MainTex, _offset);
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/7.%20Ejecucion%20apartado%202.gif)

## 3. Parallax

### a. Aplicar efecto parallax usando la técnica de scroll en la que se mueve continuamente la posición del fondo.

Creamos varios materiales que serán las distintas capas de nuestro fondo con efecto parallax y se las asignamos al objeto de tipo **Quad**. 

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/8.%20Ejecucion%20apartado%203a.gif)

### b. Aplicar efecto parallax actualizando el offset de la textura.

```c#
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
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/9.%20Ejecucion%20apartado%203b.gif)

## 4. En tu escena 2D crea un prefab que sirva de base para generar un tipo de objetos sobre los que vas a hacer un pooling de objetos que se recolectarán continuamente en tu escena. Cuando un objeto es recolectado debe pasar al pool y dejar de visualizarse. Este objeto estará disponible en el pool. Cada objeto debe llevar un contador, cuando alcance 3 será destruido. En la escena, siempre que sea posible debe haber una cantidad de objetos que fijes, hasta que el número de objetos que no se han eliminado sea menor que dicha cantidad. Recuerda que para generar los objetos puedes usar el método Instantiate. Los objetos ya creados pueden estar activos o no, para ello usar SetActive.

Creamos un objeto vacío que hará de pool de objetos. A este objeto le añadiremos un script que haga que cuando comience la ejecución del juegos, se inicialice el pool de objetos y, después de x segundos, se activará un objeto distinto del pool.

```c#
public class Spawner : MonoBehaviour
{
    [SerializeField] private Item itemPrefab;
    [SerializeField] private int spawnAmount = 3;
    [SerializeField] private float repeatRate = 2.5f;

    private List<Item> _itemPool;
    private int _currentItemIndex;
    
    private void Start()
    {
        InitItemPool();
    }

    private void InitItemPool()
    {
        _itemPool = new List<Item>();

        for (int i = 0; i < spawnAmount; i++)
        {
            _itemPool.Add(Instantiate(itemPrefab, transform));
            _itemPool[i].gameObject.SetActive(false);
        }
        
        InvokeRepeating(nameof(SetPooledItemActive), 0, repeatRate);
    }

    private void SetPooledItemActive()
    {
        
        if (_currentItemIndex < _itemPool.Count)
        {
            _itemPool[_currentItemIndex].gameObject.SetActive(true);
            
            _currentItemIndex++;
        }
        else
            _currentItemIndex = 0;
    }

    public void UnsubscribeItem(Item item)
    {
        _itemPool.Remove(item);
    }
}
```

Por otro lado, los items del pool tienen un script que se encarga de moverlos hacia la izquierda constantemente. Cuando estos colisionan con el jugador, se desactivarán y su contador interno incrementará. Si este contador llega a 3, serán eliminados del pool y de la escena.

```c#
public class Item : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;

    private int _timesCollected;

    private Spawner _spawner;

    private void Start()
    {
        _spawner = GetComponentInParent<Spawner>();
    }


    private void Update()
    {
        transform.Translate(Vector3.left * (_moveSpeed * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _timesCollected++;

            if (_timesCollected == 3)
            {
                _spawner.UnsubscribeItem(this);
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Tecnicas2D/10.%20Ejecucion%20apartado%204.gif)

## 5. Revisa tu código de la entrega anterior e indica las mejoras que podrías hacer de cara al rendimiento.

- En el fichero **CameraSwitcher.cs** el update se ejecuta cada frame y se evalua continuamente las entradas del teclado, esto hace que las llamadas redundantes a SetActive pueden causar *overhead* si el estado de la cámara ya es el deseado. Para arreglarlo, podemos usar *Input.GetKeyDown* para evitar redundancias
- En el fichero **PlayetMovementSimple.cs** podemos evitar llamar continuamente a *vector3.right* si el valor es constante. Para arreglarlo podemos usar una variable predefinida para *vector3.right*
