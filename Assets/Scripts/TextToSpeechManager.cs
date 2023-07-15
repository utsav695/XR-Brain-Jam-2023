using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using UnityEngine;

public class TextToSpeechManager : MonoBehaviour
{
    [SerializeField]
    private string greetingNarration;

    public static bool SetupComplete { get; private set; }

    private static Voice activeVoice;
    private static bool voiceFailed;
    private static bool greetingsComplete;
    private static bool noInputReceived;

    private bool eventRegisteredVoice;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Restart"))
        {
            PlayerPrefs.SetInt("Restart", 0);
            PlayerPrefs.DeleteKey("Restart");
        }

        voiceFailed = true;
        greetingsComplete = false;
        noInputReceived = false;
        SetupComplete = false;

        _ = GCTextToSpeech.Instance.GetVoices(new GetVoicesRequest()
        {
            languageCode = "en_US"
        });

        RegisterEvents();
    }

    private void OnDestroy()
    {
        RegisterEvents(false);
    }

    private void Update()
    {
        RegisterEvents();
    }

    private void RegisterEvents(bool isRegister = true)
    {
        if (!GCTextToSpeech.Instance)
        {
            return;
        }

        if (!eventRegisteredVoice && isRegister)
        {
            GCTextToSpeech.Instance.GetVoicesSuccessEvent += OnGetVoicesSuccess;
            GCTextToSpeech.Instance.SynthesizeSuccessEvent += OnSynthesizeSuccess;
            GCTextToSpeech.Instance.GetVoicesFailedEvent += OnGetVoiceFailed;
            GCTextToSpeech.Instance.SynthesizeFailedEvent += OnSynthesizeFailed;

            eventRegisteredVoice = true;
        }
        else if (eventRegisteredVoice && !isRegister)
        {
            GCTextToSpeech.Instance.GetVoicesSuccessEvent -= OnGetVoicesSuccess;
            GCTextToSpeech.Instance.SynthesizeSuccessEvent -= OnSynthesizeSuccess;
            GCTextToSpeech.Instance.GetVoicesFailedEvent -= OnGetVoiceFailed;
            GCTextToSpeech.Instance.SynthesizeFailedEvent -= OnSynthesizeFailed;
        }
    }

    public static void GreetingsComplete()
    {
        if (greetingsComplete)
        {
            return;
        }

        if (noInputReceived)
        {
            noInputReceived = false;
        }

        greetingsComplete = true;
    }

    public static void Synthesize(string content)
    {
        //error if voice failed or message is empty
        if (voiceFailed || string.IsNullOrEmpty(content))
        {
            return;
        }

        if (SetupComplete)
        {
            GCTextToSpeech.Instance.CancelAllRequests();
        }

        if (GeneralConfig.Config.betaAPI)
        {
            _ = GCTextToSpeech.Instance.Synthesize(content, new VoiceConfig()
            {
                gender = activeVoice.ssmlGender,
                languageCode = activeVoice.languageCodes[0],
                name = activeVoice.name
            },
            false, //ssml
            1, //pitch
            1, //speaking rate
            activeVoice.naturalSampleRateHertz,
            new string[0] { },
            new Enumerators.TimepointType[] { Enumerators.TimepointType.TIMEPOINT_TYPE_UNSPECIFIED });
        }
        else
        {
            _ = GCTextToSpeech.Instance.Synthesize(content, new VoiceConfig()
            {
                gender = activeVoice.ssmlGender,
                languageCode = activeVoice.languageCodes[0],
                name = activeVoice.name
            },
            false, //ssml
            1, //pitch
            1, //speaking rate
            activeVoice.naturalSampleRateHertz,
            new string[0] { });
        }
    }

    private void OnGetVoicesSuccess(GetVoicesResponse response, long requestId)
    {
        voiceFailed = false;
        activeVoice = response.voices[0];
        Synthesize(greetingNarration.Replace("VR", "ver"));
    }

    private void OnSynthesizeSuccess(PostSynthesizeResponse response, long requestId)
    {
        SetupComplete = true;
        AudioClip clip = GCTextToSpeech.Instance.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
        AudioManager.Play(clip);
    }

    private void OnGetVoiceFailed(string error, long requestId)
    {
        SetupComplete = true;
        Debug.Log("GetVoiceFailed!\n" + error + "\nRequestId: " + requestId.ToString());
    }

    private void OnSynthesizeFailed(string error, long requestId)
    {
        Debug.Log("Synthesize Failed!\n" + error + "\nRequestId: " + requestId.ToString());
    }
}
