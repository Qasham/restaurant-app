using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Restaurant_App.Model;
using Restaurant_App.Extentions;

namespace Restaurant_App
{
    public partial class AdminForm : Form
    {
        RestaurantEntities db = new RestaurantEntities();
        Waiter waiter = new Waiter();
        private AdminSetting loggedAdmin;

        public AdminForm(AdminSetting admin)
        {
            loggedAdmin = admin;
            InitializeComponent();

        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            lblLoggedAdmin.Text = loggedAdmin.Username;
            RefreshWaiterDataGridView();
            RefreshMealDataGridView();
            UpdateMealCategoryCombobox();
            RefreshDrinkCategoriesDataGridView();
            RefreshMealCategoriesDataGridView();
            RefreshDrinkDataGridView();
           
            UpdateDrinkCategoryCombobox();
            ClearTextValues();
        }

        #region Meal

        void UpdateMealCategoryCombobox()
        {

            cmbMeal.Items.Clear();
            foreach (var item in db.MealCategories)
            {
                CategoryCombobox comboMeal;
                comboMeal = new CategoryCombobox
                {
                    Text = item.Category,
                    Value = item.ID


                };

                cmbMeal.Items.Add(comboMeal);
                cmbMeal.SelectedIndex = 0;

            }
        }

        void RefreshMealDataGridView()
        {

            dgwMeals.DataSource = db.Meals.Select(m => new
            {
                m.ID,
                m.Name,
                m.MealCategory.Category,
                m.Price,
                m.PreparingTime,
                Status = m.Status == false ? "Not available" : "Available"


            }).ToList();


            dgwMeals.Columns[0].Visible = false;

        }

