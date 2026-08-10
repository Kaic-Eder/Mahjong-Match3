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
    
    public Coroutine MoverPara(Vector3 destino)
    {
        // 1. Segurança: Se ele já estiver se movendo, cancelamos o movimento anterior
        if (coroutineMovimento != null)
        {
            StopCoroutine(coroutineMovimento);
        }

        // 2. Iniciamos a nova animação e guardamos o "ticket" dela
        coroutineMovimento = StartCoroutine(AnimarMovimentoRoutine(destino));
        return coroutineMovimento;
    }
    
    private IEnumerator AnimarMovimentoRoutine(Vector3 posicaoFinal)
    {
        Vector3 posicaoInicial = transform.position;
        float distancia = Vector3.Distance(posicaoInicial, posicaoFinal);
        
        if (distancia < 0.001f)
        {
            transform.position = posicaoFinal;
            coroutineMovimento = null;
            yield break; // Cancela a Coroutine aqui
        }
        
        float duracaoCalculada = distancia / velocidadeMovimento;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoCalculada)
        {
            // Aumentamos o tempo com base no tempo do último frame
            tempoDecorrido += Time.deltaTime;
            // Calculamos a porcentagem (de 0,0 a 1,0)
            float t = tempoDecorrido / duracaoCalculada;

            // Atualizamos a posição no espaço
            transform.position = Vector3.Lerp(posicaoInicial, posicaoFinal, t);

            // PAUSA! Entrega o controle para a Unity e espera o próximo frame
            yield return null;
        }

        // Garantia de precisão matemática no final
        transform.position = posicaoFinal;
        coroutineMovimento = null; // Libera a referência
    }
    
}
