using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float autoRemovalTime = 3f;
    [SerializeField] float timeToRemovalAfterCollision = 0.5f;
    [SerializeField] float bulletSpeed = 10f;

    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.transform.name);
        this.transform.GetComponent<Collider>().enabled = false;
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        //Play hit sound

        //Play particle effect

        yield return new WaitForSeconds(timeToRemovalAfterCollision);
        //Destroy
        StopCoroutine(AutoDestroy());
        GameObject.Destroy(this.gameObject);
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(autoRemovalTime);
        StopCoroutine(Hit());
        GameObject.Destroy(this.gameObject);
    }

    public void Fire(Vector3 direction)
    {
        this.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed;
    }
}
