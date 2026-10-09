
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using UI.Views;

namespace UI.Forms
{
    public partial class frmMain : Form
    {
        private static readonly Color CanvasColor =
            Color.FromArgb(245, 247, 251);

        private static readonly Color SidebarColor =
            Color.FromArgb(24, 35, 57);

        private static readonly Color PrimaryColor =
            Color.FromArgb(65, 100, 232);

        private static readonly Color TextColor =
            Color.FromArgb(38, 49, 70);

        private static readonly Color MutedColor =
            Color.FromArgb(124, 135, 153);

        private Panel _contentHost = null!;
        private Label _pageTitle = null!;
        private Label _pageSubtitle = null!;

        private Panel _userAvatar = null!;
        private Label _userNameLabel = null!;
        private Label _userRoleLabel = null!;

        private readonly Dictionary<string, SidebarButton> _navButtons = new();

        private bool _isDragging;
        private Point _dragStart;
        private Point _formStart;

        public frmMain()
        {
            InitializeComponent();
            Controls.Clear();

            BuildShell();
            ShowDashboard();
        }

        private void BuildShell()
        {
            Text = "AssetTracker";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1050, 700);
            Size = new Size(1440, 900);
            BackColor = CanvasColor;
            Font = new Font("Segoe UI", 9.5F);
            DoubleBuffered = true;
            KeyPreview = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = CanvasColor
            };

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 252));

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            var sidebar = BuildSidebar();

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = CanvasColor
            };

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 82));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            mainLayout.Controls.Add(BuildTopBar(), 0, 0);

            _contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CanvasColor
            };

            mainLayout.Controls.Add(_contentHost, 0, 1);

            root.Controls.Add(sidebar, 0, 0);
            root.Controls.Add(mainLayout, 1, 0);

            Controls.Add(root);

            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Escape && WindowState != FormWindowState.Minimized)
                {
                    // Escape is intentionally not used for sign-out.
                }
            };
        }

        private Control BuildSidebar()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = SidebarColor,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 108));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 124));

            var brandPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SidebarColor
            };

            var logo = new Panel
            {
                Location = new Point(21, 28),
                Size = new Size(43, 43),
                BackColor = PrimaryColor
            };

            logo.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var path = CreateRoundedPath(
                    new Rectangle(0, 0, logo.Width - 1, logo.Height - 1),
                    10);

                using var brush = new LinearGradientBrush(
                    logo.ClientRectangle,
                    Color.FromArgb(89, 126, 255),
                    Color.FromArgb(49, 80, 203),
                    LinearGradientMode.ForwardDiagonal);

                e.Graphics.FillPath(brush, path);

                TextRenderer.DrawText(
                    e.Graphics,
                    "AT",
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    logo.ClientRectangle,
                    Color.White,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            };

            var brandTitle = new Label
            {
                Text = "AssetTracker",
                Location = new Point(76, 29),
                Size = new Size(155, 24),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = SidebarColor
            };

            var brandSubtitle = new Label
            {
                Text = "MANAGEMENT CONSOLE",
                Location = new Point(77, 54),
                Size = new Size(155, 17),
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 169, 195),
                BackColor = SidebarColor
            };

            brandPanel.Controls.AddRange(new Control[]
            {
                logo, brandTitle, brandSubtitle
            });

            var navPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = SidebarColor,
                Padding = new Padding(12),
                Margin = Padding.Empty
            };

            navPanel.Controls.Add(new Label
            {
                Text = "WORKSPACE",
                Size = new Size(215, 26),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(135, 150, 178),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = SidebarColor,
                Margin = new Padding(7, 0, 0, 8)
            });

            AddNavigation(navPanel, "dashboard", "Overview", "▦",
                ShowDashboard);

            AddNavigation(navPanel, "people", "People", "♙",
                ShowPeople);

            AddNavigation(navPanel, "assets", "Assets", "▣",
                () => ShowPlaceholder("Assets"));

            AddNavigation(navPanel, "categories", "Categories", "▤",
                () => ShowPlaceholder("Categories"));

            AddNavigation(navPanel, "reports", "Reports", "◷",
                () => ShowPlaceholder("Reports"));

            AddNavigation(navPanel, "settings", "Settings", "⚙",
                () => ShowPlaceholder("Settings"));

            var footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SidebarColor
            };

            var separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(54, 67, 91)
            };

            var statusLabel = new Label
            {
                Text = "●  DEMO WORKSPACE",
                Location = new Point(20, 10),
                Size = new Size(210, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(135, 205, 177),
                BackColor = SidebarColor
            };

            var statusSubtitle = new Label
            {
                Text = "Sample dashboard data",
                Location = new Point(20, 30),
                Size = new Size(210, 18),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(153, 165, 188),
                BackColor = SidebarColor
            };

            var logoutButton = new Button
            {
                Name = "LogoutButton",
                Text = "↪   Sign out",
                Location = new Point(15, 62),
                Size = new Size(218, 39),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 190, 195),
                BackColor = Color.FromArgb(37, 49, 74),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(75, 43, 58);

            logoutButton.Click += (_, _) => Logout();

            footerPanel.Controls.AddRange(new Control[]
            {
                separator,
                statusLabel,
                statusSubtitle,
                logoutButton
            });

            layout.Controls.Add(brandPanel, 0, 0);
            layout.Controls.Add(navPanel, 0, 1);
            layout.Controls.Add(footerPanel, 0, 2);

            return layout;
        }

        private void AddNavigation(
            FlowLayoutPanel host,
            string key,
            string title,
            string icon,
            Action action)
        {
            var button = new SidebarButton(title, icon)
            {
                Width = 218,
                Height = 46,
                Margin = new Padding(0, 3, 0, 3)
            };

            button.Click += (_, _) => action();

            _navButtons[key] = button;
            host.Controls.Add(button);
        }

        private void SetActiveNavigation(string key)
        {
            foreach (var item in _navButtons)
                item.Value.IsActive = item.Key == key;
        }

        private Panel BuildTopBar()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(
                    Color.FromArgb(231, 235, 243));

                e.Graphics.DrawLine(
                    pen,
                    0,
                    panel.Height - 1,
                    panel.Width,
                    panel.Height - 1);
            };

            _pageTitle = new Label
            {
                Text = "Overview",
                Location = new Point(27, 12),
                Size = new Size(400, 29),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = TextColor,
                BackColor = Color.White
            };

            _pageSubtitle = new Label
            {
                Text = "Your asset operations at a glance",
                Location = new Point(29, 44),
                Size = new Size(450, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = MutedColor,
                BackColor = Color.White
            };

            _userAvatar = new Panel
            {
                Size = new Size(36, 36),
                BackColor = Color.FromArgb(235, 241, 255)
            };

            _userAvatar.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var brush = new SolidBrush(
                    Color.FromArgb(235, 241, 255));

                e.Graphics.FillEllipse(
                    brush, 0, 0,
                    _userAvatar.Width - 1,
                    _userAvatar.Height - 1);

                TextRenderer.DrawText(
                    e.Graphics,
                    GetUserInitials(),
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    _userAvatar.ClientRectangle,
                    PrimaryColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            };

            _userNameLabel = new Label
            {
                AutoEllipsis = true,
                Size = new Size(130, 22),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextColor,
                BackColor = Color.White,
                Text = GetUserDisplayName()
            };

            _userRoleLabel = new Label
            {
                Size = new Size(130, 18),
                Font = new Font("Segoe UI", 8F),
                ForeColor = MutedColor,
                BackColor = Color.White,
                Text = GetUserRole()
            };

            var demoBadge = new Label
            {
                Text = "DEMO DATA",
                Size = new Size(92, 29),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                BackColor = Color.FromArgb(235, 241, 255)
            };

            var minimizeButton = CreateWindowButton("−");
            var maximizeButton = CreateWindowButton("□");
            var closeButton = CreateWindowButton("×");

            minimizeButton.Click += (_, _) =>
                WindowState = FormWindowState.Minimized;

            maximizeButton.Click += (_, _) =>
            {
                WindowState = WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;

                maximizeButton.Text =
                    WindowState == FormWindowState.Maximized ? "❐" : "□";
            };

            closeButton.Click += (_, _) => Close();

            panel.Controls.AddRange(new Control[]
            {
                _pageTitle,
                _pageSubtitle,
                _userAvatar,
                _userNameLabel,
                _userRoleLabel,
                demoBadge,
                minimizeButton,
                maximizeButton,
                closeButton
            });

            panel.Resize += (_, _) =>
            {
                bool showProfile = panel.ClientSize.Width >= 760;

                _userAvatar.Visible = showProfile;
                _userNameLabel.Visible = showProfile;
                _userRoleLabel.Visible = showProfile;

                _userAvatar.Location = new Point(
                    panel.ClientSize.Width - 460, 22);

                _userNameLabel.Location = new Point(
                    panel.ClientSize.Width - 414, 18);

                _userRoleLabel.Location = new Point(
                    panel.ClientSize.Width - 414, 41);

                demoBadge.Location = new Point(
                    panel.ClientSize.Width - 273, 27);

                minimizeButton.Location = new Point(
                    panel.ClientSize.Width - 162, 25);

                maximizeButton.Location = new Point(
                    panel.ClientSize.Width - 120, 25);

                closeButton.Location = new Point(
                    panel.ClientSize.Width - 78, 25);
            };

            panel.MouseDown += Window_MouseDown;
            panel.MouseMove += Window_MouseMove;
            panel.MouseUp += Window_MouseUp;

            _pageTitle.MouseDown += Window_MouseDown;
            _pageTitle.MouseMove += Window_MouseMove;
            _pageTitle.MouseUp += Window_MouseUp;

            _pageSubtitle.MouseDown += Window_MouseDown;
            _pageSubtitle.MouseMove += Window_MouseMove;
            _pageSubtitle.MouseUp += Window_MouseUp;

            return panel;
        }

        private Button CreateWindowButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(34, 31),
                Location = new Point(0, 25),
                Font = new Font("Segoe UI", 12F),
                ForeColor = TextColor,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand,
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(239, 242, 248);

            return button;
        }

        // ------------------------------------------------------------
        // User information
        // ------------------------------------------------------------

        private static string ReadUserProperty(params string[] propertyNames)
        {
            var user = Program.UserData;

            if (user == null)
                return string.Empty;

            foreach (string propertyName in propertyNames)
            {
                try
                {
                    var property = user.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);

                    if (property == null ||
                        !property.CanRead ||
                        property.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    object? value = property.GetValue(user);

                    if (value == null)
                        continue;

                    string result = value.ToString()?.Trim() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(result))
                        return result;
                }
                catch
                {
                    // Ignore properties that cannot be read.
                }
            }

            return string.Empty;
        }

        private static string GetUserDisplayName()
        {
            string displayName = ReadUserProperty(
                "DisplayName", "FullName");

            if (!string.IsNullOrWhiteSpace(displayName))
                return displayName;

            string firstName = ReadUserProperty("FirstName");
            string lastName = ReadUserProperty("LastName");

            string fullName = $"{firstName} {lastName}".Trim();

            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;

            string username = ReadUserProperty(
                "Username",
                "UserName",
                "LoginName",
                "Email",
                "Name",
                "UserCode",
                "UserID",
                "UserId");

            return string.IsNullOrWhiteSpace(username)
                ? "User"
                : username;
        }

        private static string GetUserRole()
        {
            string role = ReadUserProperty(
                "RoleName",
                "RoleTitle",
                "UserType",
                "RoleDescription");

            return string.IsNullOrWhiteSpace(role)
                ? "Signed in"
                : role;
        }

        private static string GetUserInitials()
        {
            string name = GetUserDisplayName();

            string[] parts = name.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return string.Concat(
                    parts[0][0],
                    parts[^1][0]).ToUpperInvariant();
            }

            return name.Length >= 2
                ? name.Substring(0, 2).ToUpperInvariant()
                : name.ToUpperInvariant();
        }

        // ------------------------------------------------------------
        // Navigation and logout
        // ------------------------------------------------------------

        private void SetPage(string title, string subtitle)
        {
            _pageTitle.Text = title;
            _pageSubtitle.Text = subtitle;
        }

        private void ShowDashboard()
        {
            SetPage("Overview", $"Welcome back, {GetUserDisplayName()}");
            SetActiveNavigation("dashboard");

            ShowView(new DashboardView(
                ShowPeople,
                ShowPlaceholder));
        }

        private void ShowPeople()
        {
            SetPage("People", "Manage people and access records");
            SetActiveNavigation("people");

            ShowView(new PersonListView());
        }

        private void ShowPlaceholder(string section)
        {
            SetPage(section, $"Your {section.ToLowerInvariant()} workspace");
            SetActiveNavigation(section.ToLowerInvariant());

            ShowView(new PlaceholderView(section));
        }

        private void ShowView(UserControl view)
        {
            _contentHost.SuspendLayout();

            try
            {
                while (_contentHost.Controls.Count > 0)
                {
                    Control oldControl = _contentHost.Controls[0];

                    _contentHost.Controls.RemoveAt(0);
                    oldControl.Dispose();
                }

                view.Dock = DockStyle.Fill;
                _contentHost.Controls.Add(view);
                view.BringToFront();
            }
            finally
            {
                _contentHost.ResumeLayout(true);
            }
        }

        private void Logout()
        {
            DialogResult answer = MessageBox.Show(
                this,
                "Are you sure you want to sign out?",
                "Confirm Sign Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            // Clear the in-memory user session.
            Program.UserData = null;

            Close();
        }

        // ------------------------------------------------------------
        // Window dragging
        // ------------------------------------------------------------

        private void Window_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                WindowState == FormWindowState.Maximized)
            {
                return;
            }

            _isDragging = true;
            _dragStart = Cursor.Position;
            _formStart = Location;
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
        }

        private static GraphicsPath CreateRoundedPath(
            Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            radius = Math.Max(
                1, Math.Min(radius,
                Math.Min(bounds.Width, bounds.Height) / 2));

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

        // ------------------------------------------------------------
        // Dashboard view
        // ------------------------------------------------------------

        private sealed class DashboardView : UserControl
        {
            private readonly Panel _scrollPanel;
            private readonly Panel _canvas;

            private readonly MetricCard[] _metrics;
            private readonly DashboardCard _trendCard;
            private readonly DashboardCard _statusCard;
            private readonly DashboardCard _activityCard;
            private readonly DashboardCard _quickActionsCard;

            private readonly Action _onManagePeople;
            private readonly Action<string> _onPlaceholder;

            public DashboardView(
                Action onManagePeople,
                Action<string> onPlaceholder)
            {
                _onManagePeople = onManagePeople;
                _onPlaceholder = onPlaceholder;

                Dock = DockStyle.Fill;
                BackColor = CanvasColor;

                _scrollPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    BackColor = CanvasColor
                };

                _canvas = new Panel
                {
                    BackColor = CanvasColor
                };

                var introTitle = new Label
                {
                    Text = "Workspace overview",
                    Location = new Point(0, 0),
                    Size = new Size(420, 26),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = TextColor,
                    BackColor = CanvasColor
                };

                var introSubtitle = new Label
                {
                    Text = "Key metrics and recent activity",
                    Location = new Point(1, 27),
                    Size = new Size(420, 20),
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = MutedColor,
                    BackColor = CanvasColor
                };

                var rangeLabel = new Label
                {
                    Text = "SAMPLE DATA  •  LAST 30 DAYS",
                    Size = new Size(225, 23),
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = MutedColor,
                    BackColor = CanvasColor
                };

                _metrics = new[]
                {
                    new MetricCard(
                        "Total assets", "1,284",
                        "↑  8.2% vs last month", "▣",
                        Color.FromArgb(75, 105, 232),
                        Color.FromArgb(237, 241, 255)),

                    new MetricCard(
                        "Assigned assets", "936",
                        "↑  5.4% vs last month", "↗",
                        Color.FromArgb(28, 165, 136),
                        Color.FromArgb(230, 248, 242)),

                    new MetricCard(
                        "Available assets", "248",
                        "Ready to assign", "✓",
                        Color.FromArgb(222, 147, 56),
                        Color.FromArgb(255, 245, 231)),

                    new MetricCard(
                        "People", "156",
                        "4 new this month", "♙",
                        Color.FromArgb(139, 90, 210),
                        Color.FromArgb(245, 237, 255))
                };

                var barChart = new BarChartPanel();
                var donutChart = new DonutChartPanel();

                _trendCard = CreateChartCard(
                    "Asset assignments",
                    "Monthly assignment activity (sample)",
                    barChart);

                _statusCard = CreateChartCard(
                    "Asset status",
                    "Distribution by current status",
                    donutChart);

                _activityCard = BuildActivityCard();
                _quickActionsCard = BuildQuickActionsCard();

                _canvas.Controls.AddRange(new Control[]
                {
                    introTitle,
                    introSubtitle,
                    rangeLabel,
                    _trendCard,
                    _statusCard,
                    _activityCard,
                    _quickActionsCard
                });

                foreach (MetricCard metric in _metrics)
                    _canvas.Controls.Add(metric);

                _scrollPanel.Controls.Add(_canvas);
                Controls.Add(_scrollPanel);

                _scrollPanel.Resize += (_, _) => LayoutDashboard();

                LayoutDashboard();

                void UpdateRangePosition()
                {
                    rangeLabel.Location = new Point(
                        _canvas.Width - rangeLabel.Width, 2);
                }

                _scrollPanel.Resize += (_, _) => UpdateRangePosition();
            }

            private void LayoutDashboard()
            {
                int width = Math.Max(
                    650, _scrollPanel.ClientSize.Width - 48);

                _canvas.Location = new Point(24, 18);
                _canvas.Size = new Size(width, 770);

                int gap = 12;
                int metricWidth = (width - gap * 3) / 4;

                for (int i = 0; i < _metrics.Length; i++)
                {
                    _metrics[i].Location = new Point(
                        i * (metricWidth + gap), 58);

                    _metrics[i].Size = new Size(metricWidth, 124);
                }

                int chartGap = 14;
                int trendWidth = (int)((width - chartGap) * 0.59);
                int statusWidth = width - chartGap - trendWidth;

                _trendCard.Location = new Point(0, 196);
                _trendCard.Size = new Size(trendWidth, 294);

                _statusCard.Location = new Point(
                    trendWidth + chartGap, 196);

                _statusCard.Size = new Size(statusWidth, 294);

                int activityWidth = (int)((width - chartGap) * 0.59);
                int quickWidth = width - chartGap - activityWidth;

                _activityCard.Location = new Point(0, 506);
                _activityCard.Size = new Size(activityWidth, 246);

                _quickActionsCard.Location = new Point(
                    activityWidth + chartGap, 506);

                _quickActionsCard.Size = new Size(quickWidth, 246);

                _scrollPanel.AutoScrollMinSize = new Size(0, 810);
            }

            private DashboardCard CreateChartCard(
                string title,
                string subtitle,
                Control chart)
            {
                var card = new DashboardCard();

                var titleLabel = new Label
                {
                    Text = title,
                    Location = new Point(19, 16),
                    Size = new Size(320, 25),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = TextColor,
                    BackColor = Color.White
                };

                var subtitleLabel = new Label
                {
                    Text = subtitle,
                    Location = new Point(20, 42),
                    Size = new Size(340, 19),
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = MutedColor,
                    BackColor = Color.White
                };

                var chartHost = new Panel
                {
                    Location = new Point(17, 67),
                    Size = new Size(300, 200),
                    BackColor = Color.White
                };

                chart.Dock = DockStyle.Fill;
                chartHost.Controls.Add(chart);

                card.Controls.AddRange(new Control[]
                {
                    titleLabel, subtitleLabel, chartHost
                });

                card.Resize += (_, _) =>
                {
                    titleLabel.Width = Math.Max(
                        180, card.ClientSize.Width - 38);

                    subtitleLabel.Width = Math.Max(
                        180, card.ClientSize.Width - 38);

                    chartHost.Location = new Point(17, 68);

                    chartHost.Size = new Size(
                        Math.Max(80, card.ClientSize.Width - 34),
                        Math.Max(100, card.ClientSize.Height - 83));
                };

                return card;
            }

            private DashboardCard BuildActivityCard()
            {
                var card = new DashboardCard();

                card.Controls.Add(new Label
                {
                    Text = "Recent activity",
                    Location = new Point(19, 15),
                    Size = new Size(300, 25),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = TextColor,
                    BackColor = Color.White
                });

                card.Controls.Add(new Label
                {
                    Text = "Sample updates from your workspace",
                    Location = new Point(20, 41),
                    Size = new Size(340, 19),
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = MutedColor,
                    BackColor = Color.White
                });

                string[] titles =
                {
                    "New asset registered",
                    "Asset assignment updated",
                    "People record updated",
                    "Maintenance scheduled"
                };

                string[] details =
                {
                    "Laptop added to inventory",
                    "Device assigned to Operations",
                    "Employee details were changed",
                    "A device was marked for review"
                };

                string[] times =
                {
                    "12 min ago",
                    "1 hour ago",
                    "3 hours ago",
                    "Yesterday"
                };

                Color[] accents =
                {
                    Color.FromArgb(75, 105, 232),
                    Color.FromArgb(28, 165, 136),
                    Color.FromArgb(139, 90, 210),
                    Color.FromArgb(222, 147, 56)
                };

                var rows = new List<ActivityRow>();

                for (int i = 0; i < titles.Length; i++)
                {
                    var row = new ActivityRow(
                        titles[i], details[i], times[i], accents[i]);

                    rows.Add(row);
                    card.Controls.Add(row);
                }

                card.Resize += (_, _) =>
                {
                    for (int i = 0; i < rows.Count; i++)
                    {
                        rows[i].Location = new Point(18, 69 + i * 40);

                        rows[i].Size = new Size(
                            Math.Max(100, card.ClientSize.Width - 36),
                            38);
                    }
                };

                return card;
            }

            private DashboardCard BuildQuickActionsCard()
            {
                var card = new DashboardCard();

                card.Controls.Add(new Label
                {
                    Text = "Quick actions",
                    Location = new Point(19, 15),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = TextColor,
                    BackColor = Color.White
                });

                card.Controls.Add(new Label
                {
                    Text = "Jump straight into a task",
                    Location = new Point(20, 41),
                    Size = new Size(280, 19),
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = MutedColor,
                    BackColor = Color.White
                });

                var peopleButton = CreateActionButton("♙   Manage people   →");
                var assetsButton = CreateActionButton("▣   Browse assets   →");
                var reportsButton = CreateActionButton("▤   View reports   →");

                peopleButton.Click += (_, _) => _onManagePeople();
                assetsButton.Click += (_, _) => _onPlaceholder("Assets");
                reportsButton.Click += (_, _) => _onPlaceholder("Reports");

                card.Controls.AddRange(new Control[]
                {
                    peopleButton, assetsButton, reportsButton
                });

                card.Resize += (_, _) =>
                {
                    int buttonWidth = Math.Max(
                        150, card.ClientSize.Width - 36);

                    peopleButton.Location = new Point(18, 72);
                    peopleButton.Size = new Size(buttonWidth, 42);

                    assetsButton.Location = new Point(18, 121);
                    assetsButton.Size = new Size(buttonWidth, 42);

                    reportsButton.Location = new Point(18, 170);
                    reportsButton.Size = new Size(buttonWidth, 42);
                };

                return card;
            }

            private static Button CreateActionButton(string title)
            {
                var button = new Button
                {
                    Text = title,
                    Size = new Size(250, 42),
                    BackColor = Color.FromArgb(248, 250, 254),
                    ForeColor = TextColor,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false
                };

                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor =
                    Color.FromArgb(228, 233, 242);

                button.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(238, 243, 255);

                return button;
            }
        }

        // ------------------------------------------------------------
        // Sidebar button
        // ------------------------------------------------------------

        private sealed class SidebarButton : Button
        {
            private readonly string _caption;
            private readonly string _icon;
            private bool _isHovered;
            private bool _isActive;

            [DefaultValue(false)]
            public bool IsActive
            {
                get => _isActive;
                set
                {
                    if (_isActive == value)
                        return;

                    _isActive = value;
                    Invalidate();
                }
            }

            public SidebarButton(string caption, string icon)
            {
                _caption = caption;
                _icon = icon;

                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                BackColor = SidebarColor;
                ForeColor = Color.White;
                Font = new Font("Segoe UI", 9.5F);
                Cursor = Cursors.Hand;
                Text = string.Empty;
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
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(SidebarColor);

                if (_isActive || _isHovered)
                {
                    using var path = CreateRoundedPath(
                        new Rectangle(1, 1, Width - 3, Height - 3), 9);

                    using var brush = new SolidBrush(
                        _isActive
                            ? Color.FromArgb(46, 64, 98)
                            : Color.FromArgb(34, 49, 76));

                    e.Graphics.FillPath(brush, path);
                }

                if (_isActive)
                {
                    using var accentBrush = new SolidBrush(PrimaryColor);

                    e.Graphics.FillRectangle(
                        accentBrush, 1, 10, 3, Height - 20);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    _icon,
                    new Font("Segoe UI Symbol", 15F),
                    new Rectangle(13, 0, 35, Height),
                    _isActive
                        ? Color.FromArgb(145, 172, 255)
                        : Color.FromArgb(171, 184, 207),
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    _caption,
                    new Font(
                        "Segoe UI", 9.5F,
                        _isActive ? FontStyle.Bold : FontStyle.Regular),
                    new Rectangle(57, 0, Width - 70, Height),
                    _isActive ? Color.White : Color.FromArgb(205, 214, 231),
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);
            }
        }

        // ------------------------------------------------------------
        // Dashboard cards
        // ------------------------------------------------------------

        private sealed class DashboardCard : Panel
        {
            public DashboardCard()
            {
                BackColor = Color.White;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using var brush = new SolidBrush(
                    Parent?.BackColor ?? CanvasColor);

                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var path = CreateRoundedPath(
                    new Rectangle(0, 0, Width - 1, Height - 1), 13);

                using var brush = new SolidBrush(Color.White);

                using var pen = new Pen(
                    Color.FromArgb(230, 234, 242));

                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private sealed class MetricCard : Panel
        {
            private readonly string _title;
            private readonly string _value;
            private readonly string _change;
            private readonly string _icon;
            private readonly Color _accent;
            private readonly Color _iconBackground;

            public MetricCard(
                string title,
                string value,
                string change,
                string icon,
                Color accent,
                Color iconBackground)
            {
                _title = title;
                _value = value;
                _change = change;
                _icon = icon;
                _accent = accent;
                _iconBackground = iconBackground;

                BackColor = Color.White;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using var brush = new SolidBrush(
                    Parent?.BackColor ?? CanvasColor);

                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var path = CreateRoundedPath(
                    new Rectangle(0, 0, Width - 1, Height - 1), 12);

                using var brush = new SolidBrush(Color.White);
                using var borderPen = new Pen(
                    Color.FromArgb(230, 234, 242));

                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(borderPen, path);

                var iconBounds = new Rectangle(
                    Math.Max(Width - 57, 8), 15, 39, 39);

                using var iconPath = CreateRoundedPath(iconBounds, 10);
                using var iconBrush = new SolidBrush(_iconBackground);

                e.Graphics.FillPath(iconBrush, iconPath);

                TextRenderer.DrawText(
                    e.Graphics,
                    _icon,
                    new Font("Segoe UI Symbol", 15F),
                    iconBounds,
                    _accent,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    _title,
                    new Font("Segoe UI", 9F),
                    new Rectangle(17, 15, Math.Max(75, Width - 82), 27),
                    MutedColor,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);

                TextRenderer.DrawText(
                    e.Graphics,
                    _value,
                    new Font("Segoe UI", 23F, FontStyle.Bold),
                    new Rectangle(17, 43, Width - 30, 38),
                    TextColor,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);

                TextRenderer.DrawText(
                    e.Graphics,
                    _change,
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    new Rectangle(17, 88, Width - 25, 23),
                    _change.StartsWith("↑")
                        ? Color.FromArgb(30, 155, 111)
                        : _accent,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine |
                    TextFormatFlags.EndEllipsis);
            }
        }

        // ------------------------------------------------------------
        // Bar chart
        // ------------------------------------------------------------

        private sealed class BarChartPanel : Panel
        {
            private readonly string[] _labels =
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun"
            };

            private readonly int[] _values =
            {
                42, 58, 49, 73, 65, 91
            };

            public BarChartPanel()
            {
                BackColor = Color.White;
                DoubleBuffered = true;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White);

                int left = 38;
                int right = 12;
                int top = 17;
                int bottom = 31;

                int chartWidth = Width - left - right;
                int chartHeight = Height - top - bottom;

                if (chartWidth <= 0 || chartHeight <= 0)
                    return;

                using var gridPen = new Pen(
                    Color.FromArgb(233, 237, 244));

                using var axisFont = new Font("Segoe UI", 7.5F);
                using var valueFont = new Font(
                    "Segoe UI", 7.5F, FontStyle.Bold);

                for (int tick = 0; tick <= 100; tick += 25)
                {
                    float y = top + chartHeight -
                        (chartHeight * tick / 100F);

                    e.Graphics.DrawLine(
                        gridPen, left, y, Width - right, y);

                    TextRenderer.DrawText(
                        e.Graphics,
                        tick.ToString(),
                        axisFont,
                        new Rectangle(0, (int)y - 8, left - 7, 17),
                        MutedColor,
                        TextFormatFlags.Right |
                        TextFormatFlags.VerticalCenter);
                }

                float step = chartWidth / (float)_values.Length;

                int barWidth = Math.Max(
                    12, Math.Min(32, (int)(step * 0.44F)));

                for (int i = 0; i < _values.Length; i++)
                {
                    float barHeight =
                        chartHeight * _values[i] / 100F;

                    int x = left +
                        (int)(i * step) +
                        ((int)step - barWidth) / 2;

                    int y = top + chartHeight - (int)barHeight;

                    var barBounds = new Rectangle(
                        x, y, barWidth, Math.Max(2, (int)barHeight));

                    using var barPath = CreateRoundedPath(
                        barBounds, Math.Min(7, barWidth / 2));

                    using var gradient = new LinearGradientBrush(
                        barBounds,
                        Color.FromArgb(103, 131, 250),
                        Color.FromArgb(65, 96, 225),
                        LinearGradientMode.Vertical);

                    e.Graphics.FillPath(gradient, barPath);

                    TextRenderer.DrawText(
                        e.Graphics,
                        _values[i].ToString(),
                        valueFont,
                        new Rectangle(x - 5, y - 19, barWidth + 10, 17),
                        Color.FromArgb(82, 94, 119),
                        TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter);

                    TextRenderer.DrawText(
                        e.Graphics,
                        _labels[i],
                        axisFont,
                        new Rectangle(
                            left + (int)(i * step),
                            top + chartHeight + 7,
                            (int)step, 20),
                        MutedColor,
                        TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter);
                }
            }
        }

        // ------------------------------------------------------------
        // Donut chart
        // ------------------------------------------------------------

        private sealed class DonutChartPanel : Panel
        {
            private readonly string[] _labels =
            {
                "Assigned", "Available", "Maintenance"
            };

            private readonly int[] _values =
            {
                936, 248, 100
            };

            private readonly Color[] _colors =
            {
                Color.FromArgb(69, 101, 231),
                Color.FromArgb(39, 181, 147),
                Color.FromArgb(242, 169, 70)
            };

            private int Total =>
                _values[0] + _values[1] + _values[2];

            public DonutChartPanel()
            {
                BackColor = Color.White;
                DoubleBuffered = true;

                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White);

                int diameter = Math.Min(
                    140, Math.Max(90, Height - 20));

                int x = 3;
                int y = Math.Max(3, (Height - diameter) / 2);

                var bounds = new Rectangle(
                    x, y, diameter, diameter);

                float startAngle = -90F;

                for (int i = 0; i < _values.Length; i++)
                {
                    float sweep = 360F * _values[i] / Total;

                    using var brush = new SolidBrush(_colors[i]);

                    e.Graphics.FillPie(
                        brush, bounds, startAngle, sweep);

                    startAngle += sweep;
                }

                int inset = (int)(diameter * 0.23F);

                using (var innerBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillEllipse(
                        innerBrush,
                        bounds.X + inset,
                        bounds.Y + inset,
                        bounds.Width - inset * 2,
                        bounds.Height - inset * 2);
                }

                var center = new Rectangle(
                    bounds.X + inset,
                    bounds.Y + inset,
                    bounds.Width - inset * 2,
                    bounds.Height - inset * 2);

                TextRenderer.DrawText(
                    e.Graphics,
                    Total.ToString("N0"),
                    new Font("Segoe UI", 12F, FontStyle.Bold),
                    new Rectangle(
                        center.X, center.Y + 4,
                        center.Width, center.Height / 2),
                    TextColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "TOTAL",
                    new Font("Segoe UI", 7F, FontStyle.Bold),
                    new Rectangle(
                        center.X, center.Y + center.Height / 2 - 1,
                        center.Width, 18),
                    MutedColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);

                int legendX = diameter + 16;
                int rowHeight = Math.Min(
                    54, Math.Max(38, (Height - 5) / 3));

                using var nameFont = new Font("Segoe UI", 8F, FontStyle.Bold);
                using var valueFont = new Font("Segoe UI", 10F, FontStyle.Bold);

                for (int i = 0; i < _values.Length; i++)
                {
                    int rowY = 16 + i * rowHeight;

                    using var swatch = new SolidBrush(_colors[i]);

                    e.Graphics.FillEllipse(
                        swatch, legendX, rowY + 4, 9, 9);

                    TextRenderer.DrawText(
                        e.Graphics,
                        _labels[i],
                        nameFont,
                        new Rectangle(
                            legendX + 15, rowY,
                            Math.Max(55, Width - legendX - 20), 20),
                        MutedColor,
                        TextFormatFlags.Left |
                        TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis);

                    TextRenderer.DrawText(
                        e.Graphics,
                        _values[i].ToString("N0"),
                        valueFont,
                        new Rectangle(
                            legendX + 15, rowY + 19,
                            Math.Max(55, Width - legendX - 20), 23),
                        TextColor,
                        TextFormatFlags.Left |
                        TextFormatFlags.VerticalCenter);
                }
            }
        }

        // ------------------------------------------------------------
        // Recent activity
        // ------------------------------------------------------------

        private sealed class ActivityRow : Panel
        {
            private readonly string _title;
            private readonly string _detail;
            private readonly string _time;
            private readonly Color _accent;

            public ActivityRow(
                string title,
                string detail,
                string time,
                Color accent)
            {
                _title = title;
                _detail = detail;
                _time = time;
                _accent = accent;

                BackColor = Color.White;
                DoubleBuffered = true;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using var brush = new SolidBrush(Color.White);
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                using var dotBrush = new SolidBrush(_accent);

                e.Graphics.FillEllipse(dotBrush, 2, 8, 9, 9);

                int timeWidth = 80;
                int textWidth = Math.Max(
                    60, Width - timeWidth - 32);

                TextRenderer.DrawText(
                    e.Graphics,
                    _title,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    new Rectangle(18, 0, textWidth, 19),
                    TextColor,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    _detail,
                    new Font("Segoe UI", 7.5F),
                    new Rectangle(18, 18, textWidth, 17),
                    MutedColor,
                    TextFormatFlags.Left |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    _time,
                    new Font("Segoe UI", 7F),
                    new Rectangle(
                        Width - timeWidth, 3,
                        timeWidth - 2, 17),
                    MutedColor,
                    TextFormatFlags.Right |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);
            }
        }

        // ------------------------------------------------------------
        // Placeholder pages
        // ------------------------------------------------------------

        private sealed class PlaceholderView : UserControl
        {
            private readonly Panel _card;
            private readonly Label _titleLabel;

            public PlaceholderView(string section)
            {
                Dock = DockStyle.Fill;
                BackColor = CanvasColor;

                _card = new Panel
                {
                    Size = new Size(470, 270),
                    BackColor = Color.White
                };

                _card.Paint += (_, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    using var path = CreateRoundedPath(
                        new Rectangle(
                            0, 0, _card.Width - 1, _card.Height - 1),
                        16);

                    using var brush = new SolidBrush(Color.White);
                    using var pen = new Pen(
                        Color.FromArgb(229, 234, 243));

                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                };

                var iconLabel = new Label
                {
                    Text = "✦",
                    Location = new Point(200, 30),
                    Size = new Size(70, 65),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI Symbol", 28F),
                    ForeColor = PrimaryColor,
                    BackColor = Color.FromArgb(238, 242, 255)
                };

                _titleLabel = new Label
                {
                    Text = section,
                    Location = new Point(30, 112),
                    Size = new Size(410, 35),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                    ForeColor = TextColor,
                    BackColor = Color.White
                };

                var description = new Label
                {
                    Text = "This section is a placeholder.\nConnect your service here when it is ready.",
                    Location = new Point(30, 155),
                    Size = new Size(410, 55),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = MutedColor,
                    BackColor = Color.White
                };

                _card.Controls.AddRange(new Control[]
                {
                    iconLabel, _titleLabel, description
                });

                Controls.Add(_card);

                Resize += (_, _) => CenterCard();
                CenterCard();
            }

            private void CenterCard()
            {
                _card.Location = new Point(
                    Math.Max(12, (ClientSize.Width - _card.Width) / 2),
                    Math.Max(12, (ClientSize.Height - _card.Height) / 2));
            }
        }
    }
}