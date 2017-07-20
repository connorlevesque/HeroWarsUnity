using UnityEngine;

public class PinchZoom : MonoBehaviour
{
   public float perspectiveZoomSpeed = 0.5f;
   public float orthoZoomSpeed = 0.5f;

   private Camera camera;

   void Start() {
      camera = GetComponent<Camera>();
   }

   void Update() {
      if (Input.touchCount == 2) {

         Touch touchZero = Input.GetTouch(0);
         Touch touchOne = Input.GetTouch(1);

         // Find the position in the previous frame of each touch.
         Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
         Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

         // Find the magnitude of the vector (the distance) between the touches in each frame.
         float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
         float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

         // Find the difference in the distances between each frame.
         float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

         if (camera.isOrthoGraphic) {
            camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
         } else {
            camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
         }
      }
   }
}