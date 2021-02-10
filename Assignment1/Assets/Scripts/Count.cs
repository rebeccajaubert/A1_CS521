using UnityEngine.UI;
using UnityEngine;

public class Count : MonoBehaviour
{
    public Text projectileText;

    private int pickedUp=0;

    public void newProjectilePicked()
    {
        pickedUp++;
    }
    public void newProjectileShot()
    {
        pickedUp--;
    }

    public void UpdateNb()
    {
        projectileText.text = "Projectiles : " + pickedUp.ToString();
    }

    public int getNbPickedUp()
    {
        return pickedUp;
    }
}

