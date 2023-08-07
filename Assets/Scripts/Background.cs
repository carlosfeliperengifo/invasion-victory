using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour {
   [SerializeField] private Color[] edges;
   [SerializeField] private Sprite[] images;

   private const int updateTime = 5;
   // Change the background every updateTime
   private IEnumerator Start () {
      ChangeBackground();
      yield return new WaitForSeconds(updateTime);
      StartCoroutine(Start());
   }
   // Change the border color and background image
   private void ChangeBackground () {
      int posBg = Random.Range(0, edges.Length);
      int posImage = Random.Range(0, images.Length);
      transform.GetChild(0).GetComponent<RawImage>().color = edges[posBg];
      transform.GetChild(1).GetComponent<Image>().sprite = images[posImage];
   }
}
