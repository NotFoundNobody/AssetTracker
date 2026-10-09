using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UI.Views;

namespace UI
{
    public partial class frmMain_old : Form
    {
        public frmMain_old()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnUsersManagment_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void ShowView(UserControl view)
        {
            splitContainer1.Panel2.Controls.Clear();

            view.Dock = DockStyle.Fill;

            splitContainer1.Panel2.Controls.Add(view);
        }

        private void btnUsersManagment_Click_1(object sender, EventArgs e)
        {

        }

       
        private async void btnPersonManagement_Click(object sender, EventArgs e)
        {

            var view = new PersonListView();

            ShowView(view);
        }
    }
}
