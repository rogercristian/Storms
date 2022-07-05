using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float speed = 5.0f;
    public int maxHealth = 12;

    public int health { get { return currentHealth; } }

    private int currentHealth;
    private bool isFlipping = false;
  

    private Rigidbody2D rb2D;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }      

        MovingCharacter();
    }
    public void ChangeHealth(int amount)
    {
        amount = Mathf.Clamp(currentHealth, 0, maxHealth);

    }

    private void MovingCharacter()
    {
        Vector2 position = InputManager.GetInstance().GetMoveDirection();
        rb2D.velocity = new Vector2(position.x * speed, position.y * speed);

        bool flip = isFlipping;
        Vector3 rotation = transform.eulerAngles;

        animator.SetFloat("Speed", position.magnitude);

        if(position.x < 0 && !flip)
        {
            rotation.y = 180f;
            isFlipping = true;
        } else if(position.x > 0 && flip)
        {
            rotation.y = 0f;
            isFlipping = false;
        }
        transform.eulerAngles = rotation;
    }

   
}
