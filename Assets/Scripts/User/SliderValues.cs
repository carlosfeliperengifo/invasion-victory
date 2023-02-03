using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SliderValues : MonoBehaviour {
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
   public void GetMatchTxt () {
      TextReader Datostxt;
      try {
         Datostxt = new StreamReader(Application.persistentDataPath + "/Match.txt");
         string[] datos = Datostxt.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
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
      } catch {
         SpaceshipsHorde.GetComponentInChildren<Slider>().value = 6;
         TimeHorde.GetComponentInChildren<Slider>().value = 9;
         SpeedSpaceships.GetComponentInChildren<Slider>().value = 4;
         UpdateSliders();
         SaveMatchTxt();
      }
   }
   public void UpdateSliders () {
      SpaceshipsHorde.GetComponentsInChildren<Text>()[1].text = SpaceshipsHorde.GetComponentInChildren<Slider>().value.ToString();
      TimeHorde.GetComponentsInChildren<Text>()[1].text = (TimeHorde.GetComponentInChildren<Slider>().value * 2.0f).ToString("F0") + " [s]";
      SpeedSpaceships.GetComponentsInChildren<Text>()[1].text = (SpeedSpaceships.GetComponentInChildren<Slider>().value * 0.1f + 0.4f).ToString("F1") + " [m/s]";
   }
   public void SaveMatchTxt () {
      TextWriter datos = new StreamWriter(Application.persistentDataPath + "/Match.txt", false);
      string val = SpaceshipsHorde.GetComponentInChildren<Slider>().value.ToString("F0");
      datos.WriteLine("spaceshipsHorde" + "\t" + val);

      val = (TimeHorde.GetComponentInChildren<Slider>().value * 2.0).ToString("F0");
      datos.WriteLine("timeHorde" + "\t" + val);

      val = (SpeedSpaceships.GetComponentInChildren<Slider>().value * 0.1f + 0.4f).ToString("F1");
      datos.WriteLine("speedSpaceships" + "\t" + val);
      datos.Close();
      GlobalManager.events.bt_close2();
   }
}
