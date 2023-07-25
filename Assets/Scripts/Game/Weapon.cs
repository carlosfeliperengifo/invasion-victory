using UnityEngine;

public class Weapon : MonoBehaviour {
   [SerializeField] private Material[] Mats;
   [SerializeField] private ParticleSystem fire;
   [SerializeField] private MeshRenderer model;
   [SerializeField] private SkinnedMeshRenderer model2;
   private AudioSource shotSound;
   private AudioSource reloadSound;
   private AudioSource emptySound;
   private Animator animator;
   private int idMat = 0;

   public int IdMat { get { return idMat; } }

   private void Start () {
      animator = GetComponent<Animator>();
      shotSound = GetComponents<AudioSource>()[0];
      reloadSound = GetComponents<AudioSource>()[1];
      emptySound = GetComponents<AudioSource>()[2];
   }
   public void ChangeMaterial (int index) {
      if (index >= Mats.Length) {
         idMat = Mats.Length - 1;
      } else if (index < 0) {
         idMat = 0;
      } else {
         idMat = index;
      }
      if (model != null) {
         model.material = Mats[idMat];
      } else {
         model2.material = Mats[idMat];
      }
   }
   public void Repose () {
      fire.Stop();
      shotSound.Stop();
   }
   public void Shoot () {
      fire.Play();
      animator.SetTrigger("Shoot");
      shotSound.Play();
   }
   public void Reload () {
      fire.Stop();
      animator.SetTrigger("DoReload");
      reloadSound.Play();
   }
   public void Open () {
      animator.SetTrigger("DoOpen");
      emptySound.Play();
   }
}
