using UnityEngine;
using UnityEngine.SceneManagement;

public class pausemenu : MonoBehaviour
{
    public GameObject music;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play(){

        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
 
 public void Quit(){
    SceneManager.LoadScene("title");
    if(music == null){
       music = GameObject.FindWithTag("music");
    }
    Destroy (music);
 }
}
