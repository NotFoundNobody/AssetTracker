
using Core.DTO;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI.Views;

namespace UI.Views
{
    public partial class PersonListView : UserControl
    {
        private readonly IPersonService _personService;

        private readonly DataGridView _grid;
        private readonly TextBox _searchBox;
        private readonly Label _countLabel;
        private readonly Label _statusLabel;

        private readonly Button _addButton;
        private readonly Button _editButton;
        private readonly Button _deleteButton;
        private readonly Button _refreshButton;

        private List<PersonDto> _person = new();

        private readonly Color _primaryColor = Color.FromArgb(55, 100, 190);
        private readonly Color _backgroundColor = Color.FromArgb(245, 247, 250);
        private readonly Color _textColor = Color.FromArgb(45, 55, 72);

        public PersonListView()
        {
            _personService = Program.ServiceProvider
                .GetRequiredService<IPersonService>();

            Dock = DockStyle.Fill;
            BackColor = _backgroundColor;
            Padding = new Padding(24);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 82
            };

            var titleLabel = new Label
            {
                Text = "Person Management",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = _textColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var descriptionLabel = new Label
            {
                Text = "Manage and maintain your person records.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(3, 43)
            };

            _countLabel = new Label
            {
                Text = "Total person: 0",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = _primaryColor,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(descriptionLabel);
            headerPanel.Controls.Add(_countLabel);

            headerPanel.Resize += (_, _) =>
            {
                _countLabel.Location = new Point(
                    headerPanel.ClientSize.Width - _countLabel.Width - 8,
                    24);
            };

            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54
            };

            _searchBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, 8),
                Height = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _searchBox.TextChanged += (_, _) => ApplySearch();

            searchPanel.Controls.Add(_searchBox);

