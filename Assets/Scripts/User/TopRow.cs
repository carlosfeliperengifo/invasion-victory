using UnityEngine;
using UnityEngine.UI;

public class TopRow : MonoBehaviour {
   private Sprite medal;
   private string top;
   private string nick;
   private string score;
   private Color background;

   public Sprite Medal { set { medal = value; } }
   public string Top { set { top = value; } }
   public string Nick { set { nick = value; } }
   public string Score { set { score = value; } }
   public Color Background { set { background = value; } }

   void Start () {
      transform.GetChild(0).GetComponent<Image>().sprite = medal;
      transform.GetChild(1).GetComponent<Text>().text = top;
      transform.GetChild(2).GetComponent<Text>().text = nick;
      transform.GetChild(3).GetComponent<Text>().text = score;
      transform.GetComponent<Image>().color = background;
   }
}
