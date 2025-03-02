using UnityEngine;
using System.Collections;
using static PlanetList;

public class RevealNightEarthTexture : MonoBehaviour {

	Transform tfLight;
	float _distance;
	Vector3 _dir;
	GameObject _planet;
    // Use this for initialization
    public void Init(float distance, GameObject planet) {
		var goLight = GameObject.Find ("RevealingLight");
		//_distance = distance;
		_planet = planet;
		_distance = Vector3.Distance(GlobalProperties.sun.transform.position, _planet.transform.position);
        if (goLight)
		{
			tfLight = goLight.transform;
		}
	}
	

	// Update is called once per frame
	void Update () {
		if(tfLight)
		{
            //tfLight.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _distance);
            tfLight.position = (_planet.transform.position - GlobalProperties.sun.transform.position).normalized * _distance * 1.1f /** 2f* GlobalProperties.globalDistanceMultiplier*/;
			tfLight.LookAt(_planet.transform);
            GetComponent<Renderer>().material.SetVector("_LightPos", tfLight.position);
            GetComponent<Renderer>().material.SetVector("_LightDir", tfLight.forward);
		}
	}
}
