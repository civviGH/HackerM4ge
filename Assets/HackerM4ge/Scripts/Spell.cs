using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public interface Spell
{

    Material GetThumbnail();

    string GetName();

    TWandAction[] UpdateSpell(
        TriggerState rightTriggerState,
        Vector2 rightTouchpadAxis,
        Vector3 rightControllerPosition,
        Vector3 rightControllerDirection,

        TriggerState leftTriggerState,
        Vector2 leftTouchpadAxis,
        Vector3? leftControllerPosition,
        Vector3? leftControllerDirection
    );

    TWandAction[] Select();

    TWandAction[] Deselect();
}
