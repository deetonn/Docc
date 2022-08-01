using Docc.Client.Connection;
using Docc.Common;
using Docc.Common.Storage;

namespace Docc.Client
{
    public partial class LoginForm : Form
    {
        private void ShowMsg(string message)
        {
            if (CheckForIllegalCrossThreadCalls)
                CheckForIllegalCrossThreadCalls = false;

            StatusTextBox.Text = message;

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                StatusTextBox.Text = string.Empty;
            });
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var (saveLoginResult, message) = Global.TryLoginFromSaveFile("credentials.key");

            if (!saveLoginResult)
            {
                ShowMsg($"failed to login from saved credentials. {message}");
                return;
            }

            /*
             * If we did connect, the login page is pointless so show the main form.
             */

            Hide();

            MainChatForm mainForm = new();
            mainForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Global.Client?.Connection.Connected == true)
            {
                Global.Client?.MakeRequest(
                    new RequestBuilder()
                        .WithResult(RequestResult.OK)
                        .WithLocation("/api/v1/disconnect")
                        .Build()
                );
            }

            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // attempt to login

            if (string.IsNullOrEmpty(UserTextBox.Text))
            {
                ShowMsg("must enter a username.");
                return;
            }
            if (UserTextBox.Text.Contains(" "))
            {
                ShowMsg("invalid username");
                return;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                ShowMsg("You've entered no password.");
                return;
            }

            ShowMsg("logging in...");

            var (loggedIn, message) = Global.TryLogin(UserTextBox.Text, PasswordTextBox.Text);

            if (!loggedIn)
            {
                ShowMsg($"login failed. {message}");
                return;
            }

            // save credentials after successful login if wanted

            if (RememberMe.Checked)
            {
                Global.SaveLogin(new LocalLoginCredentials
                {
                    Username = UserTextBox.Text,
                    HashedPassword = StorageUtil.Sha256Hash(PasswordTextBox.Text)
                }, "credentials.key");
            }

            // show main window and save auth token

            Hide();

            MainChatForm MainChatForm = new MainChatForm();
            MainChatForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}