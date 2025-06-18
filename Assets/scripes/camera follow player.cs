
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;


public class camerafollowplayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 0, -7);
    public float rotationSpeed = 1.0f; // Adjust this to control rotation sensitivity
    public GameObject pauseMenu;
    

    void Start()
    {
        // Optional: Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Confined;

        // Reinitialize references after the scene reloads
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("player");
        }

        if (pauseMenu == null)
        {
            pauseMenu = GameObject.Find("menu"); // Replace "PauseMenu" with the actual name of your pause menu GameObject
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Follow the player's position
        if (player != null)
        {
            transform.position = player.transform.position + offset;

            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate the camera slightly based on mouse movement
            transform.RotateAround(player.transform.position, Vector3.up, mouseX * rotationSpeed);
            transform.RotateAround(player.transform.position, transform.right, -mouseY * rotationSpeed);
        }
    }

    void Update()
    {
        pause();
        Retry();
    }

    public void Retry()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Destroy any existing player GameObjects
            GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("player");
            foreach (GameObject existingPlayer in existingPlayers)
            {
                Destroy(existingPlayer);
            }

            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // Reassign the player reference after the scene reloads
            StartCoroutine(ReassignPlayer());
        }
    }

    // Coroutine to reassign the player after the scene reloads
    private IEnumerator ReassignPlayer()
    {
        yield return null; // Wait for one frame to ensure the scene is fully loaded

        player = GameObject.FindGameObjectWithTag("player");
        if (player == null)
        {
            Debug.LogError("Player not found after scene reload!");
        }
    }

    public void pause()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                if (pauseMenu != null)
                {
                    pauseMenu.SetActive(true); // Show the pause menu
                }
            }
            else
            {
                Time.timeScale = 1;
                if (pauseMenu != null)
                {
                    pauseMenu.SetActive(false); // Hide the pause menu
                }
            }
        }
    }

    
}