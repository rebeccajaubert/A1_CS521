
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public LayerMask maze;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "ThirdPersonPlayer")
        {
            Count c = collider.transform.GetComponent<Count>();
            c.newProjectilePicked();
            c.UpdateNb();
            this.gameObject.SetActive(false);
        }

        //if (collider.gameObject.layer == maze)
        //{
           
        //    collider.gameObject.SetActive(false);
        //    Debug.Log("colide");

        //}

    }

    
}
