using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public class NewLightningSpell : Spell
{

    private Material thumbnail;

    private SpellSelectState spellSelectState = SpellSelectState.Spawning;

    private GameObject lightningPrefab;
    private ParticleSystem middlePointParticlesystem;
    private LayerMask raycastLayerMask;

    private GameObject lightningObject;
    private Transform playerEyesTransform;
    private Transform lightningStart;
    private Transform lightningMitte;
    private Transform lightningTarget;

    private Vector3 bendingStartPosition;
    private Vector3 middleBeforeBending;

    private enum SpellSelectState
    {
        Spawning = 0,
        Grabbing = 1,
        Dragging = 2,
    };

    public NewLightningSpell () {
        this.thumbnail = Resources.Load ("NewLightningThumbnailMaterial") as Material;
        this.lightningPrefab = Resources.Load ("NewLightningSpellPrefab") as GameObject;
        this.raycastLayerMask = LayerMask.GetMask ("Surfaces");
        this.playerEyesTransform = GameObject.Find ("Camera (eye)").transform;
    }

    TWandAction[] Spell.UpdateSpell (TriggerState rightTriggerState, Vector2 rightTouchpadAxis, Vector3 rightControllerPosition, Vector3 rightControllerDirection,
        TriggerState leftTriggerState, Vector2 leftTouchpadAxis, Vector3? leftControllerPosition, Vector3? leftControllerDirection)
    {
        switch (spellSelectState) {
        case SpellSelectState.Spawning:
            if (rightTriggerState.down) {
                Vector3 position = rightControllerPosition + ((rightControllerPosition - this.playerEyesTransform.position).magnitude*30f*rightControllerDirection);
                this.lightningObject = GameObject.Instantiate (this.lightningPrefab, new Vector3(0,0,0), new Quaternion ());

                Transform transformTemp = this.lightningObject.transform;
                this.lightningStart = transformTemp.GetChild (0).transform;
                this.lightningMitte = transformTemp.GetChild (1).transform;
                this.lightningTarget = transformTemp.GetChild (2).transform;

                this.lightningMitte.position = position;
                this.middlePointParticlesystem = this.lightningObject.GetComponentInChildren<ParticleSystem> ();
                this.middlePointParticlesystem.Play ();
                this.lightningObject.GetComponent<LineRenderer> ().enabled = false;

                spellSelectState = SpellSelectState.Grabbing;
            }
            break;
        case SpellSelectState.Grabbing:
            if (rightTriggerState.press) {
                this.lightningMitte.position = rightControllerPosition + ((rightControllerPosition - this.playerEyesTransform.position).magnitude * 30f * rightControllerDirection);
            } else {
                CancelSpell ();
            }
            if (rightTriggerState.up) {
                CancelSpell ();
                break;
            }
            if (leftTriggerState.down) {
                spellSelectState = SpellSelectState.Dragging;
            }
            break;
        case SpellSelectState.Dragging:
            Vector3? target = FindTarget (this.lightningMitte.position, (Vector3)leftControllerPosition - rightControllerPosition);
            if (target != null) {
                this.lightningTarget.position = (Vector3)target;
                this.lightningStart.position = rightControllerPosition;
                this.lightningMitte.position = rightControllerPosition + ((rightControllerPosition - this.playerEyesTransform.position).magnitude * 30f * rightControllerDirection);
                this.middlePointParticlesystem.Stop ();
                this.lightningObject.GetComponent<LineRenderer> ().enabled = true;
            } else {
                CancelSpell ();
            }
            if (rightTriggerState.up || leftTriggerState.up) {
                CancelSpell ();
            }
            break;
        }
        TWandAction[] actions = { };    
        return actions;
    }

    private void CancelSpell(){
        spellSelectState = SpellSelectState.Spawning;
        GameObject.Destroy (this.lightningObject);
    }

    private Vector3? FindTarget (Vector3 wandPosition, Vector3 wandDirection)
    {
        RaycastHit hitObject;
        Ray ray = new Ray (wandPosition, wandDirection);
        if (Physics.Raycast (ray, out hitObject, Mathf.Infinity, this.raycastLayerMask)) {
            return hitObject.point;
        } 
        return null;
    }

    Material Spell.GetThumbnail ()
    {
        return this.thumbnail;
    }

    string Spell.GetName ()
    {
        return "New Lightning";
    }

    TWandAction[] Spell.Select ()
    {
        TWandAction[] actions = {
            new Union2<WandAction.Drain, WandAction.Vibrate>.Case2(new WandAction.Vibrate(500)),
        };
        return actions;
    }

    TWandAction[] Spell.Deselect ()
    {
        TWandAction[] actions = { };
        return actions;
    }
}
