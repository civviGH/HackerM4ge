using UnityEngine;

public interface Spell {

	Material GetThumbnail ();
	string GetName();
	void RightTriggerUp(Vector2 touchpadAxis);
	void RightTriggerDown(Vector2 touchpadAxis);
	void RightTriggerPress(Vector2 touchpadAxis);
}
