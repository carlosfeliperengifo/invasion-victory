using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour {
   [SerializeField] private Text debugText;
   private const float maxAngularSpeed = 320f;
   //private float speed = 20.0f;
   //private Vector3 currentVelocity = Vector3.zero;
   private Quaternion initialRotation;

   //private float alpha = 0.001f;
   //private Vector3 filterGyro = Vector3.zero;

/*
   void Start () {
      //float speedFactor = Mathf.Clamp01(SystemInfo.processorCount / 4f);
      //float baseSpeed = 15f;
      //speed = baseSpeed * speedFactor;

      initialRotation = transform.rotation;
      Input.gyro.enabled = true;
   }*/
   private void Update () {
      /*Quaternion gyroRotation = Input.gyro.attitude;

      Quaternion targetRotation = initialRotation * Quaternion.Euler(90, 0, 0) * gyroRotation * Quaternion.Euler(0, 0, 180);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularSpeed * Time.deltaTime);
      transform.rotation = targetRotation;*/
      debugText.text = transform.position.ToString("F3");
   }
   /*
   private void Movement () {
   
      Vector3 gyroAccelleration = Input.gyro.userAcceleration;
      filterGyro = (alpha * gyroAccelleration) + ((1 - alpha) * filterGyro);

      Vector3 targetPosition = transform.position + (targetRotation * filterGyro) * dt;
      float targetFPS = Application.targetFrameRate;
      float smoothTime = targetFPS > 0 ? 0.5f / targetFPS : 0.3f;
      Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
      transform.position = targetPosition;

      //debugText.text = "G R: " + gyroRotation.eulerAngles.ToString() + "\nC R: " + transform.rotation.eulerAngles.ToString() + "\nG A: " + (10f*gyroAccelleration).ToString() + "\nT P: " + targetPosition.ToString() + "\nC P: " + transform.position.ToString();
   }*/
}
