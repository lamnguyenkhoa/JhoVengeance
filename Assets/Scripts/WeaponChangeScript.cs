using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeScript : MonoBehaviour
{
    private Quaternion customWeaponRotation;
    private Vector3 customWeaponScale = new Vector3(0.75f, 0.65f, 0.75f);
    private Vector3 bias = new Vector3(0, 0, 0.05f);

    // Start is called before the first frame update
    private void Awake()
    {
        Vector3 rot = new Vector3(270f, 180f, 0f);
        customWeaponRotation = Quaternion.Euler(rot);
        LevelEvents.levelEvents.onOnhandWeaponChangeTriggerEnter += OnOnhandSwitch;
    }

    private void OnOnhandSwitch(WeaponScript newOnhand, bool prevWasNull)
    {
        if (prevWasNull && newOnhand != null)
        {
            // transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject _obj = new GameObject();
            _obj.AddComponent<MeshFilter>();
            _obj.AddComponent<MeshRenderer>();
            _obj.GetComponent<MeshFilter>().mesh = newOnhand.gameObject.GetComponent<MeshFilter>().mesh;
            _obj.GetComponent<MeshRenderer>().material = newOnhand.gameObject.GetComponent<MeshRenderer>().material;
            _obj.transform.localScale = customWeaponScale;
            _obj.transform.localRotation = customWeaponRotation;
            Instantiate(_obj, this.transform);
            Destroy(_obj);
        }
        else if (prevWasNull && newOnhand == null)
        {
            return;
        }
        else if (newOnhand == null && !prevWasNull)
        {
            //transform.GetChild(0).gameObject.SetActive(true);
            this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;

            Destroy(this.transform.GetChild(1).gameObject);
        }
        else
        {
            Destroy(this.transform.GetChild(1).gameObject);
            GameObject _obj = new GameObject();
            _obj.AddComponent<MeshFilter>();
            _obj.AddComponent<MeshRenderer>();
            _obj.GetComponent<MeshFilter>().mesh = newOnhand.gameObject.GetComponent<MeshFilter>().mesh;
            _obj.GetComponent<MeshRenderer>().material = newOnhand.gameObject.GetComponent<MeshRenderer>().material;
            _obj.transform.localScale = customWeaponScale;
            _obj.transform.localRotation = customWeaponRotation;
            Instantiate(_obj, this.transform);
            Destroy(_obj);
        }
    }

    private void OnDestroy()
    {
        LevelEvents.levelEvents.onOnhandWeaponChangeTriggerEnter -= OnOnhandSwitch;
    }
}