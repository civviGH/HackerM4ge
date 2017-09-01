using UnityEngine;

using TWandAction = Union2<WandAction.Drain, WandAction.Vibrate>;

public interface Spell
{

    Material GetThumbnail();

    string GetName();

    TWandAction[] UpdateSpell(
        ControllerBridge rightController,
        ControllerBridge leftController
    );

    TWandAction[] Select();

    TWandAction[] Deselect();
}
