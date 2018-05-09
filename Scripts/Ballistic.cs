using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballistic : MonoBehaviour {


    public Transform target;
    public float launchSpeed;
    private Vector3 beginPos;
    private Vector3 targetPos;
    public  float maxHeight;
    private Vector3 controlPos;
    private float distance;
    public float velocity;
    private float timeCost;
    private float timer;

	// Use this for initialization
	void Start () {

        beginPos = transform.position;
        targetPos = target.position;
        distance = Vector3.Distance(beginPos, targetPos);
        timeCost = distance / velocity;
        controlPos = beginPos + 0.5F * (targetPos - beginPos);
        controlPos.y += maxHeight;


		
	}
	
	// Update is called once per frame
	void Update () {
		
        
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "terrain")
        {
            
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        
        if(timer < timeCost)
        {
            timer += Time.deltaTime;
            float t = timer / timeCost;
            Vector3 pos = (1 - t) * (beginPos + (controlPos - beginPos) * t) + t * (controlPos + (targetPos - controlPos) * t);
            transform.position = pos;
            

        }
        else
        {
            Destroy(gameObject);
        }



    }
}
