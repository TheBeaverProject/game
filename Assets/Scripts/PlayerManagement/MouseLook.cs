using Photon.Pun;
using UnityEngine;

namespace PlayerManagement
{
    public class MouseLook : MonoBehaviourPun
    {
        public float mouseSensitivity = 100f;
        //Mouse sensitivity could modify it in settings

        public Transform playerBody;
        //Dragged the character controller from unity

        // Alows the MouseLook to be disabled when the Player is in UI menus
        public bool followCursor = true;

        private float xRotation = 0f;
    
        void Start()
        {
            // Returns is this is not instancied by the controlled player
            if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsConnected && photonView.IsMine == false)
            {
                return;
            }
            
            //Locks the cursor in the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    
        void Update()
        {
            // Returns is this is not instancied by the controlled player
            if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsConnected && photonView.IsMine == false)
            {
                return;
            }

            if (!followCursor)
            {
                Cursor.visible = true;
                return; // Return if the Mouselook is disabled
            }
            else
            {
                Cursor.visible = false;
            }
            
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            //Get mouse input

            // If we rotate with mouseY we end up rotating toward the wrong direction
            xRotation -= mouseY;
        
            /*
            *   Set max rotation on Y axis 
            *   as in real life you can't look more than 90 degrees up or down
            *   Settings the encadrement endup to the following issue:
            *   https://github.com/TheBeaverProject/game/issues/11#issue-797167972
            *
            *   FIX: As in real life, if you look 90degrees down you look though your neck :)
            *   set max degree down to 45 (it can be a bit reduce but it match the model)  
            */
            xRotation = Mathf.Clamp(xRotation, -90f, 60f);
            //-90 and 90 to not allow the player to move too fast and see behind him
            
            transform.localRotation = Quaternion.AngleAxis(xRotation, Vector3.right);
            //Rotates the character controller
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
