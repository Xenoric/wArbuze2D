using UnityEngine;

namespace Warbuzz.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        public KeyCode keyBtnJump = KeyCode.W;
        public KeyCode keyBtnFly = KeyCode.Space;
        public KeyCode keyBtnWeapon1 = KeyCode.Alpha1;
        public KeyCode keyBtnWeapon2 = KeyCode.Alpha2;
        public KeyCode keyBtnWeapon3 = KeyCode.Alpha3;

        public Transform aimTransform;

        public float horizontalInput => Input.GetAxis("Horizontal");
        public float verticalInput => Input.GetAxis("Vertical");
        public float horizontalInputRaw => Input.GetAxisRaw("Horizontal");
        public float verticalInputRaw => Input.GetAxisRaw("Vertical");

        public void InitializeAim(bool en)
        {
            aimTransform.parent = null;
            aimTransform.gameObject.SetActive(en);
        }

        public void UpdateAimPosition()
        {
            if (!playerController.p_camera.mainCamera) return;
            Vector3 mousePosition = playerController.p_camera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            aimTransform.position = mousePosition;
        }

    }
}

