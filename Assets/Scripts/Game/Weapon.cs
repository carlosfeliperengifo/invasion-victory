using UnityEngine;

public class Weapon : MonoBehaviour {
   // Materials available for the weapon
   [SerializeField] private Material[] Mats;
   // Animation of fire
   [SerializeField] private ParticleSystem fire;
   // Weapon with different Renderers
   [SerializeField] private MeshRenderer model;
   [SerializeField] private SkinnedMeshRenderer model2;
   // Audios for each weapon status
   private AudioSource shotSound;
   private AudioSource reloadSound;
   private AudioSource emptySound;
   private Animator animator;
   private int idMat = 0;

   public int IdMat { get { return idMat; } }
   // Obtain weapon components
   private void Start () {
      animator = GetComponent<Animator>();
      shotSound = GetComponents<AudioSource>()[0];
      reloadSound = GetComponents<AudioSource>()[1];
      emptySound = GetComponents<AudioSource>()[2];
   }
   // Update weapon material
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
   // Weapon in Repose state
   public void Repose () {
      fire.Stop();
      shotSound.Stop();
   }
   // Weapon in Shoot state
   public void Shoot () {
      fire.Play();
      animator.SetTrigger("Shoot");
      shotSound.Play();
   }
   // Weapon in Reload state
   public void Reload () {
      fire.Stop();
      animator.SetTrigger("DoReload");
      reloadSound.Play();
   }
   // Weapon in Open state without munition
   public void Open () {
      animator.SetTrigger("DoOpen");
      emptySound.Play();
   }
}
