using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>(); 
            
            Debug.Log("You caught a bee!");
            player.GainScore(1);
            
            Destroy(gameObject);
        }
        
        
        
        
    }
}