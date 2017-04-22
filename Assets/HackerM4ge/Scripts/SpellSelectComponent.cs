using System.Collections;
using UnityEngine;

using SteamVRHelper;
using System;
using System.Collections.Generic;

public class SpellSelectComponent : MonoBehaviour {
    private SteamVR_TrackedObject thisControllerObject;
    private SteamVR_TrackedObject leftControllerObject;
    private SteamVR_TrackedObject headCameraObject;

    private ControllerType controllerType;
    
    private const Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    [Range(0.01f, 0.6f)]
    public float maxBeginCastingDistance = 0.2f;
    [Range(0.1f, 1f)]
    public float minFinishCastingDistance = 0.6f;
    [Range(0.01f, 0.4f)]
    public float shrinkDiameterBy = 0.1f;
    [Range(0.1f, 5f)]
    public float castingRingFadeOutSeconds = 2f;

    public GameObject spellThumbnailPrefab;


    public Material spellSelectorMaterial;

    private SpellSelectState spellSelectState = SpellSelectState.Idle;

    public GameObject leftController;
    public GameObject headCamera;

    private GameObject castingRing;

    private enum SpellSelectState
    {
        Idle = 0,
        Casting = 1,
        Selecting = 2,
    };

    void Start()
    {
        thisControllerObject = GetComponent<SteamVR_TrackedObject>();

        leftControllerObject = leftController.GetComponent<SteamVR_TrackedObject>();
        headCameraObject = headCamera.GetComponent<SteamVR_TrackedObject>();

        controllerType = Helper.GetControllerType(ThisController());
    }

    void Update()
    {
        UpdateSpellSelectState();

        UpdateVisualization();
    }

    private void UpdateVisualization()
    {
        switch (spellSelectState)
        {
            case SpellSelectState.Idle:
                break;

            case SpellSelectState.Casting:
                UpdateCasting();
                break;

            case SpellSelectState.Selecting:
                break;

        }
    }

    private void UpdateCasting()
    {
        // Position
        castingRing.transform.position = (thisControllerObject.transform.position + leftControllerObject.transform.position) / 2;

        // Size
        var distance = Vector3.Magnitude(thisControllerObject.transform.position - leftControllerObject.transform.position);
        distance = Mathf.Min(distance, minFinishCastingDistance);
        distance = Mathf.Max(distance, maxBeginCastingDistance);
        var radius = (distance - shrinkDiameterBy) / 2f;
        castingRing.GetComponent<Torus>().segmentRadius = radius;
        castingRing.GetComponent<Torus>().tubeRadius = radius / 10f;

        castingRing.GetComponent<Torus>().RefreshTorus();

        // Orientation
        var direction = castingRing.transform.position - headCameraObject.transform.position;
        direction.y = 0;
        direction.Normalize();
        castingRing.transform.up = direction;
    }

    IEnumerator RotateRing(GameObject castingRing)
    {
        float rotation = 0;
        while(castingRing != null)
        {
            rotation = (rotation + Time.deltaTime * 10) % 360;
            // Always reset the local rotation before applying ours
            castingRing.transform.up = castingRing.transform.up;
            castingRing.transform.Rotate(Vector3.up * rotation);
            yield return null;
        }
        yield break;
    }

    private void UpdateSpellSelectState()
    {
        switch (spellSelectState)
        {
            case SpellSelectState.Idle:
                if (
                    SelectButtonPressed(ThisController())
                    && SelectButtonPressed(OtherController())
                    && Distance(thisControllerObject, leftControllerObject) < maxBeginCastingDistance
                )
                {
                    spellSelectState = SpellSelectState.Casting;
                    InitCasting();
                }
                break;

            case SpellSelectState.Casting:
                if (
                    ! SelectButtonPressed(ThisController())
                    && ! SelectButtonPressed(OtherController())
                )
                {
                    if (Distance(thisControllerObject, leftControllerObject) > minFinishCastingDistance)
                    {
                        spellSelectState = SpellSelectState.Selecting;
                        InitSelecting(castingRing.transform, castingRing.GetComponent<Torus>().segmentRadius);
                        FinishCasting();
                    }
                    else
                    {
                        spellSelectState = SpellSelectState.Idle;
                        AbortCasting();
                    }
                }
                break;

            case SpellSelectState.Selecting:
                break;

        }
    }

