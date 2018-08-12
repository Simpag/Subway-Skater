using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const int COIN_SCORE_AMOUNT = 5;

    public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }

    private bool isGameStarted = false;
    private PlayerMotor motor;

    //UI and UI fields
    public Animator gameCanvasAnim;
    public Text scoreText, coinText, modifierText, highscoreText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    // Death Menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinText;

    private void Awake()
    {
        Instance = this;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        modifierScore = 1;

        highscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartRunning();

            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;

            gameCanvasAnim.SetTrigger("Show");
        }

        if (isGameStarted && !IsDead)
        {
            //Increase score
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void GetCoin()
    {
        coinScore ++;
        score += COIN_SCORE_AMOUNT;
        coinText.text = coinScore.ToString("0");
        scoreText.text = score.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;

        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnDeath()
    {
        IsDead = true;
        deadCoinText.text = score.ToString("0");
        deadScoreText.text = coinScore.ToString("0");

        deathMenuAnim.SetTrigger("Dead");

        FindObjectOfType<GlacierSpawner>().IsScrolling = false;

        gameCanvasAnim.SetTrigger("Hide");

        // Check if this is a highscore
        if (score > PlayerPrefs.GetInt("Highscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s++;

            PlayerPrefs.SetInt("Highscore", (int)s);
        }
    }

}