        private void txtMealPreTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void dgwMeals_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgwMeals.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Available")
                {
                    row.Cells["Status"].Style.BackColor = Color.LimeGreen;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
                else
                {

                    row.Cells["Status"].Style.BackColor = Color.Crimson;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgwMeals_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnMealEdit.Enabled = true;
            btnMealRemove.Enabled = true;

            lblMealID.Text = dgwMeals.Rows[e.RowIndex].Cells[0].Value.ToString();
            int MealID = Int32.Parse(lblMealID.Text);
            Meal selectedMeal = db.Meals.Find(MealID);
            txtmealName.Text = selectedMeal.Name;
            txtMealPreTime.Text = selectedMeal.PreparingTime.ToString();
            numMealPrice.Value = (decimal)selectedMeal.Price;
            cmbMeal.Text = selectedMeal.MealCategory.Category;

        }
        void ClearTextValues()
        {
            txtmealName.Clear();
            txtMealPreTime.Clear();
            numMealPrice.Value = 0;
            numDrinkPrice.Value = 0;
            txtDrinkName.Clear();
            txtFirstname.Clear();
            txtLastname.Clear();
            txtUsername.Clear();
            txtPassword.Clear();

        }
        private void btnMealAdd_Click(object sender, EventArgs e)
        {

            if (UsefulMethods.CheckTextValues(txtmealName.Text, 2) && UsefulMethods.CheckTextValues(txtMealPreTime.Text, 1))
            {
                string mealName = txtmealName.Text;
                decimal mealPrice = numMealPrice.Value;
                int mealPreTime = Int32.Parse(txtMealPreTime.Text);

                if (db.Meals.Count(p => p.Name == mealName) == 0)
                {

                    db.Meals.Add(new Meal
                    {

                        Name = mealName,
                        CategoryID = db.MealCategories.FirstOrDefault(m => m.Category == cmbMeal.Text).ID,
                        Price = mealPrice,
                        Status = true,
                        PreparingTime = mealPreTime


                    });
                    db.SaveChanges();

                    MessageBox.Show($"New Meal - {mealName} - was added successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateMealCategoryCombobox();
                    RefreshMealDataGridView();
                    ClearTextValues();


                }
                else
                {
                    MessageBox.Show("This meal already exists");
                }
            }
            else
            {
                MessageBox.Show("Input values are not valid");
            }
        }

        private void btnMealEdit_Click(object sender, EventArgs e)
        {
            Meal editedMeal = db.Meals.Find(Int32.Parse(lblMealID.Text));
            if (db.Meals.FirstOrDefault(p => p.ID == editedMeal.ID).Status == false)
            {
                MessageBox.Show("This meal removed, so you can't edited this meal!");
            }

            else if (db.Meals.Count(p => p.Name == txtmealName.Text && p.ID != editedMeal.ID) == 1)
            {
                MessageBox.Show("This meal already exists!");
            }
            else
            {
                if (!String.IsNullOrEmpty(txtmealName.Text) && !String.IsNullOrEmpty(txtMealPreTime.Text))
                {
                    editedMeal.Name = txtmealName.Text;
                    editedMeal.PreparingTime = Int32.Parse(txtMealPreTime.Text);
                    editedMeal.Price = numMealPrice.Value;
                    editedMeal.CategoryID = db.MealCategories.FirstOrDefault(m => m.Category == cmbMeal.Text).ID;

                    db.SaveChanges();
                    RefreshMealDataGridView();

                    UpdateMealCategoryCombobox();
                    btnMealEdit.Enabled = false;
                    btnMealRemove.Enabled = false;
                    ClearTextValues();
                    MessageBox.Show("Meal was edited successfully");
                }
                else
                {
                    MessageBox.Show("Input values are not valid");
                }



            }
        }

        private void btnMealRemove_Click(object sender, EventArgs e)
        {
            Meal removedMeal = db.Meals.Find(Int32.Parse(lblMealID.Text));
            removedMeal.Status = false;
            db.SaveChanges();
            RefreshMealDataGridView();

            UpdateMealCategoryCombobox();
            btnMealEdit.Enabled = false;
            btnMealRemove.Enabled = false;
            ClearTextValues();
            MessageBox.Show("Meal was removed successfully");
        }

        #endregion

        #region Meal Categories



        void RefreshMealCategoriesDataGridView()
        {

            dgwMealCategories.DataSource = db.MealCategories.Select(m => new
            {
                m.ID,
                m.Category,
                Status = m.Status == false ? "Not available" : "Available"


            }).ToList();


            dgwMealCategories.Columns[0].Visible = false;

        }
        private void dgwMealCategories_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgwMealCategories.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Available")
                {
                    row.Cells["Status"].Style.BackColor = Color.LimeGreen;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
                else
                {

                    row.Cells["Status"].Style.BackColor = Color.Crimson;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgwMealCategories_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnMealCateEdit.Enabled = true;
            btnMealCateRemove.Enabled = true;

            lblmealCategoryID.Text = dgwMealCategories.Rows[e.RowIndex].Cells[0].Value.ToString();
            int mealCateID = Int32.Parse(lblmealCategoryID.Text);
            MealCategory selectedMealCate = db.MealCategories.Find(mealCateID);
            txtMealCategoryName.Text = selectedMealCate.Category;

        }


        private void btnMealCateAdd_Click(object sender, EventArgs e)
        {
            if (UsefulMethods.CheckTextValues(txtMealCategoryName.Text, 2))
            {
                string mealCateName = txtMealCategoryName.Text;


                if (db.MealCategories.Count(p => p.Category == mealCateName) == 0)
                {

                    db.MealCategories.Add(new MealCategory
                    {

                        Category = mealCateName,
                        Status = true,



                    });
                    db.SaveChanges();

                    MessageBox.Show($"New Meal Category - {mealCateName} - was added successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateMealCategoryCombobox();
                    RefreshMealDataGridView();
                    txtMealCategoryName.Clear();
                    btnMealCateEdit.Enabled = false;
                    btnMealCateRemove.Enabled = false;



                }
                else
                {
                    MessageBox.Show("This Meal Category already exists");
                }
            }
            else
            {
                MessageBox.Show("Input values are not valid");
            }
        }


        private void btnMealCateEdit_Click(object sender, EventArgs e)
        {

            MealCategory editedMealCate = db.MealCategories.Find(Int32.Parse(lblmealCategoryID.Text));
            if (db.MealCategories.FirstOrDefault(p => p.ID == editedMealCate.ID).Status == false)
            {
                MessageBox.Show("This meal category removed, so you can't edited this meal!");
            }

            else if (db.MealCategories.Count(p => p.Category == txtMealCategoryName.Text && p.ID != editedMealCate.ID) == 1)
            {
                MessageBox.Show("This meal category already exists!");
            }
            else
            {
                if (!String.IsNullOrEmpty(txtMealCategoryName.Text))
                {
                    editedMealCate.Category = txtMealCategoryName.Text;


                    db.SaveChanges();
                    RefreshMealDataGridView();

                    UpdateMealCategoryCombobox();
                    RefreshMealCategoriesDataGridView();
                    btnMealCateEdit.Enabled = false;
                    btnMealCateRemove.Enabled = false;
                    txtMealCategoryName.Clear();
                    MessageBox.Show("Meal Category was edited successfully");
                }
                else
                {
                    MessageBox.Show("Input values are not valid");
                }



            }

        }

        private void btnMealCateRemove_Click(object sender, EventArgs e)
        {
            MealCategory removedMealCate = db.MealCategories.Find(Int32.Parse(lblmealCategoryID.Text));
            removedMealCate.Status = false;
            db.SaveChanges();
            RefreshMealDataGridView();

            UpdateMealCategoryCombobox();
            RefreshMealCategoriesDataGridView();
            btnMealCateEdit.Enabled = false;
            btnMealCateRemove.Enabled = false;
            ClearTextValues();
            MessageBox.Show("Meal Category was removed successfully");
        }


        #endregion

        #region Drink
        void UpdateDrinkCategoryCombobox()
        {
            cmbDrink.Items.Clear();

            foreach (var item in db.DrinkCategories)
            {
                CategoryCombobox comboDrink;
                comboDrink = new CategoryCombobox
                {
                    Text = item.Category,
                    Value = item.ID


                };

                cmbDrink.Items.Add(comboDrink);
                cmbDrink.SelectedIndex = 0;

            }
        }
        void RefreshDrinkDataGridView()
        {

            dgwDrink.DataSource = db.Drinks.Select(m => new
            {
                m.ID,
                m.Name,
                m.DrinkCategory.Category,
                m.Price,
                Status = m.Status == false ? "Not available" : "Available"


            }).ToList();


            dgwDrink.Columns[0].Visible = false;

        }


        private void dgwDrink_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgwDrink.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Available")
                {
                    row.Cells["Status"].Style.BackColor = Color.LimeGreen;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
                else
                {

                    row.Cells["Status"].Style.BackColor = Color.Crimson;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgwDrink_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnDrinkEdit.Enabled = true;
            btnDrinkRemove.Enabled = true;

            lblDrinkID.Text = dgwDrink.Rows[e.RowIndex].Cells[0].Value.ToString();
            int drinkID = Int32.Parse(lblDrinkID.Text);
            Drink selectedDrink = db.Drinks.Find(drinkID);
            txtDrinkName.Text = selectedDrink.Name;
            numMealPrice.Value = (decimal)selectedDrink.Price;
            cmbDrink.Text = selectedDrink.DrinkCategory.Category;
        }

        private void btnDrinkAdd_Click(object sender, EventArgs e)
        {
            if (UsefulMethods.CheckTextValues(txtDrinkName.Text, 2))
            {
                string drinkName = txtDrinkName.Text;
                decimal drinkPrice = numDrinkPrice.Value;

                if (db.Drinks.Count(p => p.Name == drinkName) == 0)
                {

                    db.Drinks.Add(new Drink
                    {

                        Name = drinkName,
                        CategoryID = db.DrinkCategories.FirstOrDefault(m => m.Category == cmbDrink.Text).ID,
                        Price = drinkPrice,
                        Status = true,


                    });
                    db.SaveChanges();

                    MessageBox.Show($"New Drink - {drinkName} - was added successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateMealCategoryCombobox();
                    RefreshMealDataGridView();
                    RefreshDrinkDataGridView();
                    UpdateDrinkCategoryCombobox();
                    ClearTextValues();


                }
                else
                {
                    MessageBox.Show("This drink already exists");
                }
            }
            else
            {
                MessageBox.Show("Input values are not valid");
            }
        }

        private void btnDrinkEdit_Click(object sender, EventArgs e)
        {
            Drink editedDrink = db.Drinks.Find(Int32.Parse(lblDrinkID.Text));
            if (db.Drinks.FirstOrDefault(p => p.ID == editedDrink.ID).Status == false)
            {
                MessageBox.Show("You can't edited removed  drink!");
            }

            else if (db.Drinks.Count(p => p.Name == txtDrinkName.Text && p.ID != editedDrink.ID) == 1)
            {
                MessageBox.Show("This Drink already exists!");
            }
            else
            {
                if (!String.IsNullOrEmpty(txtDrinkName.Text))
                {
                    editedDrink.Name = txtDrinkName.Text;
                    editedDrink.Price = numDrinkPrice.Value;
                    editedDrink.CategoryID = db.DrinkCategories.FirstOrDefault(m => m.Category == cmbDrink.Text).ID;

                    db.SaveChanges();
                    RefreshMealDataGridView();
                    ClearTextValues();
                    UpdateMealCategoryCombobox();
                    RefreshDrinkDataGridView();
                    UpdateDrinkCategoryCombobox();
                    btnDrinkEdit.Enabled = false;
                    btnDrinkRemove.Enabled = false;
                    MessageBox.Show("Drink was edited successfully");
                }
                else
                {
                    MessageBox.Show("Input values are not valid");
                }



            }
        }

        private void btnDrinkRemove_Click(object sender, EventArgs e)
        {
            Drink removedDrink = db.Drinks.Find(Int32.Parse(lblDrinkID.Text));
            removedDrink.Status = false;
            db.SaveChanges();
            UpdateMealCategoryCombobox();
            RefreshMealDataGridView();
            ClearTextValues();
            RefreshDrinkDataGridView();
            UpdateDrinkCategoryCombobox();
            btnDrinkEdit.Enabled = false;
            btnDrinkRemove.Enabled = false;
            MessageBox.Show("Drink was removed successfully");
        }





        #endregion

        #region Drink Categories

        void RefreshDrinkCategoriesDataGridView()
        {

            dgwDrinkCate.DataSource = db.DrinkCategories.Select(m => new
            {
                m.ID,
                m.Category,
                Status = m.Status == false ? "Not available" : "Available"


            }).ToList();


            dgwDrinkCate.Columns[0].Visible = false;

        }

        private void dgwDrinkCate_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgwDrinkCate.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Available")
                {
                    row.Cells["Status"].Style.BackColor = Color.LimeGreen;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
                else
                {

                    row.Cells["Status"].Style.BackColor = Color.Crimson;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgwDrinkCate_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnDrinkCateEdit.Enabled = true;
            btnDrinkCateRemove.Enabled = true;

            lblDrinkCateID.Text = dgwDrinkCate.Rows[e.RowIndex].Cells[0].Value.ToString();
            int drinkCateID = Int32.Parse(lblDrinkCateID.Text);
            DrinkCategory selectedDrinkCate = db.DrinkCategories.Find(drinkCateID);
            txtDrinkCateName.Text = selectedDrinkCate.Category;
        }

        private void btnDrinkCateAdd_Click(object sender, EventArgs e)
        {
            if (UsefulMethods.CheckTextValues(txtDrinkCateName.Text, 2))
            {
                string drinkCateName = txtDrinkCateName.Text;


                if (db.DrinkCategories.Count(p => p.Category == drinkCateName) == 0)
                {

                    db.DrinkCategories.Add(new DrinkCategory
                    {

                        Category = drinkCateName,
                        Status = true,



                    });


                    MessageBox.Show($"New Drink Category - {drinkCateName} - was added successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    db.SaveChanges();
                    RefreshDrinkDataGridView();
                    RefreshMealCategoriesDataGridView();
                    UpdateMealCategoryCombobox();
                    RefreshMealDataGridView();
                    UpdateDrinkCategoryCombobox();
                    RefreshDrinkCategoriesDataGridView();
                    btnDrinkCateEdit.Enabled = false;
                    btnDrinkCateRemove.Enabled = false;
                    txtDrinkCateName.Clear();





                }
                else
                {
                    MessageBox.Show("This Drink Category already exists");
                }
            }
            else
            {
                MessageBox.Show("Input values are not valid");
            }
        }

        private void btnDrinkCateEdit_Click(object sender, EventArgs e)
        {
            DrinkCategory editedDrinkCate = db.DrinkCategories.Find(Int32.Parse(lblDrinkCateID.Text));
            if (db.DrinkCategories.FirstOrDefault(p => p.ID == editedDrinkCate.ID).Status == false)
            {
                MessageBox.Show("You can't edited removed drink!");
            }

            else if (db.DrinkCategories.Count(p => p.Category == txtDrinkCateName.Text && p.ID != editedDrinkCate.ID) == 1)
            {
                MessageBox.Show("This Drink category already exists!");
            }
            else
            {
                if (!String.IsNullOrEmpty(txtDrinkCateName.Text))
                {
                    editedDrinkCate.Category = txtDrinkCateName.Text;



                    MessageBox.Show("Drink Category was edited successfully");
                    db.SaveChanges();
                    RefreshMealDataGridView();
                    RefreshMealCategoriesDataGridView();
                    UpdateMealCategoryCombobox();
                    RefreshDrinkDataGridView();
                    UpdateDrinkCategoryCombobox();
                    RefreshDrinkCategoriesDataGridView();
                    btnDrinkCateEdit.Enabled = false;
                    btnDrinkCateRemove.Enabled = false;
                    txtDrinkCateName.Clear();
                }
                else
                {
                    MessageBox.Show("Input values are not valid");
                }



            }
        }

        private void btnDrinkCateRemove_Click(object sender, EventArgs e)
        {
            DrinkCategory removedDrinkCate = db.DrinkCategories.Find(Int32.Parse(lblDrinkCateID.Text));
            removedDrinkCate.Status = false;

            MessageBox.Show("Drink Category was removed successfully");
            db.SaveChanges();
            RefreshDrinkDataGridView();
            RefreshMealCategoriesDataGridView();
            UpdateMealCategoryCombobox();
            RefreshMealDataGridView();
            UpdateDrinkCategoryCombobox();
            RefreshDrinkCategoriesDataGridView();
            btnDrinkCateEdit.Enabled = false;
            btnDrinkCateRemove.Enabled = false;
            txtDrinkCateName.Clear();
        }


        #endregion

        #region Waiter
        void RefreshWaiterDataGridView()
        {

            dgwWaiter.DataSource = db.Waiters.Select(m => new
            {
                m.ID,
                m.Firstname,
                m.Lastname,
                m.Username,
                Status = m.Status == false ? "Passive" : "Active"


            }).ToList();


            dgwWaiter.Columns[0].Visible = false;

        }
        private void dgwWaiter_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgwWaiter.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == "Active")
                {
                    row.Cells["Status"].Style.BackColor = Color.LimeGreen;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
                else
                {

                    row.Cells["Status"].Style.BackColor = Color.Crimson;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                }
            }
        }

        private void dgwWaiter_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnWaiterEdit.Enabled = true;
            btnWaiterRemove.Enabled = true;

            lblWaiterID.Text = dgwWaiter.Rows[e.RowIndex].Cells[0].Value.ToString();
            int waiterID = Int32.Parse(lblWaiterID.Text);
            Waiter selectedWaiter = db.Waiters.Find(waiterID);
            txtFirstname.Text = selectedWaiter.Firstname;
            txtLastname.Text = selectedWaiter.Lastname;
            txtUsername.Text = selectedWaiter.Username;
            txtPassword.Text = selectedWaiter.Password;
        }


        private void btnWaiterAdd_Click(object sender, EventArgs e)
        {
            if (UsefulMethods.CheckTextValues(txtFirstname.Text, 3)
                && UsefulMethods.CheckTextValues(txtLastname.Text, 3)
                && UsefulMethods.CheckTextValues(txtUsername.Text, 3)&&
                UsefulMethods.CheckTextValues(txtPassword.Text, 8))
            {
                string firstname = txtFirstname.Text;
                string lastname = txtLastname.Text;
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (db.Waiters.Count(s => s.Username == username) == 0)
                {
                    db.Waiters.Add(new Waiter
                    {
                       
                        Firstname = firstname,
                        Lastname  = lastname,
                        Username = username,
                        Password = UsefulMethods.HashPassword(password),
                        Status = true
                    });
                   
                    MessageBox.Show($"New waiter - {firstname} {lastname} - was added successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    db.SaveChanges();
                    RefreshWaiterDataGridView();
                    ClearTextValues();
                }
                else
                {
                    MessageBox.Show("Waiter with this username already exists");
                }
            }
            else
            {
                MessageBox.Show("Password length must be minimum 8, other input minimum 3 characters");
            }
        }

        private void btnWaiterEdit_Click(object sender, EventArgs e)
        {
            int waiterID = Convert.ToInt32(lblWaiterID.Text);
            Waiter editedWaiter = db.Waiters.Find(waiterID);

            string firstname = txtFirstname.Text;
            string lastname = txtLastname.Text;
            string username = txtUsername.Text;
            string pass = txtPassword.Text;

            if (UsefulMethods.CheckTextValues(firstname, 3) &&
                UsefulMethods.CheckTextValues(lastname, 3) &&
                UsefulMethods.CheckTextValues(username, 3)
                )
            {
                if (String.IsNullOrEmpty(pass))
                {
                    pass = editedWaiter.Password;
                }
                else
                {
                    if (UsefulMethods.CheckTextValues(pass, 8))
                    {
                        pass = UsefulMethods.HashPassword(pass);
                    }
                    else
                    {
                        MessageBox.Show("Password is not valid.");
                        return;
                    }
                }

              
                if (db.Waiters.FirstOrDefault(w => w.ID == editedWaiter.ID).Status == false)
                {
                    MessageBox.Show("You can't edit removed Waiter!");
                }
                else if (db.Waiters.Count(w => w.Username == username && w.ID != waiterID) == 1)
                {
                    MessageBox.Show("This username already exists for somebody else.");
                }
                else
                {
                    editedWaiter.Firstname = firstname;
                    editedWaiter.Lastname = lastname;
                    editedWaiter.Username = username;
                    editedWaiter.Password = pass;

                    db.SaveChanges();
                    MessageBox.Show("Waiter edited Successfully");
                    RefreshWaiterDataGridView();
                    ClearTextValues();
                    btnWaiterEdit.Enabled = false;
                    btnWaiterRemove.Enabled = false;

                }
            }
        }

        private void btnWaiterRemove_Click(object sender, EventArgs e)
        {
            int waiterID = Convert.ToInt32(lblWaiterID.Text);
            Waiter removedWaiter = db.Waiters.Find(waiterID);

            removedWaiter.Status = false;

            db.SaveChanges();
            MessageBox.Show("Waiter removed Successfully");
            RefreshWaiterDataGridView();
            ClearTextValues();       
            btnWaiterEdit.Enabled = false;
            btnWaiterRemove.Enabled = false;
        }


        private void btnSignOut_Click(object sender, EventArgs e)
        {
            LoginForm log = new LoginForm();
            log.Show();
            this.Hide();
            log.FormClosed += (sender2, e2) => this.Close();
        }





        #endregion
      
        
    }
}

















