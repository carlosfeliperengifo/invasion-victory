using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour {

   public void LoadScene (string SceneName) {
      Time.timeScale = 1;
      SceneManager.LoadScene(SceneName);
   }

   public void CloseApp () {
      Application.Quit();
   }
}