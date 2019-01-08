using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurant_App.Extentions;
using Restaurant_App.Model;

namespace Restaurant_App
{
    public partial class LoginForm : Form
    {
        RestaurantEntities db = new RestaurantEntities();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string pass = txtPassword.Text;

            if (rdbAdmin.Checked)
            {
                AdminSetting admin = db.AdminSettings.FirstOrDefault(a => a.Username == username);

                if (admin != null)
                {
                   
                    if (UsefulMethods.CheckPassword(pass, admin.Password))
                    {
                        AdminForm admForm = new AdminForm(admin);
                        admForm.Show();
                        this.Hide();
                        admForm.FormClosed += (sender1, e1) => this.Close();
                    }

                }

                else
                {
                    MessageBox.Show("Username or password is wrong");
                }

            }
            else
            {

                Waiter waiter = db.Waiters.FirstOrDefault(w => w.Username == username && w.Status == true);

                if (waiter != null && UsefulMethods.CheckPassword(pass, waiter.Password))
                {
                    Waiter_Form waiterForm = new Waiter_Form(waiter);
                    waiterForm.Show();
                    this.Hide();
                    waiterForm.FormClosed += (sender2, e2) => this.Close();
                }
                else
                {
                    MessageBox.Show("Username or password is wrong");
                }
            }

        }

     
    }
}
