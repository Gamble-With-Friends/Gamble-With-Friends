using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraScript : MonoBehaviour
{
    void Update()
    {
        HandleRaycasting();
    }

    void HandleRaycasting()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            Vector2 mouseScreenPosition = Input.mousePosition;

            Ray ray = GetComponent<Camera>().ScreenPointToRay(mouseScreenPosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            bool result = Physics.Raycast(ray, out RaycastHit raycastHit);

            if (result)
            {
                EventManager.FireClickEvent(raycastHit.transform.GetInstanceID());
            }
        }
    }
}