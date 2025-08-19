using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float velocidadeAndar = 5f;
    public float velocidadeVoo = 8f;

   
    private bool estaVoando = false;

   
    private Rigidbody2D rb;

    void Start()
    {
     
        rb = GetComponent<Rigidbody2D>();
       
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            estaVoando = !estaVoando; 

           
            rb.gravityScale = estaVoando ? 0 : 1;
        }
    }

    void FixedUpdate()
    {
        
        if (estaVoando)
        {
            MovimentoVoo();
        }
        else
        {
            MovimentoAndar();
        }
    }

    private void MovimentoAndar()
    {
       
        float moveInput = Input.GetAxis("Horizontal");

       
        rb.linearVelocity = new Vector2(moveInput * velocidadeAndar, rb.linearVelocity.y);
    }

    private void MovimentoVoo()
    {
        
        float moveInputHorizontal = Input.GetAxis("Horizontal");
        float moveInputVertical = Input.GetAxis("Vertical");

       
        Vector2 movimento = new Vector2(moveInputHorizontal, moveInputVertical);

        
        rb.linearVelocity = movimento * velocidadeVoo;
    }
}