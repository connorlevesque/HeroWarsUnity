using UnityEngine;

public class PinchZoom : MonoBehaviour {

   public float zoomSpeed = .05f;
   public float zoomInBound = 5f;
   public float zoomOutBound = 15f;

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

         // Zoom
         camera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
         camera.orthographicSize = Mathf.Max(camera.orthographicSize, zoomInBound);
         camera.orthographicSize = Mathf.Min(camera.orthographicSize, zoomOutBound);

         // // Move camera to maintain pinch center
         // Vector2 pinchCenter = (touchZeroPrevPos + touchOnePrevPos) / 2f;
         // Vector2 cameraToPinchCenter = pinchCenter - (Vector2)transform.position;
         // cameraToPinchCenter.Normalize();
         // transform.position -= (Vector3)cameraToPinchCenter * deltaMagnitudeDiff * .1f;

         // SnapToBoundaries();
      }
   }

   private void SnapToBoundaries() {
      GetComponent<CameraDrag>().SnapToBoundaries();
   }
}