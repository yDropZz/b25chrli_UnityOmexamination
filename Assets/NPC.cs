using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{

    [SerializeField] private GameObject dialogueText;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            
            dialogueText.SetActive(true);
                
            Invoke("RemoveText", 3f);   
            
            
        }
    }

    void RemoveText()
    {
        dialogueText.SetActive(false);
    }
}