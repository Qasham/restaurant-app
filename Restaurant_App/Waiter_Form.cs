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
    public partial class Waiter_Form : Form
    {
        RestaurantEntities db = new RestaurantEntities();
        private Waiter LoggedWaiter;

        public Waiter_Form(Waiter waiter)
        {
            InitializeComponent();
            LoggedWaiter = waiter;
        }
        void UpdateDataGridView()
        {
            dgwOrders.DataSource = null;
            if (!String.IsNullOrEmpty(lblpicTag.Text))
            {
                int selectedTableid = Int32.Parse(lblpicTag.Text);

                dgwOrders.DataSource = db.Orders.Where(or => or.HasPayment == false && or.TableID == selectedTableid).Select(m => new
                {
                    Meal = m.Meal.Name,
                    MealPrice = m.Meal.Price,
                    m.MealAmount,
                    Drink = m.Drink.Name,
                    m.DrinkAmount,
                    DrinkPrice =m.Drink.Price,
                    m.OrderDate,
                   



                }).ToList();
            }
           
          

        }

        void UpdateCategoryCombobox()
        {
            foreach (var item in db.MealCategories)
            {
                CategoryCombobox comboMeal = new CategoryCombobox
                {
                    Text = item.Category,
                    Value = item.ID


                };

                cmbMealCategory.Items.Add(comboMeal);
                cmbMealCategory.SelectedIndex = 0;

            }


            foreach (var item in db.DrinkCategories)
            {
                CategoryCombobox comboDrink = new CategoryCombobox
                {
                    Text = item.Category,
                    Value = item.ID


                };
                cmbDrinkCategory.Items.Add(comboDrink);
                cmbDrinkCategory.SelectedIndex = 0;

            }

        }
        void UpdateTableStatus()
        {
          
            foreach (var item in pnlTables.Controls)
            {
                if (item is Panel)
                {
                    Panel pnlStatus = (Panel)item;
                    int id = Convert.ToInt32(pnlStatus.Tag.ToString());
                    

                    if ((db.Tables.Find(id).HasOrder == true))
                    {
                     pnlStatus.BackColor = Color.Crimson;

                        foreach (var items in pnlStatus.Controls)
                        {
                            Label pnlLabel = (Label)items;
                            pnlLabel.Text = "Reserved";
                        }

                    }
                    else 
                    {
                          pnlStatus.BackColor = Color.LimeGreen;



                        foreach (var items in pnlStatus.Controls)
                        {
                            Label pnlLabel2 = (Label)items;
                            pnlLabel2.Text = "Free";

                        }

                    }
                }

            }

        }
        private void Waiter_Form_Load(object sender, EventArgs e)
        {
            lblWaiterUsername.Text = LoggedWaiter.Username;
            lblpicTag.Text = "";
            UpdateCategoryCombobox();
            UpdateTableStatus();
            UpdateDataGridView();

        }

        private void cmbMealCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

            int cateID = ((CategoryCombobox)cmbMealCategory.SelectedItem).Value;
            cmbMeal.DataSource = db.Meals.Where(m => m.CategoryID == cateID).Select(c => c.Name).ToList();
           
            
        }

        private void cmbDrinkCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cateID = ((CategoryCombobox)cmbDrinkCategory.SelectedItem).Value;
            cmbDrink.DataSource = db.Drinks.Where(d => d.CategoryID == cateID).Select(c => c.Name).ToList();

           
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string meal = cmbMeal.Text;
            string mealCategory = cmbMealCategory.Text;
            int mealAmount = (int)numMealAmount.Value;
            string drink = cmbDrink.Text;
            string drinkCategory = cmbDrinkCategory.Text;
            int drinkAmount = (int)numDrinkAmount.Value;


            Drink selectedDrink = db.Drinks.FirstOrDefault(d => d.Name == drink);
            Meal selectedMeal = db.Meals.FirstOrDefault(m => m.Name == meal);

            db.Orders.Add(new Order
            {
                MealID = selectedMeal.ID,
                MealAmount = mealAmount,
                DrinkID = selectedDrink.ID,
                DrinkAmount = drinkAmount,
                OrderDate = DateTime.Now,
                WaiterID = LoggedWaiter.ID,
                TableID = Int32.Parse(lblpicTag.Text),
                HasPayment = false

            });
            db.Tables.Find(Int32.Parse(lblpicTag.Text)).HasOrder = true;
            db.SaveChanges();
            UpdateTableStatus();
            UpdateDataGridView();
            btnAdd.Enabled = false;
            pnlOrder.Enabled = false;

        }

        private void btnTakePayment_Click(object sender, EventArgs e)
        {
            int selectedTableid = Int32.Parse(lblpicTag.Text);

            DialogResult answer = MessageBox.Show("Are you sure want to close this table?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer == DialogResult.Yes)
            {
                CheckForm chForm = new CheckForm(selectedTableid, LoggedWaiter);

                chForm.FormClosed += ChForm_FormClosed;
                chForm.ShowDialog();
            }



        }

        void refreshValues()
        {
            cmbDrink.SelectedIndex = 0;
            cmbDrinkCategory.SelectedIndex = 0;
            cmbMeal.SelectedIndex = 0;
            cmbMealCategory.SelectedIndex = 0;
            numMealAmount.Value = 1;
            numDrinkAmount.Value = 1;
        }

        private void ChForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            int selectedTableid = Int32.Parse(lblpicTag.Text);
            foreach (var item in db.Orders.Where(s => s.HasPayment == false && s.TableID == selectedTableid))
            {
                item.HasPayment = true;
                db.Tables.Find(selectedTableid).HasOrder = false;
            }

            db.SaveChanges();
            UpdateTableStatus();
            UpdateDataGridView();
            refreshValues();
            btnTakePayment.Enabled = false;


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pnlOrder.Enabled = true;
            PictureBox pic = (PictureBox)sender;
            lblpicTag.Text = pic.Tag.ToString();
            int id = Int32.Parse(lblpicTag.Text);

            
            foreach (var item in db.Orders.Where(s=>s.TableID == id))
            {
                if (item.HasPayment==false)
                {
                    btnTakePayment.Enabled = true;
                }
                else
                {
                    btnTakePayment.Enabled = false;
                }

            }
            
           
            btnAdd.Enabled = true;
            txtTableName.Text = "Table" + " " + lblpicTag.Text;

            UpdateDataGridView();
        }

       

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            LoginForm log = new LoginForm();
            log.Show();
            this.Hide();
            log.FormClosed += (sender2, e2) => this.Close();
        }
    }
}
