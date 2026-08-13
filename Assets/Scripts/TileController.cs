using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    
    [SerializeField] private int tileTypeId;
    
    [Header("Configurações de Animação")]
    [SerializeField] private float velocidadeMovimento = 12f;
    
    private Coroutine coroutineMovimento;
    public int TileTypeId => tileTypeId;
    
    

    private void Awake()
    {
        tileTypeId = int.Parse(gameObject.GetComponent<SpriteRenderer>().sprite.name.Remove(0, 6));
    }
    
}
