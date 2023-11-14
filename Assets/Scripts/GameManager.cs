using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int stageIndex;
    public int health;
    public GameObject[] stages;
    public PlayerMove player;

    public Image[] UIhealth;
    public TextMeshProUGUI UIStage;
    public TextMeshProUGUI UIPoint;
    public GameObject retry;

    public void Update()
    {
        UIPoint.text = "Point : " + (totalPoint + stagePoint).ToString();
    }

    public void nextStage()
    {
        if (stageIndex < stages.Length - 1)
        {
            //Change stage
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            UIStage.text = "STAGE " + (stageIndex + 1);

            //Calculate stage point
            totalPoint += stagePoint;
            stagePoint = 0;
        }
        else //Game clear
        {
            Time.timeScale = 0;
            TextMeshProUGUI text = retry.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Clear!";
            retry.SetActive(true);
        }
    }

    public void healthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 1, 1, 0.5f);

            //Player die effect
            player.Ondie();

            //Result UI

            //Retry button UI
            retry.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            //Health down
            healthDown();

            //Player reposition
            if (health > 1)
            {
                repositionPlayer();
            }
        }
    }

    void repositionPlayer()
    {
        player.velocityZero();
        player.transform.position = new Vector3(-6, 0.5f, 0);
    }

    //Restart map
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
