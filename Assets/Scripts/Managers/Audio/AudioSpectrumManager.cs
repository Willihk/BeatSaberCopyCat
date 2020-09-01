using UnityEngine;
using System.Collections;
using System.Security.Principal;
using Unity.Mathematics;

namespace BeatGame.Logic.Audio
{
    public class AudioSpectrumManager : MonoBehaviour
    {
        public static AudioSpectrumManager Instance;

        public AudioSource audioSource;
        public DoubleAudioSource doubleAudioSource;

        public AnimationCurve FrequencyDistributionCurve;
        float[] frequencyDistribution = new float[512];

        [HideInInspector]
        public float[] AudioBand, AudioBandBuffer, stereoBandSpread;
        [HideInInspector]
        public float Amplitude, AmplitudeBuffer;

        private float[] samplesLeft = new float[512];

        private float[] frequencyBand;
        private float[] bandBuffer;
        private float[] bufferDecrease;
        private float[] freqBandHighest;

        private float amplitudeHighest;

        [Range(0, 512)]
        public int FrequencyBands = 64;
        public float AudioProfile;

        public FFTWindow FrequencyReadMethod;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            frequencyBand = new float[FrequencyBands];
            bandBuffer = new float[FrequencyBands];
            bufferDecrease = new float[FrequencyBands];
            freqBandHighest = new float[FrequencyBands];
            AudioBand = new float[FrequencyBands];
            AudioBandBuffer = new float[FrequencyBands];

            GetFrequencyDistribution();
            MakeAudioProfile(AudioProfile);
        }

        void Update()
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            BandBuffer();
            CreateAudioBands();
        }

        void GetSpectrumAudioSource()
        {
            //if (doubleAudioSource.cur_is_source0)
            //{
            //    doubleAudioSource._source0.GetSpectrumData(samplesLeft, 0, FrequencyReadMethod);
            //}
            //else
            //{
            //    doubleAudioSource._source1.GetSpectrumData(samplesLeft, 0, FrequencyReadMethod);
            //}
            audioSource.GetSpectrumData(samplesLeft, 0, FrequencyReadMethod);
        }

        void GetFrequencyDistribution()
        {
            for (int i = 0; i < FrequencyBands; i++)
            {
                var eval = Mathf.RoundToInt(((float)i) / FrequencyBands);
                frequencyDistribution[i] = FrequencyDistributionCurve.Evaluate(eval);
            }
        }

        void MakeFrequencyBands()
        {
            int band = 0;
            float average = 0;
            float stereoLeft = 0;

            for (int i = 0; i < 512; i++)
            {
                var sample = (float)i;
                var current = FrequencyDistributionCurve.Evaluate(Mathf.RoundToInt(sample / 512));

                stereoLeft += (samplesLeft[i]) * (i + 1);

                average += stereoLeft;

                if (current == frequencyDistribution[band])
                {
                    if (i != 0)
                    {
                        average /= i;
                        stereoLeft /= i;
                    }

                    frequencyBand[band] = average * 10;
                    band++;
                }
            }
        }

        void BandBuffer()
        {
            for (int i = 0; i < FrequencyBands; i++)
            {
                if (frequencyBand[i] > bandBuffer[i])
                {
                    bandBuffer[i] = frequencyBand[i];
                    bufferDecrease[i] = 0.005f;
                }
                if (frequencyBand[i] < bandBuffer[i])
                {
                    bandBuffer[i] -= bufferDecrease[i];
                    bufferDecrease[i] *= 1.1f;
                }
            }
        }

        void CreateAudioBands()
        {
            for (int i = 0; i < FrequencyBands; i++)
            {
                if (frequencyBand[i] > freqBandHighest[i])
                {
                    freqBandHighest[i] = frequencyBand[i];
                }

                AudioBand[i] = Mathf.Clamp01(frequencyBand[i] / freqBandHighest[i]);
                AudioBandBuffer[i] = Mathf.Clamp01(bandBuffer[i] / freqBandHighest[i]);
            }
        }

        void GetAmplitude()
        {
            float currentAmplitude = 0;
            float currentAmplitudeBuffer = 0;

            for (int i = 0; i < FrequencyBands; i++)
            {
                currentAmplitude += AudioBand[i];
                currentAmplitudeBuffer += AudioBandBuffer[i];
            }
            if (currentAmplitude > amplitudeHighest)
            {
                amplitudeHighest = currentAmplitude;
            }
            Amplitude = currentAmplitude / amplitudeHighest;
            AmplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
        }

        void MakeAudioProfile(float audioProfile)
        {
            for (int i = 0; i < FrequencyBands; i++)
            {
                freqBandHighest[i] = audioProfile;
            }
        }
    }
}