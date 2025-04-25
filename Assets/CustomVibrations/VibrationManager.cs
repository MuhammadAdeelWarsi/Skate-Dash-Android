using UnityEngine;
using MoreMountains.NiceVibrations;


public class VibrationManager : MonoBehaviour
{
    private bool isVibrationEnabled;
    public bool IsVibrationEnabled => isVibrationEnabled;


    public void InitializeVibrationSettings()   //Initializes vibration settings based on saved data from settings menu
    {
        isVibrationEnabled = PrefsDataManager.Instance.LoadData(PrefsDataManager.PrefsKeys.Vibration, 1) == 1;
    }

    public void UpdateVibrationSettings(bool value)
    {
        isVibrationEnabled = value;
    }

    public void PlayVibration(HapticTypes vibrationType)    //Simple vibration
    {
        MMVibrationManager.Haptic(vibrationType);
    }
}
