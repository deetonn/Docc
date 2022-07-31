using Docc.Client.Connection;
using Docc.Common;

namespace Docc.Client
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Global.Connection?.Connection.Connected == true)
            {
                Global.Connection.MakeRequest(
                    new RequestBuilder()
                        .WithResult(RequestResult.OK)
                        .WithLocation("/api/v1/disconnect")
                        .Build()
                );
            }

            Environment.Exit(0);
        }
    }
}