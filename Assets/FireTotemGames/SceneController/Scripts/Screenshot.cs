using System.IO;
using UnityEngine;

public class Screenshot : MonoBehaviour
{

	void Update ()
	{
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K) == true)
		{
			// create screenshot folder if it doesn't exists
			string folderpath = Application.dataPath + "/../Screenshots";
			if (Directory.Exists(folderpath) == false)
			{
				Directory.CreateDirectory(folderpath);
			}

            // multiplier to current resolution
			int superSize = 1;

			// save screenshot with date, time and resolution
			string resolution = Screen.width * superSize + "x" + Screen.height * superSize + "_";
			string dateAndTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			string filename = folderpath + "/screen_" + dateAndTime + resolution + ".png";
			ScreenCapture.CaptureScreenshot(filename, superSize);
			Debug.Log("Screenshot saved to: " + filename);
		}
#endif
    }
}
