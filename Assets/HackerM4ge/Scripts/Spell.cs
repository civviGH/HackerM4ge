using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public interface Spell
{

    Material GetThumbnail();

    string GetName();

    TWandAction[] UpdateSpell(
        TriggerState triggerState,
        Vector2 touchpadAxis,
        Vector3 wandPosition,
        Vector3 wandDirection
    );

    TWandAction[] Select();

    TWandAction[] Deselect();
}
