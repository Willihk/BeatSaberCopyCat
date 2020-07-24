using UnityEngine;
using System.Collections;
using System.Security.Principal;
using Unity.Mathematics;

public class AudioSpectrumManager : MonoBehaviour
{
    public static AudioSpectrumManager Instance;

    public AudioSource AudioSource;

    public int frequencyBandCount = 8;

    public float[] audioSpectrum;
    public float[] FrequencyBands;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        audioSpectrum = new float[512];
        FrequencyBands = new float[frequencyBandCount];
    }

    void Update()
    {
        AudioSource.GetSpectrumData(FrequencyBands, 0, FFTWindow.Blackman);

        //CalcFrequencyBands();
    }

    void CalcFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < frequencyBandCount; i++)
        {
            float average = 0;
            int sampleCount = (int)math.pow(2, i) * 2;

            if (i == frequencyBandCount -1)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += audioSpectrum[count] * (count + 1);
                count++;
            }

            average /= count;

            FrequencyBands[i] = average * 10;
        }
    }
}
