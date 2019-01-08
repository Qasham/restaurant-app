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

namespace Restaurant_App
{
    public partial class CheckForm : Form
    {
        RestaurantEntities db = new RestaurantEntities();
        private int tableID;
        private Waiter currentWaiter;
        public CheckForm(int id, Waiter waiter)
        {
            tableID = id;
            currentWaiter = waiter;
            InitializeComponent();
            
        }

       

        private void CheckForm_Load_1(object sender, EventArgs e)
        {
            
            dgwOrders.DataSource = db.Orders.Where(o => o.TableID == tableID && o.HasPayment == false).Select(s => new
            {
                Meal = s.Meal.Name,
                s.Meal.Price,
                MeaLAmount = s.MealAmount,
                Drink = s.Drink.Name,
                DrinkPrice=s.Drink.Price,
                s.DrinkAmount,      
                TotalCost = (s.DrinkAmount* s.Drink.Price)+ (s.MealAmount * s.Meal.Price)


            }).ToList();
            dgwOrders.Columns[6].Visible = false;
            decimal sum=0;
            for (int i = 0; i < dgwOrders.Rows.Count; ++i)
            {
                sum += Convert.ToDecimal(dgwOrders.Rows[i].Cells[6].Value);



            }
            lblTotalCost.Text = sum.ToString()+ " " + "AZN";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
