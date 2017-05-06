using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteamVRHelper;
using System;

public class TeleportController : MonoBehaviour
{
    public AudioSource teleportSound;

    private SteamVR_TrackedObject trackedObj;
    private ControllerType controllerType;

    // TODO Da das hier mit dem WandController gemeinsame ist: Konstanten vielleicht auslagern, oder anders kapseln?
    private const Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private const Valve.VR.EVRButtonId touchButtonA = Valve.VR.EVRButtonId.k_EButton_A;
    private GameObject teleportLine;
    private GameObject highlightedPlatform;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        controllerType = Helper.GetControllerType(Controller());
    }

    void Update()
    {
        ShowTeleportDirection();
        Teleport();
    }

    void ShowTeleportDirection()
    {
        if (TeleportButtonDown())
        {
            teleportLine = new GameObject();
            teleportLine.transform.position = transform.position;
            teleportLine.transform.SetParent(transform);
            teleportLine.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = teleportLine.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.blue;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            Vector3 wandDirection = Helper.WandDirection(transform, controllerType);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + 20 * wandDirection);
        }

        if (TeleportButtonPressed() && teleportLine != null)
        {
            Vector3 wandDirection = Helper.WandDirection(transform, controllerType);

            LineRenderer lineRenderer = teleportLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + 20 * wandDirection);

            GameObject nextPlatform = GetNextPlatform();
            if (nextPlatform != null)
            {
                highlightPlatform(nextPlatform);
            }
            else
            {
                unhighlightPlatform();
            }
        }
    }

    void Teleport()
    {
        if (TeleportButtonUp())
        {
            if (teleportLine != null)
            {
                GameObject nextPlatform = GetNextPlatform();
                if (nextPlatform != null)
                {
                    CameraComponent cameraComponent = transform.parent.gameObject.GetComponent<CameraComponent>();
                    transform.parent.gameObject.transform.position += nextPlatform.transform.position - cameraComponent.currentPlatformTransform.position;
                    cameraComponent.currentPlatformTransform = nextPlatform.transform;
                    teleportSound.Play();

                    // TODO Vielleicht wäre ein generischerer Mechanismus sinnvoll, über Event Hooks oder den WandController.
                    GetComponent<SpellSelectComponent>().Reset();
                }
            }
            unhighlightPlatform();
            Destroy(teleportLine);
            teleportLine = null;
        }
    }

    void highlightPlatform(GameObject nextPlatform)
    {
        unhighlightPlatform();
        nextPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * 0.5f);
        highlightedPlatform = nextPlatform;
    }

    void unhighlightPlatform()
    {
        if (highlightedPlatform != null)
        {
            highlightedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            highlightedPlatform = null;
        }
    }

    GameObject GetNextPlatform()
    {
        RaycastHit hitObject;
        Ray ray = new Ray(transform.position, Helper.WandDirection(transform, controllerType));
        if (Physics.Raycast(ray, out hitObject))
        {
            if (hitObject.collider.transform.parent != null)
            {
                GameObject nextPlatform = hitObject.collider.transform.parent.gameObject;
                if (hitObject.collider.isTrigger && nextPlatform.GetComponent<PlatformComponent>() != null)
                {
                    return nextPlatform;
                }
            }
        }
        return null;
    }

    private bool TeleportButtonPressed()
    {
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return Controller().GetPress(touchButtonA);
            case ControllerType.Vive:
                return Controller().GetPress(touchpad);
            default:
                throw new NotImplementedException();
        }
    }

    private bool TeleportButtonUp()
    {
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return Controller().GetPressUp(touchButtonA);
            case ControllerType.Vive:
                return Controller().GetPressUp(touchpad);
            default:
                throw new NotImplementedException();
        }
    }

    private bool TeleportButtonDown()
    {
        Direction direction = Helper.GetDirectionOfTouchpad(Controller());
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return Controller().GetPressDown(touchButtonA);
            case ControllerType.Vive:
                return Controller().GetPressDown(touchpad) && direction == Direction.up;
            default:
                throw new NotImplementedException();
        }
    }

    private SteamVR_Controller.Device Controller()
    {
        return SteamVR_Controller.Input((int)trackedObj.index);
    }
}
