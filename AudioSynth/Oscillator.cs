using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AudioSynth
{
    public class Oscillator : GroupBox
    {
        public Oscillator()
        {
            this.Controls.Add(new Button()
            {
                Name = "Sine",
                Location = new Point(10, 15),
                Text = "Sine",
                BackColor = Color.Blue,
                ForeColor = Color.White
            }) ;
            this.Controls.Add(new Button()
            {
                Name = "Square",
                Location = new Point(65, 15),
                Text = "Square"
            });
            this.Controls.Add(new Button()
            {
                Name = "Saw",
                Location = new Point(120, 15),
                Text = "Saw"
            });
            this.Controls.Add(new Button()
            {
                Name = "Triangle",
                Location = new Point(10, 50),
                Text = "Triangle"
            });
            this.Controls.Add(new Button()
            {
                Name = "Noise",
                Location = new Point(65, 50),
                Text = "Noise"
            });

            foreach (Control cont in this.Controls)
            {
                cont.Size = new Size(50, 30);
                cont.Font = new Font("Microsoft Sans Serif", 6.75f);
                cont.Click += WaveButton_Click;
            }

            this.Controls.Add(new CheckBox()
            {
                Name = "OscillatorOn",
                Location = new Point(210, 10),
                Size = new Size(40, 30),
                Text = "On",
                Checked = true
            });
        }

        public WaveForm WaveForm { get; private set; }
        public bool On => ((CheckBox)this.Controls["OscillatorOn"]).Checked;
        private void WaveButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            this.WaveForm = (WaveForm)Enum.Parse(typeof(WaveForm), button.Text);
            //MessageBox.Show($"The button {this.WaveForm}");
            foreach(Button btn in this.Controls.OfType<Button>())
            {
                btn.UseVisualStyleBackColor = true;
                btn.ForeColor = Color.Black;
            }
            button.BackColor = Color.Blue;
            button.ForeColor = Color.White;
        }
    }
}
