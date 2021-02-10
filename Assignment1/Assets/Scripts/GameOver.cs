using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public Transform player;
    public Text overText;
    public Text winText;
    public Text instructions;
   // private IEnumerator coroutine;

    private void Start()
    {
        overText.enabled = false;
        winText.enabled = false;
        instructions.enabled = true;
        StartCoroutine("Inst");
       

    }
    void Update()
    {
        if(player.position.y < 210 && winText.enabled==false)
        {
             overText.enabled = true;
        }
        if(player.position.x > 1690 && overText.enabled==false)
        {
            winText.enabled = true;
        }
    }
    IEnumerator Inst()
    {
        
        yield return new WaitForSeconds(3f);
        instructions.enabled = false;
    }


}
