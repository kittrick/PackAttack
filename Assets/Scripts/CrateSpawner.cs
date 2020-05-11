using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrateSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject dropTarget;
    public float dropDistance = 5f;
    public TextMeshProUGUI scoreText;
    public int highScore = 0;
    public CinemachineTargetGroup cmtg;
    
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            dropTarget.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = getHitPoint();
            dropTarget.transform.position = pos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (dropTarget.transform.position.y > highScore)
            {
                highScore = Mathf.CeilToInt(dropTarget.transform.position.y);
                scoreText.text = highScore.ToString();
            }
            DropCrate();
            dropTarget.SetActive(false);
        }
        
        // Explode crate
        if (Input.GetMouseButtonUp(1))
        {
            GameObject go = getHitObject();
            if (go.GetComponent<collisionDamage>() != null)
            {
                go.GetComponent<collisionDamage>().Explode();   
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!Mathf.Approximately(Input.GetAxis("Horizontal"),0))
        {
            dropTarget.transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Horizontal"));
        }
    }

    private void DropCrate()
    {
        Vector3 pos = getHitPoint();
        GameObject go = Instantiate(prefab, pos, dropTarget.transform.rotation, transform);
        cmtg.AddMember(go.transform, 1f, 3f);
    }

    private Vector3 getHitPoint()
    {
        RaycastHit hit;
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
        return hit.point + Vector3.up * dropDistance;
    }
    
    private GameObject getHitObject()
    {
        RaycastHit hit;
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
        return hit.collider.gameObject;
    }
}
