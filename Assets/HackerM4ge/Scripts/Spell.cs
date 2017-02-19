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

    void Select();

    void Deselect();
}
