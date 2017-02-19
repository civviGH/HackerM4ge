using UnityEngine;

public interface Spell {

	Material GetThumbnail ();

	string GetName();

	void UpdateSpell(
		TriggerState triggerState, 
		Vector2 touchpadAxis, 
		Vector3 wandPosition,
		Vector3 wandDirection
	);

    Union2<WandAction.Drain, WandAction.Vibrate> Select();

    void Deselect();
}
