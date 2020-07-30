using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{ 
    public List<GameObject> redTeam = new List<GameObject>();
    public List<GameObject> blueTeam = new List<GameObject>();
    //lists of alive agents on both teams
    public GameObject EndPanel; //the one with the game result
    public Text timerText;
    
    int H = 0; //time variables
    int M = 0;
    int S = 0;
    bool timerGo; //for timer stop

    private void Start()
    {
        timerGo = true;
        StartCoroutine(Timer());
    }

    public void checkTheEnd()
    { //if any of the team is out of alive agents
        if (redTeam.Count==0)
            endGame(false);
        if (blueTeam.Count == 0)
            endGame(true);
    }  //we end the game

    void endGame(bool redWon)
    {
        EndPanel.SetActive(true); //we show the panel with the game result
        if(redWon)
             EndPanel.GetComponentInChildren<Text>().text = "Game ended. Red won";
        else
            EndPanel.GetComponentInChildren<Text>().text = "Game ended. Blue won";
        timerGo = false; //and stop the timer
    }

    public void restartGame()
    {  //this button restarts the scene to play again
        Debug.Log("restart");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    IEnumerator Timer()
    {
        while (timerGo)
        {
            yield return new WaitForSeconds(1);
            S++;
            if (S == 60)
            {
                M++;
                if (M == 60)
                {
                    H++;
                    M = 0;
                }
                S = 0;
            } //we sum up seconds and show the time in the timer text
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", H, M, S);
        }
    }
}
