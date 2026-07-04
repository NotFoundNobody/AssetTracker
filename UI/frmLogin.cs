using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UI
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {

                this.Invoke(() =>
                {
                    errorProvider1.SetError(btnLogin,"");
                });
                

                var IAuthenticationService = Program.ServiceProvider.GetRequiredService<IAuthenticationService>();
                var logindata = new LoginRequest { Username=txtUsername.Text,Password=txtPassword.Text};
               var result=await IAuthenticationService.Login(logindata);
                if (result==null)
                {

                    this.Invoke(() =>
                    {
                        errorProvider1.SetError(btnLogin,"Login Error");
                    });
                    return;
                }
                this.Invoke(() =>
                {
                    MessageBox.Show(result.Person.FirstName+" "+ result.Person.LastName, "Welcome");
                });

            });
        }
    }
}
