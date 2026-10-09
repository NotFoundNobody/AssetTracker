
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI;

namespace UI.Forms
{
    public partial class frmLogin : Form
    {
        private readonly Color _backgroundColor = Color.FromArgb(246, 248, 252);
        private readonly Color _primaryColor = Color.FromArgb(53, 96, 230);
        private readonly Color _textColor = Color.FromArgb(37, 48, 68);
        private readonly Color _mutedColor = Color.FromArgb(120, 132, 151);

        private Panel _contentPanel = null!;
        private ShadowCardPanel _cardHost = null!;
        private RoundedSurfacePanel _loginCard = null!;

        private InputFieldPanel _usernameField = null!;
        private InputFieldPanel _passwordField = null!;

        private TextBox _usernameTextBox => _usernameField.InputTextBox;
        private TextBox _passwordTextBox => _passwordField.InputTextBox;

        private ModernButton _loginButton = null!;
        private ProgressBar _loginProgressBar = null!;
        private Label _messageLabel = null!;

        private bool _isLoggingIn;
        private bool _isDragging;
        private Point _dragStart;
        private Point _formStart;

        public frmLogin()
        {
            InitializeComponent();

            // Remove the controls from the previous design.
            Controls.Clear();

            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "AssetTracker - Sign In";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(900, 600);
            ClientSize = new Size(1100, 700);
            BackColor = _backgroundColor;
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;
            KeyPreview = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = _backgroundColor
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel brandPanel = CreateBrandPanel();

            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _backgroundColor
            };

            BuildContentPanel();

            root.Controls.Add(brandPanel, 0, 0);
            root.Controls.Add(_contentPanel, 1, 0);

            Controls.Add(root);

            AcceptButton = _loginButton;

            _contentPanel.Resize += (_, _) => CenterLoginCard();

            CenterLoginCard();
            KeyDown += FrmLogin_KeyDown;
        }

