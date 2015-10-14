using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShutdownOp : Operation {

	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};

	public override System.Type[] getTriggers() {
		return triggers;
	}

	public override void perform(GameObject instigator, Trigger trig) {
		AudioSource audioSource = parent.GetComponent<AudioSource>();
		if (audioSource != null) {
			audioSource.PlayOneShot(audioSource.clip);
		}
		AbstractPowerSource[] powerSources = parent.GetComponents<AbstractPowerSource>();
        foreach (AbstractPowerSource source in powerSources) {
			source.disable();
		}
	}
}
