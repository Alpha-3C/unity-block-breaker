using System;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEngine;

[InitializeOnLoad]
internal static class AutoDemoRecorder
{
    private const string SettingsPath = "Assets/Editor/DemoRecorderSettings.asset";
    private static RecorderController controller;

    static AutoDemoRecorder()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            var settings = AssetDatabase.LoadAssetAtPath<DemoRecorderSettings>(SettingsPath);
            if (settings != null && settings.AutoRecordOnPlay)
                StartRecording();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            StopRecording();
        }
    }

    private static void StartRecording()
    {
        if (controller != null)
            return;

        try
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDirectory = Path.Combine(projectRoot, "Recordings");
            Directory.CreateDirectory(outputDirectory);

            var movie = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            movie.name = "Automatic Demo Movie";
            movie.Enabled = true;
            // Keep the default Game View input. MovieRecorderSettings configures it
            // to use even dimensions for MP4; replacing it loses that setting.
            movie.CaptureAudio = true;
            movie.EncoderSettings = new CoreEncoderSettings
            {
                Codec = CoreEncoderSettings.OutputCodec.MP4,
                EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
            };
            movie.OutputFile = Path.Combine(outputDirectory,
                "demo_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")).Replace('\\', '/');

            var recordingSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recordingSettings.AddRecorderSettings(movie);
            recordingSettings.SetRecordModeToManual();
            // Variable playback keeps gameplay tied to real time while recording.
            // FrameRate is the maximum capture rate in this mode.
            recordingSettings.FrameRatePlayback = FrameRatePlayback.Variable;
            recordingSettings.FrameRate = 60;
            recordingSettings.ExitPlayMode = false;

            controller = new RecorderController(recordingSettings);
            controller.PrepareRecording();
            if (!controller.StartRecording())
            {
                controller.StopRecording();
                Debug.LogError("Demo Recorder failed to start. Check the Recorder messages in Console.");
                controller = null;
            }
            else
            {
                Debug.Log("Demo Recorder started. Video will be saved under Recordings/.");
            }
        }
        catch (Exception exception)
        {
            try
            {
                controller?.StopRecording();
            }
            catch (Exception cleanupException)
            {
                Debug.LogException(cleanupException);
            }
            controller = null;
            Debug.LogException(exception);
        }
    }

    private static void StopRecording()
    {
        if (controller == null)
            return;

        try
        {
            controller.StopRecording();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            controller = null;
        }
    }
}
