using UnityEngine;

public class Mask : MonoBehaviour
{
	public GameObject agileMask, strongMask, phaseMask, harmonyMask;

	public void DisplayMask(int maskIndex)
	{
		agileMask.SetActive(false);
		strongMask.SetActive(false);
		phaseMask.SetActive(false);
		harmonyMask.SetActive(false);
		switch(maskIndex)
		{
			case 0:
				agileMask.SetActive(true);
				break;
			case 1:
				strongMask.SetActive(true);
				break;
			case 2:
				phaseMask.SetActive(true);
				break;
			case 3:
				harmonyMask.SetActive(true);
				break;
		}
	}
}
