using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // move left
        transform.position = new Vector2(transform.position.x - .1f * Time.deltaTime, transform.position.y);
    }
}
