using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitcher : MonoBehaviour
{
    public GameObject scoreLight1, scoreLight2, scoreLight3, scoreLight4;
    public static ArrayList scoreLightSet = new ArrayList();
    // Start is called before the first frame update
    void Start()
    {
        scoreLight1 = GetComponent<GameObject>();
        scoreLight2 = GetComponent<GameObject>();
        scoreLight3 = GetComponent<GameObject>();
        scoreLight4 = GetComponent<GameObject>();
        scoreLightSet.Add(scoreLight1);
        scoreLightSet.Add(scoreLight2);
        scoreLightSet.Add(scoreLight3);
        scoreLightSet.Add(scoreLight4);
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0://No coins here. This is the main menu.
                Coins.totalCoins = 0;
                break;
            case 1:
                Coins.totalCoins = 15;
                break;
            case 2:
                Coins.totalCoins = 37;
                break;
            case 3:
                Coins.totalCoins = 18;
                break;
            case 4:
                Coins.totalCoins = 20;
                break;
            case 5:
                Coins.totalCoins = 54;                             
                break;
            case 6:
                Coins.totalCoins = 1;//No coins here. This is the end!
                //At the end, some circles are supposed to change color to indicate the coin score of each stage. It's not done yet.                                
                break;
        }
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void CloseGame()
    {
        Application.Quit();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Coins.coinCount == 0)
            {
                print("Wow! You avoided all coins! Congratulations!");
                //Coins.coinScores[SceneManager.GetActiveScene().buildIndex] = 5;
            }
            Coins.coinCount = 0;
            if (SceneManager.GetActiveScene().buildIndex > 5)
            {
                SceneManager.LoadScene(0);
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            
        }
    }
}
