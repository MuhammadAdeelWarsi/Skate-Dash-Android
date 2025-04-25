using System;
using System.Collections;
using UnityEngine;
using CustomControllers;
using System.Linq;


public class EnvironmentHandler : MonoBehaviour
{
    [Header("---Weather and Timezone---")]
    [SerializeField] PrototypeDay dayPhase;
    [SerializeField] PrototypeNight nightPhase;
    [SerializeField] Sky currentSky = Sky.Day;
    [SerializeField] int skyPhaseMinDelay = 120, skyPhaseMaxDelay = 240;  //Phase change delay in seconds
    [SerializeField] int skyChangeDelay;

    [Header("---Fly Insects---")]
    [SerializeField] FlyInsect[] flyInsects;
    [SerializeField] int flyInsectsMinSpawnDelay = 60, flyInsectsMaxSpawnDelay = 90;  //Spawn time delay in seconds
    [SerializeField] int flySpawnDelay;
    private FlyInsect dragonFly;


    private void Awake()
    {
        PrototypeDino.onInitializedEvent += () => StartCoroutine(CheckFlyInsectsTime());
        PrototypeDino.onInitializedEvent += () => StartCoroutine(CheckSkyTime());

        dragonFly = flyInsects.Single(flyInsect => flyInsect._Name == FlyInsect.Name.Dragonfly);
    }
    private void Start()
    {
        GameManager.Instance.UiManager.onAudioToggleEvent += ToggleEnvironmentAudios;
    }

    private void ToggleEnvironmentAudios(PrefsDataManager.PrefsKeys audioKey, bool value)
    {
        switch (audioKey)
        {
            case PrefsDataManager.PrefsKeys.Music:
                if (value)  //Enable music
                {
                    AudioManager.AudioNames targetMusic = currentSky == Sky.Day ? AudioManager.AudioNames.DayMusic : AudioManager.AudioNames.NightMusic;
                    GameManager.Instance.AudioManager.UnMuteAudio(targetMusic, false);
                }
                else        //Disable music
                {
                    if (GameManager.Instance.AudioManager.IsSoundEnabled)   //Enable wind blow sound if sound is not disabled yet
                        GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.WindBlowing);
                    else
                        GameManager.Instance.AudioManager.MuteAudio(AudioManager.AudioNames.WindBlowing);
                }
                break;

            case PrefsDataManager.PrefsKeys.Sound:
                if (value)  //Enable sound
                {
                    if (!GameManager.Instance.Player.IsInitialized)
                        GameManager.Instance.AudioManager.UnMuteAudio(AudioManager.AudioNames.WindBlowing, true);
                    else if(!GameManager.Instance.AudioManager.IsMusicEnabled)
                        GameManager.Instance.AudioManager.UnMuteAudio(AudioManager.AudioNames.WindBlowing, false);

                    if (dragonFly.gameObject.activeSelf) //Dragonfly is currently active in environment
                        GameManager.Instance.AudioManager.UnMuteAudio(AudioManager.AudioNames.Dragonfly, false);
                }
                else        //Disable sound
                {
                    //Gameplay has not started yet or music is also not allowed
                    if (!GameManager.Instance.Player.IsInitialized || !GameManager.Instance.AudioManager.IsMusicEnabled)
                        GameManager.Instance.AudioManager.MuteAudio(AudioManager.AudioNames.WindBlowing);

                    if (dragonFly.gameObject.activeSelf)
                        GameManager.Instance.AudioManager.MuteAudio(AudioManager.AudioNames.Dragonfly);
                }
                break;
        }
    }

    #region Fly Insects
    private IEnumerator CheckFlyInsectsTime()   //Updates and checks time to spawn any of the flies
    {
        int elapsedSeconds = 0;
        flySpawnDelay = GetSpawnDelay(EnvironmentalObjects.FlyInsect);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            elapsedSeconds += 1;

            if(elapsedSeconds > flySpawnDelay)    //Fly can be spawn now
            {
                FlyInsect currentFlyInsect = flyInsects[UnityEngine.Random.Range(0, flyInsects.Length)];
                currentFlyInsect.gameObject.SetActive(true);

                elapsedSeconds = 0;

                yield return new WaitForSeconds(2f);    //Delay to let complete transition of Fly for avoiding wrong state info
                yield return new WaitForSeconds(currentFlyInsect.GetActiveState().length);  //Wait for animation to complete
                currentFlyInsect.gameObject.SetActive(false);

                flySpawnDelay = GetSpawnDelay(EnvironmentalObjects.FlyInsect);
            }
        }
    }
    #endregion

    #region Sky
    private IEnumerator CheckSkyTime()  //Updates and checks time to change the sky
    {
        int elapsedSeconds = 0;
        Func<bool> delayBeforeNextCheck;
        skyChangeDelay = GetSpawnDelay(EnvironmentalObjects.Sky);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            elapsedSeconds += 1;

            if (elapsedSeconds > skyChangeDelay)    //Skies can change now (either to night or day)
            {
                elapsedSeconds = 0;
                if(currentSky == Sky.Day)
                {
                    nightPhase.StartCycle();    //Starting night phase
                    currentSky = Sky.Night;
                    delayBeforeNextCheck = () => nightPhase.HasCycleStarted();
                }
                else
                {
                    nightPhase.EndCycle();      //Ending night phase
                    currentSky = Sky.Day;
                    delayBeforeNextCheck = () => nightPhase.HasCycleEnded();
                }

                yield return new WaitUntil(delayBeforeNextCheck);  //Wait for sky animation to complete
                skyChangeDelay = GetSpawnDelay(EnvironmentalObjects.Sky);
            }
        }
    }
    #endregion

    private int GetSpawnDelay(EnvironmentalObjects targetEnvironmentalObject)
    {
        switch (targetEnvironmentalObject)
        {
            case EnvironmentalObjects.FlyInsect:
                return UnityEngine.Random.Range(flyInsectsMinSpawnDelay, flyInsectsMaxSpawnDelay + 1);

            case EnvironmentalObjects.Sky:
                return UnityEngine.Random.Range(skyPhaseMinDelay, skyPhaseMaxDelay + 1);

            default:    //No such object in enviro has spawn delay
                return 0;
        }
    }


    #region Enums
    private enum EnvironmentalObjects
    {
        FlyInsect,
        Sky
    }

    private enum Sky
    {
        Day,
        Night
    }
    #endregion
}