    private void InitSelecting(Transform castingRingTransform, float castingRingRadius)
    {
        List<Spell> listOfSpells = new List<Spell>();
        // Add spells to spellList
        listOfSpells.Add(new MeteorSpell());
        listOfSpells.Add(new LasertrapSpell());
        listOfSpells.Add(new ChaosSpell());

        for (int i = 0; i < listOfSpells.Count; i++) {
            float angle = 360 * i / listOfSpells.Count;
            var position = castingRingTransform.position + castingRingTransform.forward.normalized * castingRingRadius;
            var spellSelect = Instantiate(spellThumbnailPrefab, position, new Quaternion());
            spellSelect.GetComponent<Renderer>().material = listOfSpells[i].GetThumbnail();
        }
    }

    private void InitCasting()
    {
        castingRing = new GameObject();
        castingRing.AddComponent<Torus>();
        castingRing.GetComponent<Torus>().numSegments = 32;
        castingRing.GetComponent<Torus>().numTubes = 12;
        castingRing.GetComponent<Renderer>().material = spellSelectorMaterial;
        castingRing.GetComponent<Renderer>().material.mainTextureScale = new Vector2(30, 1);

        UpdateCasting();

        StartCoroutine(RotateRing(castingRing));
    }

    private void AbortCasting()
    {
        DestroyImmediate(castingRing);
    }

    private void FinishCasting()
    {
        StartCoroutine(FadeOutCastingRing());
    }

    IEnumerator FadeOutCastingRing()
    {
        // yield return new WaitForSeconds(2f);
        float startTime = Time.time;
        float now = Time.time;
        float timePassed = now - startTime;

        int colorId = Shader.PropertyToID("_TintColor");
        float initialAlpha = castingRing.GetComponent<Renderer>().material.GetColor(colorId).a;

        while (timePassed < castingRingFadeOutSeconds)
        {
            var color = castingRing.GetComponent<Renderer>().material.GetColor(colorId);
            color.a = initialAlpha * (castingRingFadeOutSeconds - timePassed) / castingRingFadeOutSeconds;
            castingRing.GetComponent<Renderer>().material.SetColor(colorId, color);
            yield return null;
            now = Time.time;
            timePassed = now - startTime;
        }
        DestroyImmediate(castingRing);
    }

    private bool SelectButtonPressed(SteamVR_Controller.Device controller)
    {
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return controller.GetPress(gripButton);
            case ControllerType.Vive:
                return controller.GetPress(gripButton);
            default:
                throw new NotImplementedException();
        }
    }

    private bool SelectButtonUp(SteamVR_Controller.Device controller)
    {
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return controller.GetPressUp(gripButton);
            case ControllerType.Vive:
                return controller.GetPressUp(gripButton);
            default:
                throw new NotImplementedException();
        }
    }

    private bool SelectButtonDown(SteamVR_Controller.Device controller)
    {
        switch (controllerType)
        {
            case ControllerType.Oculus:
                return controller.GetPressDown(gripButton);
            case ControllerType.Vive:
                return controller.GetPressDown(gripButton);
            default:
                throw new NotImplementedException();
        }
    }

    private SteamVR_Controller.Device ThisController()
    {
        return SteamVR_Controller.Input((int)thisControllerObject.index);
    }

    private SteamVR_Controller.Device OtherController()
    {
        return SteamVR_Controller.Input((int)leftControllerObject.index);
    }

    private double Distance(SteamVR_TrackedObject objectA, SteamVR_TrackedObject objectB)
    {
        return Vector3.Distance(objectA.transform.position, objectB.transform.position);
    }
}
