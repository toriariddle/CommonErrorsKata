using CommonErrorsKata.Shared;
using System.IO;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonErrorsKata
{
    public partial class CommonErrorsForm : Form
    {
        private readonly AnswerQueue<TrueFalseAnswer> _answerQueue;
        private readonly string[] _files;
        private readonly SynchronizationContext _synchronizationContext;
        private int _time = 100;
        private string _currentBaseName;

        public CommonErrorsForm()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            _files = Directory.GetFiles(Environment.CurrentDirectory +  @"..\..\..\ErrorPics");
            var possibleAnswers = new[] { "Missing File", "null instance", "divide by zero" };
            lstAnswers.DataSource = possibleAnswers;
            _answerQueue = new AnswerQueue<TrueFalseAnswer>(15);
            Next();
            lstAnswers.Click += LstAnswers_Click;
            StartTimer();
        }
        private async void StartTimer()
        {
            await Task.Run(() =>
            {
                for (_time = 100; _time > 0; _time--)
                {
                    UpdateProgress(_time);
                    Thread.Sleep(50);
                }
                Message("Need to be quicker on your feet next time!  Try again...");
            });
        }

        private void LstAnswers_Click(object sender, EventArgs e)
        {
            _time = 100;
            var tokens = _currentBaseName.Split(' ');
            //TODO:  Figure out what is a valid answer.
            _answerQueue.Enqueue(new TrueFalseAnswer(true));
            Next();
        }

        private void Next()
        {
            if (_answerQueue.Count == 15 && _answerQueue.Grade >= 98)
            {
                MessageBox.Show("Congratulations you've defeated me!");
                Application.Exit();
                return;
            }
            label1.Text = _answerQueue.Grade + "%";
            var file = _files.GetRandom();
            _currentBaseName= Path.GetFileName(file);
            pbImage.ImageLocation = file;
        }

        public void UpdateProgress(int value)
        {
            _synchronizationContext.Post(x => {
                progress.Value = value;
            }, value);
        }
        public void Message(string value)
        {
            if (value != null) _synchronizationContext.Post(x => { MessageBox.Show(value); }, value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
