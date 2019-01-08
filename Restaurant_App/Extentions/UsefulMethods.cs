using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Restaurant_App.Extentions
{
   public static class UsefulMethods
    {
       
            public static string HashPassword(string password)
            {
                byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(password);
                byte[] hashedArray = new SHA256Managed().ComputeHash(byteArray);
                string hashedPassword = ASCIIEncoding.ASCII.GetString(hashedArray);

                return hashedPassword;
            }

            public static bool CheckPassword(string password, string hashedPassword) => HashPassword(password) == hashedPassword;

            public static bool CheckTextValues(string text, int len)
            {
                if (String.IsNullOrEmpty(text) || text.Length < len)
                {
                    return false;
                }


                return true;
            }

        
    }
}
