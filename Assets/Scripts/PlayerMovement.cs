using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1;

    private Animator animator;
    
    private static readonly int Running = Animator.StringToHash("Running");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var xAxis = speed * horizontalAxis * Time.deltaTime;
        transform.Translate(xAxis, 0, 0);
        FlipSprite(horizontalAxis);
    }

    private void FlipSprite(float axis)
    {
        int direction;
        if (axis > 0)
        {
            direction = 1;
        }
        else if (axis < 0)
        {
            direction = -1;
        }
        else
        {
            TriggerRunningAnim(false);
            return;
        }

        TriggerRunningAnim(true);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    private void TriggerRunningAnim(bool isRunning)
    {
        animator.SetBool(Running, isRunning);
    }
    
}