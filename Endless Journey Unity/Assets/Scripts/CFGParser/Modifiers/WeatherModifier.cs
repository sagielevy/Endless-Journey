using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.CFGParser.DataHolder;
using UnityEngine;

namespace Assets.Scripts.CFGParser.Modifiers
{
    public class WeatherModifier : IWorldModifier
    {
        private WindZone wind;
        private ParticleSystem hail; // TODO add more systems

        public WeatherModifier(WindZone windZone, ParticleSystem hail)
        {
            wind = windZone;
            this.hail = hail;

            // Set relative position of weather
            this.hail.transform.localPosition += new Vector3(0, Globals.maxHeight);
        }

        public IEnumerator<WaitForEndOfFrame> ModifySection(ISentenceData data)
        {
            var weatherData = data as IWeatherData;

            // Cancel any active weather effects
            if (!weatherData.IsWeatherActive())
            {
                // If active - turn inactive
                if (hail.gameObject.activeSelf)
                    hail.gameObject.SetActive(false);
            }

            while (weatherData.IsWeatherActive())
            {
                // Enable correct system once
                if (hail.gameObject.activeSelf != (weatherData.WeatherType() == WeatherTypes.Hail))
                {
                    // If type and active mismatch but weather should be active, set active/inactive according to type
                    hail.gameObject.SetActive(weatherData.WeatherType() == WeatherTypes.Hail);
                }

                // Enable/disable wind once
                if (wind.gameObject.activeSelf != weatherData.IsWindActive())
                {
                    wind.gameObject.SetActive(weatherData.IsWindActive());
                }

                // TODO make change to weather more versetile
                hail.gravityModifier = Mathf.Lerp(hail.gravityModifier, weatherData.GravityModifier(), Globals.speedChange * Time.deltaTime);
                wind.windMain = Mathf.Lerp(wind.windMain, weatherData.WindMain(), Globals.speedChange * Time.deltaTime);
                wind.windTurbulence = Mathf.Lerp(wind.windTurbulence, weatherData.WindTurbulence(), Globals.speedChange * Time.deltaTime);
                wind.windPulseMagnitude = Mathf.Lerp(wind.windPulseMagnitude, weatherData.WindPulseMag(), Globals.speedChange * Time.deltaTime);
                wind.windPulseFrequency = Mathf.Lerp(wind.windPulseFrequency, weatherData.WindPulseFreq(), Globals.speedChange * Time.deltaTime);

                

                yield return Globals.EndOfFrame;
            }
        }
    }
}
