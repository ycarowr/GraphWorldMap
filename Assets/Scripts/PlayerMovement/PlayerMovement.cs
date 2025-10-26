using System;
using UnityEngine;

namespace ToolsYwr.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private const string AxisHorizontal = "Horizontal";
        [SerializeField] private float speed = 5f;

        private Rigidbody2D Rigidbody2D { get; set; }
        private Vector2 Direction { get; set; }

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var moveX = Input.GetAxisRaw(AxisHorizontal);
            var moveY = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                moveY = 1f;
            } 
            else
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                moveY = -1f;
            }
            
            Direction = new Vector2(moveX, moveY).normalized;
        }

        private void FixedUpdate()
        {
            Rigidbody2D.velocity = Direction * speed;
        }
    }
}
