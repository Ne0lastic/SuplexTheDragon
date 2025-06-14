using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class mainmenu : MonoBehaviour
{
 public void Play(){

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    Time.timeScale = 1f;
 }
 public void Quit(){
    Application.Quit();

 }
 
 
 
 
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
