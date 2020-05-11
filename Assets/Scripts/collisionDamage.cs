using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class collisionDamage : MonoBehaviour
{
    public float damage = 0f;
    public float damageIntensity = 0.0001f;
    public float explosionForce = 1000f;
    public float explosionRadius = 1f;
    public float upwardsModifier = 1f;
    public CinemachineTargetGroup cmtg;
    
    public GameObject solid;
    public GameObject exploded;
    
    private Material solidMat;
    private Rigidbody solidRB;
    private GameObject[] explodedChildren;
    private Material[] explodedMat;
    private Rigidbody[] explodedRB;

    private int count;
    
    // Start is called before the first frame update
    void Start()
    {
        count = exploded.transform.childCount;
        solidMat = solid.GetComponent<Renderer>().material;
        solidRB = GetComponent<Rigidbody>();
        explodedChildren = new GameObject[count];
        explodedMat = new Material[count];
        explodedRB = new Rigidbody[count];
        for(int i = 0; i < count; i++)
        {
            explodedChildren[i] = exploded.transform.GetChild(i).gameObject;
            explodedMat[i] = exploded.transform.GetChild(i).GetComponent<Renderer>().material;
            explodedRB[i] = exploded.transform.GetChild(i).GetComponent<Rigidbody>();
        }

        cmtg = GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    void OnCollisionEnter(Collision collision)
    {
        damage += collision.impulse.magnitude * damageIntensity;
        Mathf.Clamp(damage, 0f, 1f);
        solidMat.SetFloat("damageBlend", damage);
        if (damage >= 1f)
        {
            Explode();
        }
    }

    public void Explode()
    {
        exploded.transform.position = solid.transform.position;
        exploded.transform.rotation = solid.transform.rotation;
        solid.SetActive(false);
        solidRB.detectCollisions = false;
        exploded.SetActive(true);
        cmtg.RemoveMember(transform);
        for (int i = 0; i < count; i++)
        {
            explodedMat[i].SetFloat("damageBlend", damage);
            explodedRB[i].AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            StartCoroutine(FreezeCollisions(i,3f));
        }
    }

    private IEnumerator FreezeCollisions(int i, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        explodedRB[i].detectCollisions = false;
        yield return new WaitForSeconds(timeout);
        Destroy(gameObject);
    }
}
