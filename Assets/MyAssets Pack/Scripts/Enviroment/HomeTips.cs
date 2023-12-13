using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HomeTips : MonoBehaviour
{
    public GameObject weapon;
    public Transform DropPoiont;
    private bool canDrop;
    private float dropTime = 5;
    private void Start()
    {
        canDrop = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canDrop)
        {
            Invoke("DropWeapon", 5);
            print("等待掉落");
            Debug.Log(dropTime -= Time.deltaTime);
            canDrop = false;
        }
    }

    private void DropWeapon()
    {
        Instantiate(weapon, DropPoiont);
    }
}
