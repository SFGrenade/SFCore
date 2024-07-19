using System;
using System.IO;
using System.Text;
using UnityEngine;
using WavLib;

namespace SFCore.Utils;

public class WavUtils
{
    /// <summary>
    /// Converts audio data from a stream into an AudioClip using WavLib.
    /// </summary>
    /// <returns>The AudioClip.</returns>
    /// <param name="dataStream">The wav data stream</param>
    public static AudioClip ToAudioClip(Stream dataStream, string origName = "")
    {
        WavData.Inspect(dataStream, msg => InternalLogger.LogDebug(msg));
        WavData wavData = new WavData();
        wavData.Parse(dataStream, msg => InternalLogger.LogDebug(msg));
        dataStream.Close(); // just to be sure, closing the stream here

        float[] wavSoundData = wavData.GetSamples();
        AudioClip audioClip = AudioClip.Create(origName, wavSoundData.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels,
            (int)wavData.FormatChunk.SampleRate, false);
        audioClip.SetData(wavSoundData, 0);
        return audioClip;
    }

    /// <summary>
    /// Converts audio data from a file into an AudioClip using WavLib.
    /// </summary>
    /// <returns>The AudioClip.</returns>
    /// <param name="filePath">The wav file</param>
    public static AudioClip ToAudioClip(string filePath)
    {
        return ToAudioClip(File.Open(filePath, FileMode.Open), Path.GetFileNameWithoutExtension(filePath));
    }

    /// <summary>
    /// Converts audio data from a byte array into an AudioClip using WavLib.
    /// </summary>
    /// <returns>The AudioClip.</returns>
    /// <param name="wavData">The wav data</param>
    public static AudioClip ToAudioClip(byte[] wavData, string origName = "")
    {
        MemoryStream memoryStream = new MemoryStream(wavData, false);
        return ToAudioClip(memoryStream, origName);
    }
}