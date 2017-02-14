using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed = 4f;

    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == this.transform.root.gameObject)
            return;


        Destroy(this.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (this.transform.parent != null)
            this.transform.parent = null;
    }
}
