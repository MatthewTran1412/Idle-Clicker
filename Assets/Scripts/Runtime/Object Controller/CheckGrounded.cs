using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleClicker
{
    public class CheckGrounded : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Ground"))
            {
                GameManager.instance.UpdateScoreText(PlayerController.instance.gameObject.transform, transform);
                GameManager.instance.SetCamToTarget(transform);
                GameManager.instance.ThrowingCamera.transform.position= transform.position;
                Destroy(gameObject, 1f);
            }
        }
        private void OnDestroy()=> GameManager.instance.SetCamToTarget(null);
    }
}
