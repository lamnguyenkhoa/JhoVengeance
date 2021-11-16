using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWeaponChangeScript : MonoBehaviour
{
    private Vector3 customWeaponScale = new Vector3(19f, 16.5f, 19f);
    private Quaternion customWeaponRotation = Quaternion.AngleAxis(85f, Vector3.right);
    private Vector3 customWeaponBias = new Vector3(0f, 0f, 0.09f);

    private void Awake()
    {
        LevelEvents.levelEvents.onOffhandWeaponChangeTriggerEnter += OnOffhandSwitch;
    }

    private void OnOffhandSwitch(WeaponScript newOffhand, bool prevNull)
    {
        if (prevNull && newOffhand != null)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            GameObject _obj = new GameObject();
            _obj.AddComponent<MeshFilter>();
            _obj.AddComponent<MeshRenderer>();
            _obj.GetComponent<MeshFilter>().mesh = newOffhand.gameObject.GetComponent<MeshFilter>().mesh;
            _obj.GetComponent<MeshRenderer>().material = newOffhand.gameObject.GetComponent<MeshRenderer>().material;
            _obj.transform.localScale = customWeaponScale;
            _obj.transform.localRotation = customWeaponRotation;
            _obj.transform.position += customWeaponBias;
            Instantiate(_obj, transform);
            Destroy(_obj);
        }
        else if (prevNull && newOffhand == null)
        {
            return;
        }
        else if (newOffhand == null && !prevNull)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            Destroy(this.transform.GetChild(1).gameObject);
        }
        else
        {
            Destroy(this.transform.GetChild(1).gameObject);
            GameObject _obj = new GameObject();
            _obj.AddComponent<MeshFilter>();
            _obj.AddComponent<MeshRenderer>();
            _obj.GetComponent<MeshFilter>().mesh = newOffhand.gameObject.GetComponent<MeshFilter>().mesh;
            _obj.GetComponent<MeshRenderer>().material = newOffhand.gameObject.GetComponent<MeshRenderer>().material;
            _obj.transform.localScale = customWeaponScale;
            _obj.transform.localRotation = customWeaponRotation;
            _obj.transform.position += customWeaponBias;
            Instantiate(_obj, this.transform);
            Destroy(_obj);
        }
    }

    private void OnDestroy()
    {
        LevelEvents.levelEvents.onOffhandWeaponChangeTriggerEnter -= OnOffhandSwitch;
    }
}