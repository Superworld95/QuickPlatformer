using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Coins : MonoBehaviour
{
    public static int coinCount = 0;
    public static int totalCoins = 0;
    public TMP_Text coinUI;
    public static List<int> coinScores = new List<int>();
    public static AudioSource coinChing; //For some reason I did not figure out, the sound effect doesn't play for all coins even though their code is the same.

    // Start is called before the first frame update
    void Start()
    {
        coinChing = GetComponent<AudioSource>();
        for (int i = 0; i < 6; i++)
        {
            coinScores.Add(3);
        }
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            print(coinScores[0]);
            for (int i = 0; i < 5; i++)
            {
                if (coinScores != null)
                {
                    if (coinScores[i].Equals(2))
                    {
                        //scoreLightSet[i];
                        print("Got all coins! " + i);
                    }
                    else if (coinScores[i].Equals(0))
                    {
                        print("Got NO coins! " + i);
                        //print(scoreLightSet[0]);
                    }
                    else
                    {
                        print("Got some coins. " + i);
                    }
                }
            }       

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (coinUI != null)
        {
            coinUI.text = "Coins: " + coinCount + "/" + totalCoins;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !other.isTrigger) //Adding the !other.isTrigger comes from ChatGPT.
        {            
            Player player = other.GetComponent<Player>();
            if (player != null)
            {                
                player.IncreaseCoinCount();
            }
            coinCount++;
            coinUI.text = "Coins: " + coinCount + "/" + totalCoins;
            if (coinCount == totalCoins)
            {
                print("You have all coins!");
                coinScores[SceneManager.GetActiveScene().buildIndex] = 2;
            }
            PlayCoinSound();
            
        }
    }
    
    void PlayCoinSound()
    { //This is from a video tutorial.
        coinChing.PlayOneShot(coinChing.clip, 1);
        gameObject.SetActive(false);
    }
}
