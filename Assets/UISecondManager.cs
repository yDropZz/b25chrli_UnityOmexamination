using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // https://discussions.unity.com/t/c-countdown-timer/37915/2
    [SerializeField] private TextMeshProUGUI timerText;
    private float currCountdownValue;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        StartCoroutine(StartCountdown());
        timerText.text = "Timer" + currCountdownValue;
    }

    // Update is called once per frame
    void Update()
    {

        if (currCountdownValue <= 1)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
      
        
    }
    
    public IEnumerator StartCountdown(float countdownValue = 59)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            Debug.Log("Countdown: " + currCountdownValue);
            timerText.text = "Timer " + currCountdownValue;
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
    }
}