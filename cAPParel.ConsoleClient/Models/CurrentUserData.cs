using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Models
{
    public class CurrentUserData
    {
        private static CurrentUserData? _instance;
        private string _token;
        private List<int> _shoppingCart;
        private CurrentUserData() { }
        public static CurrentUserData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrentUserData();
                }
                return _instance;
            }
        }
        
        public void SetToken(string token)
        {
            _token = token;
        }
        public string GetToken()
        {
            return _token;
        }
        public void AddToShoppingCart(int productId)
        {
            _shoppingCart.Add(productId);
        }
        public void RemoveFromShoppingCart(int productId)
        {
            _shoppingCart.Remove(productId);
        }
        public void ClearShoppingCart()
        {
            _shoppingCart.Clear();
        }
    }
}
