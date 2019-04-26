using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace InteractiveVideo
{
    static class Program
    {
        public static Form1 form;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form1();

            Application.Run(form);
        }

        static void FormClosing(object o, EventArgs e)
        {
        }
    }

    class SelectionMenu
    {
        public Label label;
        public Button[] buttons;

        public SelectionMenu(string[] selections)
        {
            int x = 0;
            int y = 0;

            label = new Label();
            label.Text = "Что делать?";
            label.Location = new Point(x, y);
            y += label.Size.Height;

            buttons = new Button[selections.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                Button btn = new Button();
                btn.Text = selections[i];
                btn.BackColor = Color.Wheat;
                btn.Size = new Size(100, 80);
                btn.Location = new Point(x, y);
                y += btn.Size.Height;
                buttons[i] = btn;
            }
        }

        public void AddOnForm()
        {
            Invoker.Invoke(Program.form, (frm) =>
            {
                frm.Controls.Add(label);
            });

            for (int x = 0; x < buttons.Length; x++)
            {
                Invoker.Invoke(Program.form, (frm) =>
                {
                    frm.Controls.Add(buttons[x]);
                });
            }
        }
        public void RemoveFromForm()
        {
            Invoker.Invoke(Program.form, (frm) =>
            {
                frm.Controls.Remove(label);
            });

            for (int x = 0; x < buttons.Length; x++)
            {
                Invoker.Invoke(Program.form, (frm) =>
                {
                    frm.Controls.Remove(buttons[x]);
                });
            }
        }
    }

    static class VideoPlayer
    {
        public static void Play(string path)
        {
            Program.form.videoPlayer.URL = path;
            Program.form.videoPlayer.Ctlcontrols.play();
        }
    }

    static class Scene
    {
        public static string currentScenePath;

        static SelectionMenu menu = null;

        public static void Start(string path)
        {
            currentScenePath = path;

            if (menu != null) menu.RemoveFromForm();

            Tuple<string[], string[]> data = null;

            try { VideoPlayer.Play(path + "video.mp4"); } catch { }

            try { data = GetData(GetText(path + "scrypt.txt")); } catch { return; }

            if (data.Item1.Length == 0) return;

            menu = new SelectionMenu(data.Item1);

            for (int x = 0; x < data.Item1.Length; x++)
            {
                int i = x;

                menu.buttons[i].Click += (o, e) =>
                {
                    Start(data.Item2[i]);
                };
            }

            menu.AddOnForm();
        }

        static Tuple<string[], string[]> GetData(string text)
        {
            int pos1 = 0;
            int pos2;

            List<string> selections = new List<string>();
            List<string> paths = new List<string>();

            while (true)
            {
                pos1 = text.IndexOf('{', pos1);

                if (pos1 == -1) break;

                pos1++;

                pos2 = text.IndexOf('}', pos1);

                if (pos2 == -1) throw new Exception("Файл поврежден");

                selections.Add(text.Substring(pos1, pos2 - pos1));

                pos1 = text.IndexOf('{', pos2);

                if (pos1 == -1) throw new Exception("Файл поврежден");

                pos1++;

                pos2 = text.IndexOf('}', pos1);

                if (pos2 == -1) throw new Exception("Файл поврежден");

                paths.Add(text.Substring(pos1, pos2 - pos1));

                pos1 = pos2;
            }

            return new Tuple<string[], string[]>(selections.ToArray(), paths.ToArray());
        }

        public static string GetText(string path)
        {
            StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding(1251));
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        public static void ReWrite(string path, string text)
        {
            StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate), System.Text.Encoding.GetEncoding(1251));
            sw.Write(text);
            sw.Close();
        }

        static void End()
        {
            MessageBox.Show("End");
        }
    }

    static class Invoker
    {
        public static void Invoke(Control control, Action<Control> Func)
        {
            if (control.InvokeRequired)
                control.Invoke(Func, control);
            else
                Func(control);
        }
    }
}