using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.AtmosphericScattering.Scripts
{
    public class SunMovement : MonoBehaviour
    {
        public Transform DirLightTransform;
        public float rotationSpeed = 1f;
        [Range(170, 179)]
        public float sunsetStart = 170;
        [Range(1, 10)]
        public float nightRotationRatio = 6;
        [Range(1, 10)]
        public float daytimeRotationRatio = 5;

        private const float ratioXtoY = 2;
        private const float sunsetEnd = 181;
        private float timeOfDaySpeed = 1;

        public void Update()
        {
            // Slow down on sunrises and sunsets, speed up on night and daytime. Force positive number
            float currentTOD = 180 - DirLightTransform.rotation.eulerAngles.x;

            // Use default speed during golden hours
            if (sunsetStart <= currentTOD && currentTOD <= sunsetEnd)
            {
                // Change speed smoothly
                timeOfDaySpeed = Mathf.Lerp(timeOfDaySpeed, 1, Time.deltaTime);
            } else if (currentTOD > sunsetEnd)
            {
                // Nighttime, change speed to night
                timeOfDaySpeed = Mathf.Lerp(timeOfDaySpeed, nightRotationRatio, Time.deltaTime);
            } else if (currentTOD < sunsetEnd)
            {
                // Day time, change speed to day
                timeOfDaySpeed = Mathf.Lerp(timeOfDaySpeed, daytimeRotationRatio, Time.deltaTime);
            }

            //Debug.Log(Time.deltaTime + " " + timeOfDaySpeed);

            DirLightTransform.Rotate(0, Time.deltaTime * rotationSpeed * timeOfDaySpeed, 0, Space.World);
            DirLightTransform.Rotate(Time.deltaTime * rotationSpeed * ratioXtoY * timeOfDaySpeed, 0, 0, Space.Self);
        }
    }
}
