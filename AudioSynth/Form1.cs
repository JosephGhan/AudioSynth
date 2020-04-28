using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Windows.Input;

namespace AudioSynth
{
    public partial class Form1 : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;
        bool isPlaying = false;
        SoundPlayer sound = new SoundPlayer();
        int started = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            IEnumerable<Oscillator> oscillators = this.Controls.OfType<Oscillator>().Where(o => o.On);
            Keys key = new Keys();
            Random random = new Random();
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];
            float frequency;
            int oscillatiorsCount = oscillators.Count();
            switch (e.KeyCode)
            {
                case Keys.Z:
                    frequency = 65.4f; //C2
                    key = Keys.Z;
                    break;
                case Keys.X:
                    frequency = 138.59f; //C3
                    key = Keys.X;
                    break;
                case Keys.C:
                    frequency = 261.62F; //C4
                    key = Keys.C;
                    break;
                case Keys.V:
                    frequency = 523.25f; //C5
                    key = Keys.V;
                    break;
                case Keys.B:
                    frequency = 1046.5f; //C6
                    key = Keys.B;
                    break;
                case Keys.N:
                    frequency = 2093f; //C7
                    key = Keys.N;
                    break;
                case Keys.M:
                    frequency = 4196.01f; //C8
                    key = Keys.M;
                    break;
                default:
                    return;
            }
            foreach (Oscillator oscillator in oscillators)
            {
                int samplesPerWaveLength = (int)(SAMPLE_RATE / frequency);
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);
                short tempSample;
                switch (oscillator.WaveForm)
                {
                    case WaveForm.Sine:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)) /oscillatiorsCount);
                        }
                        break;
                    case WaveForm.Square:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16((short.MaxValue * Math.Sign(Math.Sin((Math.PI * 2 * frequency) / SAMPLE_RATE * i))) / oscillatiorsCount);
                        }
                        break;
                    case WaveForm.Saw:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            tempSample = -short.MaxValue;
                            for (int j = 0; j < samplesPerWaveLength && i < SAMPLE_RATE; j++)
                            {
                                tempSample += ampStep;
                                wave[i++] += Convert.ToInt16(tempSample / oscillatiorsCount);
                            }
                            i--;
                        }
                        break;
                    case WaveForm.Triangle:
                        tempSample = -short.MaxValue;
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            if(Math.Abs(tempSample + ampStep) > short.MaxValue)
                            {
                                ampStep = (short)-ampStep;
                            }
                            tempSample += ampStep;
                            wave[i] += Convert.ToInt16(tempSample / oscillatiorsCount);
                        }
                        break;
                    case WaveForm.Noise:
                        for (int i = 0; i < SAMPLE_RATE; i++)
                        {
                            wave[i] += Convert.ToInt16(random.Next(-short.MaxValue, short.MaxValue) / oscillatiorsCount);
                        }
                        break;
                }
                
            }
            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                short blockAlign = BITS_PER_SAMPLE / 8;
                int subChunkTwoSize = SAMPLE_RATE * blockAlign;
                binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                binaryWriter.Write(36 + subChunkTwoSize);
                binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                binaryWriter.Write(16);
                binaryWriter.Write((short)1);
                binaryWriter.Write((short)1);
                binaryWriter.Write(SAMPLE_RATE);
                binaryWriter.Write(SAMPLE_RATE * blockAlign);
                binaryWriter.Write(blockAlign);
                binaryWriter.Write(BITS_PER_SAMPLE);
                binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                binaryWriter.Write(subChunkTwoSize);
                binaryWriter.Write(binaryWave);
                memoryStream.Position = 0;
                sound = new SoundPlayer(memoryStream);
                PlaySound(sound, true);
                //MessageBox.Show("Play");
            }
        }

        private void PlaySound(SoundPlayer sound, bool play)
        {   
            if (play && !isPlaying)
            { 
                    isPlaying = true;
                    sound.Load();
                    sound.PlayLooping();
                    started++;
            }
            else if (!play && !isPlaying)
            {
                sound.Stop();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            isPlaying = false;
            PlaySound(sound, false);
        }
    }

    public enum WaveForm 
    {
        Sine, Square, Saw, Triangle, Noise
    }
}
