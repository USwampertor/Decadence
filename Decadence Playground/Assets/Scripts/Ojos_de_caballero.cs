using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Ojos_de_caballero : MonoBehaviour {

    //public int i;
    public GameObject[] efectos;
    int i = 0;
    // Use this for initialization
    void Start () {

    }
    void Unsegundo()
    {
        StartCoroutine("Wait");
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.65f);

        efectos[i].SetActive(false);
        i++;
        if (i == efectos.Length) {
            i = 0;
        }
    }
    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            efectos[i].SetActive(true);
            Unsegundo();
        }
    }
}

