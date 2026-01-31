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
			case 1:
				agileMask.SetActive(true);
				break;
			case 2:
				strongMask.SetActive(true);
				break;
			case 3:
				phaseMask.SetActive(true);
				break;
			case 4:
				harmonyMask.SetActive(true);
				break;
		}
	}
}
