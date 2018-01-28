using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing.Utilities;

[RequireComponent(typeof(FocusPuller))]
public class ManualFocus : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.F))
        {
            // Focus!
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            Debug.DrawRay(transform.position, ray.direction);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                GetComponent<FocusPuller>().target = hit.transform;
            }
        }
	}
}
