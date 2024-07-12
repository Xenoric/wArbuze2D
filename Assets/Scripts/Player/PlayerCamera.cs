using UnityEngine;


namespace Warbuzz.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        public Camera mainCamera;
       
        public float CameraSpeed = 5f; 
        public float Correction = 0f;
        public float MaxDistanceFromPlayer = 5f;

        public void Init()
        {
            mainCamera = Camera.main;
           
        }

        public void UpdateCameraPosition(Vector3 aimPos)
        {
            if (!mainCamera) return;
            Vector3 playerPos = transform.position;
            Vector3 targetPos = aimPos;
            Vector3 desiredPosition = Vector3.Lerp(playerPos, targetPos, Correction);
            
            Vector3 direction = desiredPosition - playerPos;
            
            if (direction.magnitude > MaxDistanceFromPlayer)
            {
                direction = direction.normalized * MaxDistanceFromPlayer;
            }
            Vector3 cameraPosition = playerPos + direction + offset;
            
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPosition, CameraSpeed);
        }
    }
}