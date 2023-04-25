using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;
    public GameObject pacman2;
    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource powerPelletAudio;
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio; 

    public int currentMunch = 0;
    public int score_1;
    public int score_2; 
    public Text scoreText_1;
    public Text scoreText_2; 

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletCollectedInThisLife;

    public bool hadDeathOnThisLevel = false;
    public bool isdead_1 = false;
    public bool isdead_2 = false;

    public bool gameIsRunning;

    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource death; 

    public int currentLevel; 

    public Image blackBackground;

    public Text gameOverText;

    public bool isPowerPelletRunning = false;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public int powerPelletMultiplier = 1;

    public enum GhostMode
    {
        chase, scatter
    }

    public GhostMode currentGhostMode;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };
    public int ghostModeTimerIndex;
    public float ghostModeTimer = 0;
    public bool runningTimer;
    public bool completedTimer; 

    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;
        blackBackground.enabled = false; 

        redGhostController = redGhost.GetComponent<EnemyController>(); 
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;

        pacman = GameObject.Find("Player");
        pacman2 = GameObject.Find("Player2");

        StartCoroutine(Setup());
  
    }

    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("FirebaseLogin");
    }

    public IEnumerator Setup()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        completedTimer = false;
        runningTimer = true; 
        gameOverText.enabled = false; 
        //if pacman clears a level, a background will appear covering the level, and the game will pause for 0.1 seconds 
        if (clearedLevel)
        {
            blackBackground.enabled = true; 
            //Activate background
            yield return new WaitForSeconds(0.1f);
        }
        blackBackground.enabled = false; 

        pelletCollectedInThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;

        float waitTimer = 1f; 

        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets; 
            waitTimer = 4f; 
            //Pellets will respawn when pacman clears the level or starts a new game  
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }
        
        if (newGame)
        {
            startGameAudio.Play();
            score_1 = 0;
            score_2 = 0;
            scoreText_1.text = "Score: " + score_1.ToString();
            scoreText_2.text = "Score: " + score_2.ToString();
            currentLevel = 1; 
        }

        pacman.GetComponent<PlayerController>().Setup(new Vector2(0.44f, -0.58f), KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, "right");
        pacman2.GetComponent<PlayerController>().Setup(new Vector2(-0.4300001f, -0.5800002f), KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, "left");

        redGhostController.Setup();
        pinkGhostController.Setup(); 
        blueGhostController.Setup();
        orangeGhostController.Setup();

        newGame = false;
        clearedLevel = false;
        yield return new WaitForSeconds(waitTimer);

        StartGame(); 
    }

    void StartGame()
    {
        gameIsRunning = true;
        siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        powerPelletAudio.Stop();
        respawningAudio.Stop();
        

    }

    void Update()
    {
        if (!gameIsRunning)
        {
            return; 
        }

        if (redGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || pinkGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || blueGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning
            || orangeGhostController.ghostNodeState == EnemyController.GhostNodeStatesEnum.respawning)
        {
            if (!respawningAudio.isPlaying)
            {
                respawningAudio.Play();
            }
        }
        else
        {
            if (respawningAudio.isPlaying)
            {
                respawningAudio.Stop(); 
            }
        }
        
        if(!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++; 
                if (currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter; 
                }
                else
                {
                    currentGhostMode = GhostMode.chase; 
                }

                if (ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase; 
                }
            }
        }

        if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime; 
            if (currentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                siren.Play();
                powerPelletMultiplier = 1; 
            }
        }
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int amount, string collision_tag)
    {
        if (collision_tag == "Player")
        {
            score_1 += amount;
            scoreText_1.text = "P1 Score: " + score_1.ToString();
        }
        
        if (collision_tag == "Player2")
        {
            score_2 += amount;
            scoreText_2.text = "P2 Score: " + score_2.ToString();
        }
    }

    public IEnumerator CollectedPellet(NodeController nodeController, string collision_tag) //collision_tag is either going to be Player1 or Player2
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1; 
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletCollectedInThisLife++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0; 

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32; 
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60; 
        }

        if (pelletCollectedInThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true; 
        }

        if (pelletCollectedInThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        AddToScore(10, collision_tag);

        //Check how many pellets were eaten 
        if (pelletsLeft == 0)
        {
            currentLevel++;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }

        //Is this a power pellet 
        if (nodeController.isPowerPellet)
        {
            siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;

            redGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);
        }

        //Is this a revive pellet 
        if (nodeController.isRevivePellet)
        {
            AddToScore(800 , collision_tag);

            //Debug.Log("Entering revive");
            if (isdead_1)
            {
                //Debug.Log("Reviving 1");
                pacman = GameObject.Find("Player");
                pacman.GetComponent<PlayerController>().Setup(new Vector2(0.44f, -0.58f), KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, "right");
                siren.Stop();
                powerPelletAudio.Play();
                isPowerPelletRunning = true;
                currentPowerPelletTime = 0;

                redGhostController.SetFrightened(true);
                pinkGhostController.SetFrightened(true);
                blueGhostController.SetFrightened(true);
                orangeGhostController.SetFrightened(true);
            }
            if (isdead_2)
            {
                //Debug.Log("Reviving 2");
                pacman2 = GameObject.Find("Player2");
                pacman2.GetComponent<PlayerController>().Setup(new Vector2(-0.4300001f, -0.5800002f), KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, "left");
                siren.Stop();
                powerPelletAudio.Play();
                isPowerPelletRunning = true;
                currentPowerPelletTime = 0;

                redGhostController.SetFrightened(true);
                pinkGhostController.SetFrightened(true);
                blueGhostController.SetFrightened(true);
                orangeGhostController.SetFrightened(true);
            }
        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true; 
    }

    public void GhostEaten(string collision_tag)
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplier, collision_tag);
        powerPelletMultiplier++; 
        StartCoroutine(PauseGame(1)); 
    }

    public IEnumerator PlayerEaten(string collision_tag)
    {

        if(!(isdead_1 && isdead_2))
        {
            hadDeathOnThisLevel = true;
            
            if (collision_tag == "Player" && pacman != null)
            {
                gameIsRunning = false;

                pacman.GetComponent<PlayerController>().Stop();
                yield return new WaitForSeconds(1);
                pacman.GetComponent<PlayerController>().Death();
                death.Play();
                yield return new WaitForSeconds(3);

                gameIsRunning = true;
                //node_1_death = pacman.GetComponent<PlayerController>().movementController.currentNode;
                //node_1_death.hasPellet = true;
                //pacman.GetComponent<PlayerController>().movementController.currentNode.GetComponent<PlayController>().hasPellet = true;

                pacman = null;
                isdead_1 = true;
            }
            if (collision_tag == "Player2" && pacman2 != null)
            {
                gameIsRunning = false;

                pacman2.GetComponent<PlayerController>().Stop();
                yield return new WaitForSeconds(1);
                pacman2.GetComponent<PlayerController>().Death();
                death.Play();
                yield return new WaitForSeconds(3);

                gameIsRunning = true;
                //node_2_death = pacman2.GetComponent<PlayerController>().movementController.currentNode;
                //node_2_death.hasPellet = true;
                pacman2 = null;
                isdead_2 = true;
            }    
        }

        if (isdead_1 && isdead_2)
        {
            StopGame();

            yield return new WaitForSeconds(1);

            redGhostController.SetVisible(false);
            pinkGhostController.SetVisible(false);
            blueGhostController.SetVisible(false);
            orangeGhostController.SetVisible(false);

            newGame = true;
            //Display gameover text 
            gameOverText.enabled = true;

            yield return new WaitForSeconds(3);
            isdead_1 = false;
            isdead_2 = false;

            pacman = GameObject.Find("Player");
            pacman2 = GameObject.Find("Player2");

            StartCoroutine(Setup());
        }


    }
}
