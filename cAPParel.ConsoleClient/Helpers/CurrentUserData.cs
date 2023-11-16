using cAPParel.ConsoleClient.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Helpers
{
    public class CurrentUserData
    {
        private static CurrentUserData? _instance;
        private string _token;
        private List<PieceDto> _shoppingCart = new List<PieceDto>();
        private int _choice;
        private CategoryFullDto _category;
        private Color _colorChoice;
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
        public void SetColorChoice(Color color)
        {
            _colorChoice = color;
        }
        public Color GetColorChoice()
        {
            return _colorChoice;
        }
        public void SetCategory(CategoryFullDto category)
        {
            _category = category;
        }
        public CategoryFullDto GetCategory()
        {
            return _category;
        }
        public void SetChoice(int choice)
        {
            _choice = choice;
        }
        public int GetChoice()
        {
            return _choice;
        }

        public void SetToken(string token)
        {
            _token = token;
        }
        public string GetToken()
        {
            return _token;
        }
        public void AddToShoppingCart(PieceDto piece)
        {
            _shoppingCart.Add(piece);
        }
        public void RemoveFromShoppingCart(PieceDto piece)
        {
            _shoppingCart.Remove(piece);
        }
        public void ClearShoppingCart()
        {
            _shoppingCart.Clear();
        }
        public List<PieceDto> GetShoppingCart()
        {
            return _shoppingCart;
        }
    }
}
