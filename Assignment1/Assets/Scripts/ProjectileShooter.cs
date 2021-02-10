
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    GameObject prefab;

    void Start()
    {
        prefab = Resources.Load("Projectile") as GameObject;
        prefab.SetActive(true);
    }

    void FixedUpdate()
    {
        Count c = this.transform.GetComponent<Count>();
        int nbProjectiles = c.getNbPickedUp();

        if (Input.GetMouseButtonDown(1) && nbProjectiles!=0) //shoot using mouse left click
        {
            GameObject projectile = Instantiate(prefab) as GameObject;
            projectile.transform.position = transform.position + Camera.main.transform.forward * 3;
            projectile.transform.rotation = Quaternion.Euler(90, Camera.main.transform.eulerAngles.y, 0f);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            Vector3 angle = new Vector3(0f, 2, 0f);
            rb.velocity = (Camera.main.transform.forward + angle) * 6;
            rb.useGravity = true;
            // rb.AddForce(Camera.main.transform.forward * 1000);

            c.newProjectileShot();
            c.UpdateNb();
        }
    }

  
 

}