            searchPanel.Resize += (_, _) =>
            {
                _searchBox.Width = Math.Max(200, searchPanel.ClientSize.Width);
            };

            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 4)
            };

            _addButton = CreateButton(
                "Add Person",
                Color.FromArgb(39, 174, 96));

            _editButton = CreateButton(
                "Edit Selected",
                _primaryColor);

            _deleteButton = CreateButton(
                "Delete Selected",
                Color.FromArgb(220, 70, 70));

            _refreshButton = CreateButton(
                "Refresh",
                Color.FromArgb(100, 116, 139));

            actionPanel.Controls.Add(_addButton);
            actionPanel.Controls.Add(_editButton);
            actionPanel.Controls.Add(_deleteButton);
            actionPanel.Controls.Add(_refreshButton);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 10),
                GridColor = Color.FromArgb(230, 234, 240)
            };

            _grid.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(235, 240, 248);

            _grid.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;

            _grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            _grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);
            _grid.ColumnHeadersHeight = 44;
            _grid.DefaultCellStyle.Padding = new Padding(8);

            _grid.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 232, 252);

            _grid.DefaultCellStyle.SelectionForeColor = _textColor;
            _grid.RowTemplate.Height = 42;

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PersonID",
                HeaderText = "Person ID",
                DataPropertyName = nameof(PersonDto.PersonID),
                Visible = false
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FirstName",
                HeaderText = "First Name",
                DataPropertyName = nameof(PersonDto.FirstName)
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastName",
                HeaderText = "Last Name",
                DataPropertyName = nameof(PersonDto.LastName)
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NationalCode",
                HeaderText = "National Code",
                DataPropertyName = nameof(PersonDto.NationaleCode)
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CellPhone",
                HeaderText = "Phone Number",
                DataPropertyName = nameof(PersonDto.CellPhone)
            });

            _statusLabel = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Height = 28,
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 8, 0, 0),
                BackColor = Color.White
            };

            gridPanel.Controls.Add(_grid);

            Controls.Add(gridPanel);
            Controls.Add(_statusLabel);
            Controls.Add(actionPanel);
            Controls.Add(searchPanel);
            Controls.Add(headerPanel);

            _addButton.Click += async (_, _) => await AddPersonAsync();
            _editButton.Click += async (_, _) => await EditSelectedPersonAsync();
            _deleteButton.Click += async (_, _) => await DeleteSelectedPersonAsync();
            _refreshButton.Click += async (_, _) => await LoadDataAsync();

            _grid.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0)
                    await EditSelectedPersonAsync();
            };

            _grid.SelectionChanged += (_, _) => UpdateActionButtons();

            Load += async (_, _) => await LoadDataAsync();

            UpdateActionButtons();
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                Width = 135,
                Height = 34,
                Margin = new Padding(0, 0, 10, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
        }

        private async Task LoadDataAsync()
        {
            try
            {
                SetBusy(true, "Loading person...");

                var result = await _personService.GetPersonList(
                    Program.UserData.Person!);

                _person = result ?? new List<PersonDto>();

                ApplySearch();

                _statusLabel.Text = "person loaded successfully.";
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Failed to load person.";

                MessageBox.Show(
                    $"Unable to load person.\n\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false, _statusLabel.Text);
            }
        }

        private void ApplySearch()
        {
            string keyword = _searchBox.Text.Trim();

            IEnumerable<PersonDto> filteredPerson = _person;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filteredPerson = _person.Where(person =>
                    (person.FirstName ?? string.Empty)
                        .Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (person.LastName ?? string.Empty)
                        .Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (person.NationaleCode ?? string.Empty)
                        .Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (person.CellPhone ?? string.Empty)
                        .Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            _grid.DataSource = null;
            _grid.DataSource = filteredPerson.ToList();

            _countLabel.Text = $"Total person: {_person.Count}";

            UpdateActionButtons();
        }

        private PersonDto? GetSelectedPerson()
        {
            return _grid.CurrentRow?.DataBoundItem as PersonDto;
        }

        private async Task AddPersonAsync()
        {
            using var form = CreatePersonForm("Add Person");

            var view = new PersonView(_personService)
            {
                Dock = DockStyle.Fill
            };

            form.Controls.Add(view);
            form.ShowDialog(FindForm());

            await LoadDataAsync();
        }

        private async Task EditSelectedPersonAsync()
        {
            var person = GetSelectedPerson();

            if (person == null)
            {
                MessageBox.Show(
                    "Please select a person to edit.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using var form = CreatePersonForm("Edit Person");

            var view = new PersonView(_personService, person)
            {
                Dock = DockStyle.Fill
            };

            form.Controls.Add(view);
            form.ShowDialog(FindForm());

            await LoadDataAsync();
        }

        private async Task DeleteSelectedPersonAsync()
        {
            var person = GetSelectedPerson();

            if (person == null)
            {
                MessageBox.Show(
                    "Please select a person to delete.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            string fullName = $"{person.FirstName} {person.LastName}".Trim();

            var confirmation = MessageBox.Show(
                $"Are you sure you want to delete {fullName}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
                return;

            try
            {
                SetBusy(true, "Deleting person...");

                bool success = await _personService.DeletePerson(person.PersonID);

                if (!success)
                {
                    MessageBox.Show(
                        "The person could not be deleted.",
                        "Delete Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                _statusLabel.Text = "Person deleted successfully.";

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to delete the person.\n\n{ex.Message}",
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false, _statusLabel.Text);
            }
        }

        private Form CreatePersonForm(string title)
        {
            return new Form
            {
                Text = title,
                ClientSize = new Size(650, 480),
                MinimumSize = new Size(600, 440),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = _backgroundColor,
                ShowInTaskbar = false
            };
        }

        private void UpdateActionButtons()
        {
            bool hasSelection = GetSelectedPerson() != null;

            _editButton.Enabled = hasSelection;
            _deleteButton.Enabled = hasSelection;
        }

        private void SetBusy(bool isBusy, string status)
        {
            _addButton.Enabled = !isBusy;
            _editButton.Enabled = !isBusy && GetSelectedPerson() != null;
            _deleteButton.Enabled = !isBusy && GetSelectedPerson() != null;
            _refreshButton.Enabled = !isBusy;
            _searchBox.Enabled = !isBusy;
            _grid.Enabled = !isBusy;

            _statusLabel.Text = status;
            UseWaitCursor = isBusy;
        }
    }
}