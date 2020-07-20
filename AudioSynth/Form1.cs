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
        int octave = 4;
        Notes note = new Notes();

        public Form1()
        {
            InitializeComponent();
            //MessageBox.Show(note.frequency[octave, 0].ToString());
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
                case Keys.Q:
                    frequency = note.frequency[0, octave];
                    key = Keys.Q;
                    break;
                case Keys.W:
                    frequency = note.frequency[1, octave];
                    key = Keys.W;
                    break;
                case Keys.E:
                    frequency = note.frequency[2, octave];
                    key = Keys.E;
                    break;
                case Keys.R:
                    frequency = note.frequency[3, octave];
                    key = Keys.R;
                    break;
                case Keys.T:
                    frequency = note.frequency[4, octave];
                    key = Keys.T;
                    break;
                case Keys.Y:
                    frequency = note.frequency[5, octave];
                    key = Keys.Y;
                    break;
                case Keys.F:
                    frequency = note.frequency[6, octave];
                    key = Keys.F;
                    break;
                case Keys.G:
                    frequency = note.frequency[7, octave];
                    key = Keys.I;
                    break;
                case Keys.H:
                    frequency = note.frequency[8, octave];
                    key = Keys.H;
                    break;
                case Keys.J:
                    frequency = note.frequency[9, octave];
                    key = Keys.J;
                    break;
                case Keys.K:
                    frequency = note.frequency[10, octave];
                    key = Keys.K;
                    break;
                case Keys.L:
                    frequency = note.frequency[11, octave];
                    key = Keys.L;
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
                    //started++;
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            octave = Convert.ToInt32(comboBox1.SelectedIndex);
        }
    }

    public enum WaveForm 
    {
        Sine, Square, Saw, Triangle, Noise
    }
}
