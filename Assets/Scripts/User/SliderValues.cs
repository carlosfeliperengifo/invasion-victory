using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SliderValues : MonoBehaviour {
   [SerializeField] private GameObject Difficulty;
   [SerializeField] private GameObject SpaceshipsHorde;
   [SerializeField] private GameObject TimeHorde;
   [SerializeField] private GameObject SpeedSpaceships;

   public static SliderValues instance;
   private void Awake () {
      if (instance != null && instance != this) {
         Destroy(gameObject);
      } else {
         instance = this;
      }
   }
   private void Start () {
      if (File.Exists(Application.persistentDataPath + "/Config.txt")) {
         LoadConfigurationTxt();
      } else {
         Difficulty.GetComponentInChildren<Slider>().value = 1;
      }
      if (Difficulty.GetComponentInChildren<Slider>().value == 1) {
         DifficultyChange();
         SaveMatchTxt();
      } else {
         DifficultyChange();
         if (File.Exists(Application.persistentDataPath + "/Match.txt")) {
            LoadMatchTxt();
         } else {
            RandomValues();
            SaveMatchTxt();
         }
      }
   }
   public void DifficultyChange () {
      if (Difficulty.GetComponentInChildren<Slider>().value == 1) {
         Difficulty.GetComponent<Text>().text = "Dificultad aleatoria";
         SpaceshipsHorde.GetComponentInChildren<Slider>().interactable = false;
         TimeHorde.GetComponentInChildren<Slider>().interactable = false;
         SpeedSpaceships.GetComponentInChildren<Slider>().interactable = false;
         RandomValues();
      } else {
         Difficulty.GetComponent<Text>().text = "Dificultad manual";
         SpaceshipsHorde.GetComponentInChildren<Slider>().interactable = true;
         TimeHorde.GetComponentInChildren<Slider>().interactable = true;
         SpeedSpaceships.GetComponentInChildren<Slider>().interactable = true;
      }
   }
   public void LoadConfigurationTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Config.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "difficulty":
            Difficulty.GetComponentInChildren<Slider>().value = int.Parse(col[1]);
            if (Difficulty.GetComponentInChildren<Slider>().value == 1) {
               Difficulty.GetComponent<Text>().text = "Dificultad aleatoria";
            } else {
               Difficulty.GetComponent<Text>().text = "Dificultad manual";
            }
            break;
         default:
            break;
         }
      }
   }
   public void LoadMatchTxt () {
      TextReader Datostxt = new StreamReader(Application.persistentDataPath + "/Match.txt");
      string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      Datostxt.Close();
      foreach (string dato in datos) {
         string[] col = dato.Split(new char[] { '\t' });
         switch (col[0]) {
         case "spaceshipsHorde":
            SpaceshipsHorde.GetComponentInChildren<Slider>().value = int.Parse(col[1]);
            break;
         case "timeHorde":
            TimeHorde.GetComponentInChildren<Slider>().value = float.Parse(col[1]) / 2.0f;
            break;
         case "speedSpaceships":
            SpeedSpaceships.GetComponentInChildren<Slider>().value = (float.Parse(col[1]) - 0.4f) / 0.1f;
            break;
         }
      }
      UpdateSliderTexts();
   }
   private void RandomValues () {
      SpaceshipsHorde.GetComponentInChildren<Slider>().value = UnityEngine.Random.Range(3, 10);
      TimeHorde.GetComponentInChildren<Slider>().value = UnityEngine.Random.Range(3, 15);
      SpeedSpaceships.GetComponentInChildren<Slider>().value = UnityEngine.Random.Range(0, 8);
      UpdateSliderTexts();
   }
   public void UpdateSliderTexts () {
      SpaceshipsHorde.GetComponentsInChildren<Text>()[1].text = SpaceshipsHorde.GetComponentInChildren<Slider>().value.ToString();
      TimeHorde.GetComponentsInChildren<Text>()[1].text = (TimeHorde.GetComponentInChildren<Slider>().value * 2.0f).ToString("F0") + " [s]";
      SpeedSpaceships.GetComponentsInChildren<Text>()[1].text = (SpeedSpaceships.GetComponentInChildren<Slider>().value * 0.1f + 0.4f).ToString("F1") + " [m/s]";
   }
   private void SaveConfigurationTxt () {
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Config.txt", false);
      datos.WriteLine("difficulty" + "\t" + Difficulty.GetComponentInChildren<Slider>().value.ToString());
      datos.Close();
   }
   public void SaveMatchTxt () {
      SaveConfigurationTxt();
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Match.txt", false);
      string val = SpaceshipsHorde.GetComponentInChildren<Slider>().value.ToString("F0");
      datos.WriteLine("spaceshipsHorde" + "\t" + val);

      val = (TimeHorde.GetComponentInChildren<Slider>().value * 2.0).ToString("F0");
      datos.WriteLine("timeHorde" + "\t" + val);

      val = (SpeedSpaceships.GetComponentInChildren<Slider>().value * 0.1f + 0.4f).ToString("F1");
      datos.WriteLine("speedSpaceships" + "\t" + val);
      datos.Close();
   }
}
