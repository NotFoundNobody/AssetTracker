
using Core.DTO;
using Core.Interfaces;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Views
{
    public partial class PersonView : UserControl
    {
        private readonly IPersonService _personService;
        private readonly PersonDto? _person;
        private readonly bool _isEditMode;

        private TextBox _firstNameTextBox = null!;
        private TextBox _lastNameTextBox = null!;
        private TextBox _nationalCodeTextBox = null!;
        private TextBox _cellPhoneTextBox = null!;
        private Button _saveButton = null!;
        private Button _cancelButton = null!;
        private Label _headerLabel = null!;

        private readonly Color _backgroundColor = Color.FromArgb(245, 247, 250);
        private readonly Color _primaryColor = Color.FromArgb(55, 100, 190);
        private readonly Color _textColor = Color.FromArgb(45, 55, 72);

        public PersonView(IPersonService personService, PersonDto? person = null)
        {
            _personService = personService
                ?? throw new ArgumentNullException(nameof(personService));

            _person = person;
            _isEditMode = person != null;

            BuildInterface();

            if (_isEditMode)
                LoadPersonData();
        }

        private void BuildInterface()
        {
            Dock = DockStyle.Fill;
            BackColor = _backgroundColor;
            Padding = new Padding(24);

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = _backgroundColor,
                Padding = new Padding(0)
            };

            mainPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65));

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 12));

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50));

            _headerLabel = new Label
            {
                Text = _isEditMode
                    ? "Edit Person Information"
                    : "Add New Person",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = _textColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var fieldsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                Padding = new Padding(18, 14, 18, 14)
            };

            fieldsPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 4; i++)
            {
                fieldsPanel.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 25));
            }

            _firstNameTextBox = CreateTextBox();
            _lastNameTextBox = CreateTextBox();
            _nationalCodeTextBox = CreateTextBox();
            _cellPhoneTextBox = CreateTextBox();

            fieldsPanel.Controls.Add(
                CreateField("First Name", _firstNameTextBox), 0, 0);

            fieldsPanel.Controls.Add(
                CreateField("Last Name", _lastNameTextBox), 0, 1);

            fieldsPanel.Controls.Add(
                CreateField("National Code", _nationalCodeTextBox), 0, 2);

            fieldsPanel.Controls.Add(
                CreateField("Phone Number", _cellPhoneTextBox), 0, 3);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = _backgroundColor,
                Padding = new Padding(0, 6, 0, 0)
            };

            _saveButton = CreateButton(
                _isEditMode ? "Save Changes" : "Add Person",
                _primaryColor);

            _cancelButton = CreateButton(
                "Cancel",
                Color.FromArgb(100, 116, 139));

            buttonPanel.Controls.Add(_saveButton);
            buttonPanel.Controls.Add(_cancelButton);

            mainPanel.Controls.Add(_headerLabel, 0, 0);
            mainPanel.Controls.Add(fieldsPanel, 0, 1);
            mainPanel.Controls.Add(new Panel(), 0, 2);
            mainPanel.Controls.Add(buttonPanel, 0, 3);

            Controls.Add(mainPanel);

            _saveButton.Click += async (_, _) => await SaveAsync();

            _cancelButton.Click += (_, _) => CloseParentForm();

        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 6, 0, 0)
            };
        }

        private Control CreateField(string labelText, TextBox textBox)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0, 2, 0, 6)
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = _textColor
            };

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);

            return panel;
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                Width = 145,
                Height = 36,
                Margin = new Padding(8, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
        }

        private void LoadPersonData()
        {
            if (_person == null)
                return;

            _firstNameTextBox.Text = _person.FirstName ?? string.Empty;
            _lastNameTextBox.Text = _person.LastName ?? string.Empty;
            _nationalCodeTextBox.Text = _person.NationaleCode ?? string.Empty;
            _cellPhoneTextBox.Text = _person.CellPhone ?? string.Empty;
        }

        private async Task SaveAsync()
        {
            string firstName = _firstNameTextBox.Text.Trim();
            string lastName = _lastNameTextBox.Text.Trim();
            string nationalCode = _nationalCodeTextBox.Text.Trim();
            string cellPhone = _cellPhoneTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowValidationMessage("Please enter the first name.");
                _firstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ShowValidationMessage("Please enter the last name.");
                _lastNameTextBox.Focus();
                return;
            }

            var confirmation = MessageBox.Show(
                _isEditMode
                    ? "Are you sure you want to save these changes?"
                    : "Are you sure you want to add this person?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes)
                return;

            _saveButton.Enabled = false;
            _cancelButton.Enabled = false;

            try
            {
                var personDto = new PersonDto
                {
                    PersonID = _isEditMode
                        ? _person!.PersonID
                        : Guid.NewGuid(),

                    FirstName = firstName,
                    LastName = lastName,
                    NationaleCode = nationalCode,
                    CellPhone = cellPhone
                };

                bool success;

                if (_isEditMode)
                {
                    success = await _personService.UpdatePerson(personDto);
                }
                else
                {
                    success = await _personService.AddPerson(personDto);
                }

                if (!success)
                {
                    MessageBox.Show(
                        _isEditMode
                            ? "Unable to update the person."
                            : "Unable to add the person.",
                        "Operation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                MessageBox.Show(
                    _isEditMode
                        ? "Person updated successfully."
                        : "Person added successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CloseParentForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while saving the person.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    _saveButton.Enabled = true;
                    _cancelButton.Enabled = true;
                }
            }
        }

        private void ShowValidationMessage(string message)
        {
            MessageBox.Show(
                message,
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void CloseParentForm()
        {
            FindForm()?.Close();
        }
    }
}