        private Panel CreateBrandPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 48, 91)
            };

            panel.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var brush = new LinearGradientBrush(
                    panel.ClientRectangle,
                    Color.FromArgb(23, 36, 73),
                    Color.FromArgb(57, 91, 170),
                    LinearGradientMode.ForwardDiagonal);

                e.Graphics.FillRectangle(brush, panel.ClientRectangle);

                using var circleBrush = new SolidBrush(
                    Color.FromArgb(22, 100, 255, 255));

                e.Graphics.FillEllipse(
                    circleBrush,
                    panel.Width - 130,
                    panel.Height - 240,
                    220,
                    220);
            };

            var logo = new Panel
            {
                Location = new Point(42, 68),
                Size = new Size(70, 70),
                BackColor = Color.Transparent
            };

            logo.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var circleBrush = new SolidBrush(
                    Color.FromArgb(76, 111, 235));

                e.Graphics.FillEllipse(
                    circleBrush, 0, 0, logo.Width - 1, logo.Height - 1);

                using var pen = new Pen(Color.White, 2.8F);

                e.Graphics.DrawRectangle(pen, 23, 18, 24, 32);
                e.Graphics.DrawLine(pen, 28, 25, 42, 25);
                e.Graphics.DrawLine(pen, 28, 32, 42, 32);
                e.Graphics.DrawLine(pen, 28, 39, 38, 39);
            };

            var brandTitle = new Label
            {
                Text = "AssetTracker",
                AutoSize = true,
                Location = new Point(42, 158),
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            var headline = new Label
            {
                Text = "Manage your assets.\nStay in control.",
                AutoSize = true,
                Location = new Point(44, 226),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 243, 255),
                BackColor = Color.Transparent
            };

            var description = new Label
            {
                Text = "Manage people, assets and organizational\nresources in one secure place.",
                AutoSize = true,
                Location = new Point(46, 320),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(197, 208, 238),
                BackColor = Color.Transparent
            };

            var featureOne = CreateFeatureLabel(
                "✓", "People and access management", 397);

            var featureTwo = CreateFeatureLabel(
                "✓", "Centralized asset inventory", 439);

            var featureThree = CreateFeatureLabel(
                "✓", "A clear operational overview", 481);

            var footer = new Label
            {
                Text = "SECURE ACCESS  •  ASSET MANAGEMENT",
                AutoSize = true,
                Location = new Point(44, 0),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(188, 202, 234),
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[]
            {
                logo,
                brandTitle,
                headline,
                description,
                featureOne,
                featureTwo,
                featureThree,
                footer
            });

            panel.Resize += (_, _) =>
            {
                footer.Location = new Point(
                    44, panel.ClientSize.Height - 42);

                panel.Invalidate();
            };

            EnableDragging(panel);

            return panel;
        }

        private Label CreateFeatureLabel(
            string symbol,
            string text,
            int top)
        {
            return new Label
            {
                Text = $"{symbol}   {text}",
                AutoSize = true,
                Location = new Point(46, top),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(225, 233, 250),
                BackColor = Color.Transparent
            };
        }

        private void BuildContentPanel()
        {
            var windowBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = _backgroundColor
            };

            var minimizeButton = CreateWindowButton("−");
            var closeButton = CreateWindowButton("×");

            minimizeButton.Anchor =
                AnchorStyles.Top | AnchorStyles.Right;

            closeButton.Anchor =
                AnchorStyles.Top | AnchorStyles.Right;

            minimizeButton.Location = new Point(
                windowBar.ClientSize.Width - 88, 8);

            closeButton.Location = new Point(
                windowBar.ClientSize.Width - 46, 8);

            minimizeButton.Click += (_, _) =>
                WindowState = FormWindowState.Minimized;

            closeButton.Click += (_, _) => Close();

            windowBar.Controls.Add(minimizeButton);
            windowBar.Controls.Add(closeButton);

            windowBar.Resize += (_, _) =>
            {
                minimizeButton.Location = new Point(
                    windowBar.ClientSize.Width - 88, 8);

                closeButton.Location = new Point(
                    windowBar.ClientSize.Width - 46, 8);
            };

            windowBar.MouseDown += Window_MouseDown;
            windowBar.MouseMove += Window_MouseMove;
            windowBar.MouseUp += Window_MouseUp;

            _cardHost = new ShadowCardPanel
            {
                Size = new Size(460, 550),
                BackColor = Color.Transparent
            };

            _loginCard = new RoundedSurfacePanel
            {
                Location = new Point(17, 17),
                Size = new Size(426, 510),
                BackColor = Color.White
            };

            BuildLoginCard();

            _cardHost.Controls.Add(_loginCard);

            _contentPanel.Controls.Add(_cardHost);
            _contentPanel.Controls.Add(windowBar);

            windowBar.BringToFront();
        }

        private void BuildLoginCard()
        {
            var welcomeLabel = new Label
            {
                Text = "Welcome back",
                AutoSize = true,
                Location = new Point(32, 32),
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = _textColor
            };

            var subtitleLabel = new Label
            {
                Text = "Sign in to continue to your account.",
                AutoSize = true,
                Location = new Point(34, 78),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = _mutedColor
            };

            var separator = new Panel
            {
                Location = new Point(34, 113),
                Size = new Size(358, 1),
                BackColor = Color.FromArgb(237, 240, 246)
            };

            var usernameLabel = CreateFieldLabel("Username", 134);

            _usernameField = new InputFieldPanel(
                "UsernameTextBox",
                "Enter your username",
                false,
                _textColor,
                _primaryColor)
            {
                Location = new Point(32, 158),
                Size = new Size(362, 46)
            };

            var passwordLabel = CreateFieldLabel("Password", 222);

            _passwordField = new InputFieldPanel(
                "PasswordTextBox",
                "Enter your password",
                true,
                _textColor,
                _primaryColor)
            {
                Location = new Point(32, 246),
                Size = new Size(362, 46)
            };

            _messageLabel = new Label
            {
                Text = "",
                Location = new Point(34, 300),
                Size = new Size(358, 31),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(198, 55, 66),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _loginButton = new ModernButton
            {
                Name = "LoginButton",
                Text = "Sign In",
                Location = new Point(32, 341),
                Size = new Size(362, 48),
                BackColor = _primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabIndex = 2
            };

            _loginButton.Click += btnLogin_Click;

            _loginProgressBar = new ProgressBar
            {
                Name = "LoginProgressBar",
                Location = new Point(32, 399),
                Size = new Size(362, 6),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };

            var footerLabel = new Label
            {
                Text = "SECURE ACCESS  •  AUTHORIZED USERS ONLY",
                Location = new Point(32, 428),
                Size = new Size(362, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = _mutedColor,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            _loginCard.Controls.AddRange(new Control[]
            {
                welcomeLabel,
                subtitleLabel,
                separator,
                usernameLabel,
                _usernameField,
                passwordLabel,
                _passwordField,
                _messageLabel,
                _loginButton,
                _loginProgressBar,
                footerLabel
            });

            _usernameTextBox.TabIndex = 0;
            _passwordTextBox.TabIndex = 1;

            _usernameTextBox.KeyDown += Input_KeyDown;
            _passwordTextBox.KeyDown += Input_KeyDown;
        }

        private Label CreateFieldLabel(string text, int top)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(34, top),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = _textColor
            };
        }

        private Button CreateWindowButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(34, 30),
                BackColor = _backgroundColor,
                ForeColor = _textColor,
                Font = new Font("Segoe UI", 13F),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(230, 234, 243);

            return button;
        }

        private void CenterLoginCard()
        {
            if (_contentPanel == null || _cardHost == null)
                return;

            int x = (_contentPanel.ClientSize.Width - _cardHost.Width) / 2;
            int y = (_contentPanel.ClientSize.Height - _cardHost.Height) / 2 + 15;

            _cardHost.Location = new Point(
                Math.Max(5, x),
                Math.Max(52, y));
        }

        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            if (_isLoggingIn || IsDisposed || Disposing)
                return;

            string username = _usernameTextBox.Text.Trim();
            string password = _passwordTextBox.Text;

            _messageLabel.Text = "";

            if (string.IsNullOrWhiteSpace(username))
            {
                _messageLabel.Text = "Please enter your username.";
                _usernameField.FocusInput();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _messageLabel.Text = "Please enter your password.";
                _passwordField.FocusInput();
                return;
            }

            bool focusPasswordAfterRequest = false;

            SetLoading(true);

            try
            {
                // Allow WinForms to render the loading indicator.
                await Task.Delay(150);

                var authenticationService =
                    Program.ServiceProvider
                        .GetRequiredService<IAuthenticationService>();

                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var result = await authenticationService.Login(loginRequest);

                // Keep the rest of your existing success/error handling here.

               

                if (IsDisposed || Disposing)
                    return;

                if (result == null)
                {
                    _messageLabel.Text =
                        "Invalid username or password.";

                    _passwordTextBox.Clear();
                    focusPasswordAfterRequest = true;
                    return;
                }

                Program.UserData = result;

                Hide();

                using (var mainForm = new frmMain())
                {
                    mainForm.ShowDialog();
                }
                if (Program.UserData == null && !IsDisposed && !Disposing)
                {
                    _usernameTextBox.Clear();
                    _passwordTextBox.Clear();
                    _messageLabel.Text = "";

                    Show();
                    WindowState = FormWindowState.Normal;
                    Activate();
                    _usernameTextBox.Focus();

                    return;
                }

                Close();

          
              
            }
            catch (Exception)
            {
                if (!IsDisposed && !Disposing)
                {
                    _messageLabel.Text =
                        "Unable to sign in. Please try again.";

                    focusPasswordAfterRequest = true;
                }
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    SetLoading(false);

                    if (focusPasswordAfterRequest)
                        _passwordField.FocusInput();
                }
            }
        }

        private void SetLoading(bool isLoading)
        {
            _isLoggingIn = isLoading;

            _loginButton.Enabled = !isLoading;
            _usernameTextBox.Enabled = !isLoading;
            _passwordTextBox.Enabled = !isLoading;

            if (_passwordField.ToggleButton != null)
                _passwordField.ToggleButton.Enabled = !isLoading;

            _loginButton.Text = isLoading
                ? "Signing in..."
                : "Sign In";

            _loginProgressBar.Visible = isLoading;

            Cursor = isLoading
                ? Cursors.WaitCursor
                : Cursors.Default;
        }

        private void Input_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            if (sender == _usernameTextBox)
            {
                _passwordField.FocusInput();
            }
            else if (sender == _passwordTextBox)
            {
                _ = LoginAsync();
            }
        }

        private void FrmLogin_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void EnableDragging(Control control)
        {
            control.MouseDown += Window_MouseDown;
            control.MouseMove += Window_MouseMove;
            control.MouseUp += Window_MouseUp;

            foreach (Control child in control.Controls)
                EnableDragging(child);
        }

        private void Window_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _isDragging = true;
            _dragStart = Cursor.Position;
            _formStart = Location;

            if (sender is Control control)
                control.Capture = true;
        }

        private void Window_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point current = Cursor.Position;

            Location = new Point(
                _formStart.X + current.X - _dragStart.X,
                _formStart.Y + current.Y - _dragStart.Y);
        }

        private void Window_MouseUp(object? sender, MouseEventArgs e)
        {
            _isDragging = false;

            if (sender is Control control)
                control.Capture = false;
        }

        private static GraphicsPath CreateRoundedPath(
            Rectangle bounds,
            int radius)
        {
            var path = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            radius = Math.Max(
                1,
                Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));

            int diameter = radius * 2;

            path.AddArc(
                bounds.X, bounds.Y, diameter, diameter, 180, 90);

            path.AddArc(
                bounds.Right - diameter, bounds.Y,
                diameter, diameter, 270, 90);

            path.AddArc(
                bounds.Right - diameter, bounds.Bottom - diameter,
                diameter, diameter, 0, 90);

            path.AddArc(
                bounds.X, bounds.Bottom - diameter,
                diameter, diameter, 90, 90);

            path.CloseFigure();

            return path;
        }

        private sealed class InputFieldPanel : Panel
        {
            private readonly bool _isPassword;
            private readonly Color _textColor;
            private readonly Color _accentColor;

            private bool _isFocused;

            public TextBox InputTextBox { get; }

            public Button? ToggleButton { get; private set; }

            public InputFieldPanel(
                string controlName,
                string placeholder,
                bool isPassword,
                Color textColor,
                Color accentColor)
            {
                _isPassword = isPassword;
                _textColor = textColor;
                _accentColor = accentColor;

                Name = controlName + "Container";
                BackColor = Color.White;
                TabStop = false;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor,
                    true);

                InputTextBox = new TextBox
                {
                    Name = controlName,
                    Location = new Point(44, 12),
                    Size = new Size(
                        isPassword ? 244 : 303, 23),
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = _textColor,
                    BackColor = Color.FromArgb(250, 251, 254),
                    PlaceholderText = placeholder,
                    UseSystemPasswordChar = isPassword,
                    TabStop = true
                };

                InputTextBox.Enter += (_, _) =>
                {
                    _isFocused = true;
                    Invalidate();
                };

                InputTextBox.Leave += (_, _) =>
                {
                    _isFocused = ContainsFocus;
                    Invalidate();
                };

                Controls.Add(InputTextBox);

                if (isPassword)
                {
                    ToggleButton = new Button
                    {
                        Name = "PasswordToggleButton",
                        Text = "Show",
                        Location = new Point(301, 7),
                        Size = new Size(51, 30),
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        ForeColor = _accentColor,
                        BackColor = Color.FromArgb(250, 251, 254),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        TabStop = false,
                        UseVisualStyleBackColor = false
                    };

                    ToggleButton.FlatAppearance.BorderSize = 0;
                    ToggleButton.FlatAppearance.MouseOverBackColor =
                        Color.FromArgb(239, 243, 255);

                    ToggleButton.Click += (_, _) =>
                    {
                        bool wasHidden =
                            InputTextBox.UseSystemPasswordChar;

                        InputTextBox.UseSystemPasswordChar = !wasHidden;
                        ToggleButton.Text = wasHidden ? "Hide" : "Show";

                        FocusInput();

                        InputTextBox.SelectionStart =
                            InputTextBox.Text.Length;
                    };

                    Controls.Add(ToggleButton);
                }

                Click += (_, _) => FocusInput();
            }

            public void FocusInput()
            {
                if (!InputTextBox.Enabled)
                    return;

                InputTextBox.Focus();
                _isFocused = true;
                Invalidate();
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Color parentColor = Parent?.BackColor ?? Color.White;
                e.Graphics.Clear(parentColor);

                var bounds = new Rectangle(
                    0, 0, Width - 1, Height - 1);

                using var path = CreateRoundedPath(bounds, 9);

                using var backgroundBrush = new SolidBrush(
                    Color.FromArgb(250, 251, 254));

                using var borderPen = new Pen(
                    _isFocused
                        ? _accentColor
                        : Color.FromArgb(221, 227, 238),
                    _isFocused ? 1.8F : 1F);

                e.Graphics.FillPath(backgroundBrush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Color iconColor = _isFocused
                    ? _accentColor
                    : Color.FromArgb(142, 153, 173);

                using var pen = new Pen(iconColor, 1.7F);

                if (_isPassword)
                {
                    // Lock icon.
                    e.Graphics.DrawArc(
                        pen, 15, 9, 13, 14, 180, 180);

                    using var lockBrush = new SolidBrush(
                        Color.FromArgb(250, 251, 254));

                    e.Graphics.FillRectangle(
                        lockBrush, 12, 17, 20, 15);

                    e.Graphics.DrawRectangle(
                        pen, 12, 17, 20, 15);

                    e.Graphics.DrawEllipse(
                        pen, 20, 21, 4, 4);

                    e.Graphics.DrawLine(
                        pen, 22, 24, 22, 28);
                }
                else
                {
                    // User icon.
                    e.Graphics.DrawEllipse(
                        pen, 17, 9, 8, 8);

                    e.Graphics.DrawArc(
                        pen, 11, 19, 20, 17, 180, 180);
                }
            }
        }

        private sealed class RoundedSurfacePanel : Panel
        {
            public RoundedSurfacePanel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor,
                    true);

                BackColor = Color.White;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Color parentColor = Parent?.BackColor ?? Color.White;
                e.Graphics.Clear(parentColor);

                var bounds = new Rectangle(
                    0, 0, Width - 1, Height - 1);

                using var path = CreateRoundedPath(bounds, 18);
                using var brush = new SolidBrush(Color.White);
                using var pen = new Pen(
                    Color.FromArgb(231, 235, 243));

                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private sealed class ShadowCardPanel : Panel
        {
            public ShadowCardPanel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor,
                    true);

                BackColor = Color.Transparent;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.Clear(Parent?.BackColor ?? Color.White);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Layer translucent rounded shapes to simulate a soft shadow.
                for (int i = 12; i >= 1; i--)
                {
                    int alpha = 14 - i;

                    var bounds = new Rectangle(
                        17 - i,
                        17 - i,
                        426 + (i * 2),
                        510 + (i * 2));

                    using var path = CreateRoundedPath(
                        bounds, 18 + i);

                    using var brush = new SolidBrush(
                        Color.FromArgb(
                            Math.Max(1, alpha),
                            27, 39, 64));

                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private sealed class ModernButton : Button
        {
            private bool _isHovered;
            private bool _isPressed;

            public ModernButton()
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                UseVisualStyleBackColor = false;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _isHovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _isHovered = false;
                _isPressed = false;
                Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                    _isPressed = true;

                Invalidate();
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _isPressed = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Color buttonColor;

                if (!Enabled)
                    buttonColor = Color.FromArgb(176, 187, 210);
                else if (_isPressed)
                    buttonColor = Color.FromArgb(35, 68, 185);
                else if (_isHovered)
                    buttonColor = Color.FromArgb(43, 81, 211);
                else
                    buttonColor = BackColor;

                var bounds = new Rectangle(
                    0, 0, Width - 1, Height - 1);

                using var path = CreateRoundedPath(bounds, 11);
                using var brush = new SolidBrush(buttonColor);

                e.Graphics.FillPath(brush, path);

                TextRenderer.DrawText(
                    e.Graphics,
                    Text,
                    Font,
                    ClientRectangle,
                    Enabled ? ForeColor : Color.FromArgb(235, 238, 245),
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);
            }
        }
    }
}