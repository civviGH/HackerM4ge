using UnityEngine;

public interface Spell {

	Material GetThumbnail ();
	string GetName();
	void ButtonUp();
	void ButtonDown();
	void ButtonPress();
}
