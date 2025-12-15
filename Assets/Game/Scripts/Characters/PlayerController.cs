using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters
{
    public class PlayerController : MonoBehaviour
    {
        private int playerID;
        private Rigidbody2D rb;
        private CircleCollider2D cd;

        // Variables de configuración
        private float speed = 5f;
        private float jumpForce = 5f;
        public LayerMask groundLayer;

        // Variables internas para el control
        private float offsetY;
        private Vector2 origin;

        // NUEVO: Variables para almacenar el input
        private bool jumpRequested = false;
        private float moveInputX = 0f;

        void Start()
        {
            playerID = this.transform.CompareTag("P1") ? 1 : 2;
            rb = GetComponent<Rigidbody2D>();
            cd = GetComponent<CircleCollider2D>();

            // Ajustamos el offset. Nota: bounds.extents.y es la mitad de la altura.
            // Sumarle 0.5f extra podría ser mucho si tu sprite es pequeño, 
            // pero lo mantengo como lo tenías.
            offsetY = cd.bounds.extents.y + 0.1f;
        }

        // 1. LEER INPUTS AQUÍ (Siempre en Update)
        void Update()
        {
            HandleInput();
        }

        // 2. APLICAR FÍSICAS AQUÍ (Siempre en FixedUpdate)
        void FixedUpdate()
        {
            ApplyMovement();
            ApplyJump();
        }

        private void HandleInput()
        {
            // Reseteamos el input horizontal cada frame
            moveInputX = 0f;

            if (playerID == 1)
            {
                if (Input.GetKey(KeyCode.A)) moveInputX = -1f;
                if (Input.GetKey(KeyCode.D)) moveInputX = 1f;

                // Capturamos la INTENCIÓN de saltar
                if (Input.GetKeyDown(KeyCode.W))
                {
                    jumpRequested = true;
                }
            }
            else if (playerID == 2)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) moveInputX = -1f;
                if (Input.GetKey(KeyCode.RightArrow)) moveInputX = 1f;

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    jumpRequested = true;
                }
            }
        }

        private void ApplyMovement()
        {
            // Movemos según el input capturado en Update
            // NOTA: linearVelocity es de Unity 6. Si usas una versión anterior, usa rb.velocity
            rb.linearVelocity = new Vector2(moveInputX * speed, rb.linearVelocity.y);
        }

        private void ApplyJump()
        {
            // Solo intentamos saltar si se pidió en el Update
            if (jumpRequested)
            {
                // Actualizamos el origen del Raycast justo antes de comprobar
                origin = new Vector2(transform.position.x, transform.position.y - offsetY);

                // Comprobamos el suelo
                bool isGrounded = Physics2D.BoxCast(origin, cd.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);

                if (isGrounded)
                {
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                }

                // IMPORTANTE: Consumimos el salto para que no salte infinitamente
                jumpRequested = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (cd != null)
            {
                Gizmos.color = Color.red;
                // Recalculamos el origen para verlo en tiempo real en el editor
                Vector2 drawOrigin = new Vector2(transform.position.x, transform.position.y - (cd.bounds.extents.y + 0.1f));
                Gizmos.DrawWireCube(drawOrigin, cd.bounds.size);
            }
        }
    }
}