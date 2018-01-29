using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PostProcessing.Utilities;

[RequireComponent(typeof(FocusPuller))]
public class PlayerControl : MonoBehaviour
{
    private string path;

    private void Start()
    {
        path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + Path.DirectorySeparatorChar +
                "Endless Journey" + Path.DirectorySeparatorChar;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    void Update () {
        // Take Screenshot
		if (Input.GetKeyDown(KeyCode.E))
        {
            ScreenCapture.CaptureScreenshot(path + Path.GetRandomFileName() + ".png");
        }

        // Focus!
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            Debug.DrawRay(transform.position, ray.direction);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                GetComponent<FocusPuller>().target = hit.transform;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
