using UnityEngine;
using UnityEngine.UI;

public class soundmenu : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if(!PlayerPrefs.HasKey("musicVolume")) {
        PlayerPrefs.SetFloat("musicVolume", 1);
        load();
       }
       else
       {
        load();
       }
    }

    // Update is called once per frame
   public void changeVolume(){
    AudioListener.volume = volumeSlider.value;
    save();
   }
   private void load(){
    volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
   }
   private void save(){
    PlayerPrefs.SetFloat("musicVolume",volumeSlider.value);
   }

}
