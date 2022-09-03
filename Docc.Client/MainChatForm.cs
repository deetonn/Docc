using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Docc.Client.Connection;
using Docc.Common;

namespace Docc.Client;

public partial class MainChatForm : Form
{
    private LogsForm logsForm;

    public MainChatForm()
    {
        InitializeComponent();

        this.MouseDown += MainChatForm_MouseDown;
        this.MouseMove += MainChatForm_MouseMove;
        this.MouseUp += MainChatForm_MouseUp;

        CheckForIllegalCrossThreadCalls = false;

        if (Global.Client is null)
            return;

        // Just handle it, many endpoints that return nothing send this.

        Global.AddCallback("/", (req) =>
        {
        });
        Global.AddCallback("/error/handle", (req) =>
        {
            if (!req.Arguments.ContainsKey("name"))
                return;
            if (!req.Arguments.ContainsKey("msg"))
                return;
            if (!req.Arguments.ContainsKey("color"))
                return;

            var color = Color.FromName(req.Arguments["color"]);
            var msg = req.Arguments["msg"];
            var name = "[!] " + req.Arguments["name"];

            InsertChatMessageLocal(msg, color, name);
        });
        Global.AddCallback("/chat/api/receive", (req) =>
        {
            if (!req.Arguments.ContainsKey("name"))
                return;
            if (!req.Arguments.ContainsKey("msg"))
                return;
            if (!req.Arguments.ContainsKey("color"))
                return;

            var color = Color.FromName(req.Arguments["color"]);
            var msg = req.Arguments["msg"];
            var name = req.Arguments["name"];

            InsertChatMessageLocal(msg, color, name);
        });

        Global.Client.Connection.OnRequest = (req) =>
        {
            Global.Log($"received request to '{req.Location}. Body: {req}'");

            if (Global.Handlers.ContainsKey(req.Location))
            {
                Global.Handlers[req.Location].Invoke(req);
                return;
            }

            ShowMsg($"unhandled message: {req.Location}");
        };
    }

    public void InsertChatMessageLocal(string message, Color color, string name)
    {
        ListViewItem item = new($"{name}: {message}")
        {
            ForeColor = color
        };
        ChatBox.Items.Add(item);
    }

    public void ShowMsg(string message)
    {
        if (CheckForIllegalCrossThreadCalls)
            CheckForIllegalCrossThreadCalls = false;

        ErrorBox.Text = message;

        Task.Run(async () =>
        {
            Global.Log($"mainForm: {message}");
            await Task.Delay(5000);
            ErrorBox.Text = string.Empty;
        });
    }

    private void MainChatForm_Load(object sender, EventArgs e)
    {
        if (Global.Client
            is null)
        {
            SessionIdLabel.Text = "Session: Not connected";
            return;
        }


        if (!Global.Client.Connection.Connected)
        {
            SessionIdLabel.Text = "Session: Not connected";
            return;
        }

        SessionIdLabel.Text = $"SessionID: {Global.Client.Connection.SessionId}";

        WindowState = FormWindowState.Minimized;
        Show();
        WindowState = FormWindowState.Normal;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        // fetch name, color
        // then send request to '/chat/api/sendMessage'

        if (ChatBox.Items.Count > 15)
        {
            ChatBox.Clear();
        }

        if (string.IsNullOrEmpty(InputTextBox.Text))
        {
            ShowMsg("cannot send nothing lol...");
            return;
        }
        if ((Global.Client is null) || !Global.Client.Connection.Connected)
        {
            ShowMsg("not connected.");
            return;
        }

        var args = new Dictionary<string, string>();

        if (CustomNameEnabled.Checked)
        {
            args.Add("name", CustomNameTextBox.Text);
        }
        if (CustomColorEnabled.Checked)
        {
            args.Add("col", CustomColorTextBox.Text);
        }

        args.Add("msg", InputTextBox.Text);

        Global.Client.MakeSpurnRequest(
            new RequestBuilder()
                .WithLocation("/chat/api/sendMessage")
                .WithResult(RequestResult.OK)
                .WithArguments(args)
                .Build()
        );

        var color = CustomColorEnabled.Checked
            ? Color.FromName(CustomColorTextBox.Text)
            : Color.White;

        InsertChatMessageLocal(InputTextBox.Text, color, "You");

        InputTextBox.Clear();
    }

    private void button2_Click(object sender, EventArgs e)
    {
        if (Global.Client?.Connection.Connected == true)
        {
            Global.Client?.MakeSpurnRequest(
                new RequestBuilder()
                    .WithResult(RequestResult.OK)
                    .WithLocation("/api/v1/disconnect")
                    .Build()
            );
        }

        // anything else will cause it to stay alive
        // possibly due to the other window still existing,
        // but not open.
        Process.GetCurrentProcess().Kill();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Minimized;
    }

    // for moving the form without borders


    private bool dragging = false;
    private Point dragCursorPoint;
    private Point dragFormPoint;

    private void MainChatForm_MouseDown(object? sender, MouseEventArgs e)
    {
        dragging = true;
        dragCursorPoint = Cursor.Position;
        dragFormPoint = this.Location;
    }

    private void MainChatForm_MouseMove(object? sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
            this.Location = Point.Add(dragFormPoint, new Size(dif));
        }
    }

    private void MainChatForm_MouseUp(object? sender, MouseEventArgs e)
    {
        dragging = false;
    }

    private void ViewLogsButton_Click(object sender, EventArgs e)
    {
        logsForm = new LogsForm();
        logsForm.Show();
    }
}
