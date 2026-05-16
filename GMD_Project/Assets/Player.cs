using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 movement;

    public float speed = 5f;
    public TextMeshPro text;

    private void Update()
    {
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        transform.Translate(move * (speed * Time.deltaTime), Space.World);
    }

    public void OnMovement(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    public void OnA()
    {
        ShowInput("A");
    }

    public void OnB()
    {
        ShowInput("B");
    }

    public void OnX()
    {
        ShowInput("X");
    }

    public void OnY()
    {
        ShowInput("Y");
    }

    public void OnLeftTrigger()
    {
        ShowInput("L Trigger");
    }

    public void OnRightTrigger()
    {
        ShowInput("R Trigger");
    }

    public void OnStart()
    {
        ShowInput("Start");
    }

    private void ShowInput(string message)
    {
        Debug.Log(message);

        if (text != null)
            text.text = message;
    }
}