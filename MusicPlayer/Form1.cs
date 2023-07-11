using System;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Runtime.CompilerServices;

namespace MusicPlayer
{
    public partial class Form1 : Form
    {
        WaveOut waveOut = null;
        AudioFileReader audioFile = null;
        string fileName = null;
        private TimeSpan savedTrackBarValue;
        private string folderPath;

        public Form1()
        {
            InitializeComponent();

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;

            trackBar1.Value = 0;

            trackBar1.Scroll += new EventHandler(this.trackBar1_Scroll);

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 100;

            trackBar2.Value = 50;

            
            

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
            }
            base.OnFormClosing(e);
        }

        // Opentool for select file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            waveOut?.Pause();
            waveOut?.Stop();
            waveOut?.Dispose();
            audioFile?.Dispose();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files (*.wav)|*.wav|All Files (*.*)|*.*";

            

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                string filePath = openFileDialog.FileName;
                audioFile = new AudioFileReader(filePath);

                // Initializing a new WaveOut object and starting playback
                waveOut = new WaveOut();
                waveOut.PlaybackStopped += WaveOut_PlaybackStopped; // Handler for the end of playback event
                waveOut.Init(audioFile);
                waveOut.Play();

                // Setting the timer value to 0 and starting
                timer1 = new Timer();
                timer1.Interval = 1000;
                timer1.Tick += timer1_Tick;
                timer1.Start();

                // Track length output
                TimeSpan duration = audioFile.TotalTime;
                label3.Text = duration.ToString(@"mm\:ss");

                trackBar1.Value = 0;
            }

        }

        // bt Play
        private void button1_Click(object sender, EventArgs e)
        {           
            if (waveOut != null && (waveOut.PlaybackState == PlaybackState.Playing || waveOut.PlaybackState == PlaybackState.Paused))
            {
                savedTrackBarValue = audioFile.CurrentTime;
                waveOut.Stop();
                waveOut.Dispose(); // Clearing WaveOut before creating a new one
                waveOut = null;
                timer1.Stop();
            }

            if (audioFile == null)
            {
                MessageBox.Show("Select audiofile.");
                return;
            }

            audioFile.CurrentTime = savedTrackBarValue;

            if (waveOut == null)
            {
                waveOut = new WaveOut();
                waveOut.Init(audioFile);
            }

            waveOut.Play();
            timer1.Start();
        }

        // bt Stop
        private void button2_Click(object sender, EventArgs e)
        {           
            if (waveOut != null && (waveOut.PlaybackState == PlaybackState.Playing || waveOut.PlaybackState == PlaybackState.Paused))
            {
                savedTrackBarValue = audioFile.CurrentTime;
                waveOut.Stop();
                timer1.Stop();
                
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int currentValue = trackBar1.Value;
            if (waveOut != null && audioFile != null)
            {
                if (currentValue == trackBar1.Maximum)
                {
                    currentValue = trackBar1.Maximum - 1;
                    button2_Click(sender, e);
                }

                double newPosition = audioFile.TotalTime.TotalMilliseconds * (double)(currentValue / (double)trackBar1.Maximum);
                audioFile.CurrentTime = TimeSpan.FromMilliseconds(newPosition);
            }
            this.Text = "Current value: " + currentValue.ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        // Volume trackBar
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            adjustVolume(trackBar2.Value);
        }

        private void adjustVolume(int volume)
        {
            if (audioFile != null)
            {
                audioFile.Volume = (float)volume / 100.0f;
            }
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Zeroing the track position and stopping playback after the end of the track
            audioFile.Position = 0;
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (audioFile != null)
            {
                label2.Text = audioFile.CurrentTime.ToString(@"hh\:mm\:ss");
                label3.Text = audioFile.TotalTime.ToString(@"hh\:mm\:ss");

                if (audioFile.CurrentTime >= audioFile.TotalTime)
                {
                    timer1.Stop();
                    waveOut.Stop();
                    audioFile.Dispose();
                    
                }
            }
        }
    }
}