using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class secretnumBox : MonoBehaviour
{
    public GameObject player;
    public Text text;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject==player)
        {
            text.text = text.text + num.ToString();
        }
    }
}
