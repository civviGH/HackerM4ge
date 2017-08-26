using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public class LightningSpell : Spell
{
    
    private Material thumbnail;

    private SpellSelectState spellSelectState = SpellSelectState.Aiming;

    private GameObject lightningPrefab;
    private LayerMask raycastLayerMask;

    private GameObject lightningObject;
    private Transform lightningStart;
    private Transform lightningMitte;
    private Transform lightningTarget;

    private Vector3 bendingStartPosition;
    private Vector3 middleBeforeBending;

    private enum SpellSelectState
    {
        Aiming = 0,
        Grabbing = 1,
        Bending = 2,
    };

    public LightningSpell () {
        this.thumbnail = Resources.Load ("LightningThumbnailMaterial") as Material;
        this.lightningPrefab = Resources.Load ("LightningSpellPrefab") as GameObject;
        this.raycastLayerMask = LayerMask.GetMask ("Surfaces");
    }
	
    TWandAction[] Spell.UpdateSpell (TriggerState rightTriggerState, Vector2 rightTouchpadAxis, Vector3 rightControllerPosition, Vector3 rightControllerDirection,
        TriggerState leftTriggerState, Vector2 leftTouchpadAxis, Vector3? leftControllerPosition, Vector3? leftControllerDirection)
    {
        switch (spellSelectState) {
        case SpellSelectState.Aiming:
            if (rightTriggerState.down) {
                Vector3? target = FindTarget (rightControllerPosition, rightControllerDirection);
            
                if (target != null) {
                    this.lightningObject = GameObject.Instantiate (this.lightningPrefab, rightControllerPosition-new Vector3(-10,-10,-10), new Quaternion ());
                    Transform transformTemp = this.lightningObject.transform;
                    this.lightningStart = transformTemp.GetChild (0).transform;
                    this.lightningMitte = transformTemp.GetChild (1).transform;
                    this.lightningTarget = transformTemp.GetChild (2).transform;

                    this.lightningStart.position = rightControllerPosition;
                    this.lightningTarget.position = (Vector3)target;
                    this.lightningMitte.position = (this.lightningStart.position + this.lightningTarget.position) / 2;

                    spellSelectState = SpellSelectState.Grabbing;
                }
            }
            break;
        case SpellSelectState.Grabbing:
            if (rightTriggerState.press) {
                Vector3? target = FindTarget (rightControllerPosition, rightControllerDirection);
                if (target != null) {
                    this.lightningStart.position = rightControllerPosition;
                    this.lightningTarget.position = (Vector3)target;
                    this.lightningMitte.position = (this.lightningStart.position + this.lightningTarget.position) / 2;
                } else {
                    CancelSpell ();
                }
            } else {
                CancelSpell ();
            }
            if (leftTriggerState.down) {
                this.bendingStartPosition = (Vector3) leftControllerPosition;
                this.middleBeforeBending = this.lightningMitte.position;
                spellSelectState = SpellSelectState.Bending;
            }
            break;
        case SpellSelectState.Bending:
            if (leftTriggerState.up) {
                spellSelectState = SpellSelectState.Grabbing;
                break;
            }
            if (rightTriggerState.press) {
                Vector3? target = FindTarget (rightControllerPosition, rightControllerDirection);
                if (target != null) {
                    this.lightningStart.position = rightControllerPosition;
                    this.lightningTarget.position = (Vector3)target;
                    this.lightningMitte.position = (this.lightningStart.position + this.lightningTarget.position) / 2;
                    this.middleBeforeBending = this.lightningMitte.position;
                } else {
                    CancelSpell ();
                }
            } else {
                CancelSpell ();
            }
            if (leftTriggerState.down) {
                this.bendingStartPosition = (Vector3) leftControllerPosition;
            }
            Vector3 offset = (Vector3)(this.bendingStartPosition - leftControllerPosition) * 20;
            this.lightningMitte.position = this.middleBeforeBending + offset;
            break;

            // TODO spark effekt wenn man kein ziel getroffen hat
        }
        TWandAction[] actions = { };    
        return actions;
    }

    private void CancelSpell(){
        spellSelectState = SpellSelectState.Aiming;
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
        return "Lightning";
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
