using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Zadanie3
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length>1 && new ValidatorGame(args).Validate())
            {
                Game game = new Game(args);
                game.Move();
            }
            else
            {
                Console.WriteLine("Invalid run. Count of words need to be >=3 && odd");
            }
        }
    }

    class Game
    {
        private readonly List<string> menuItems;
        private readonly Random indexPC = new Random();
        private readonly string exitString = "0";
        private readonly int invalidId = -1;
        public Game(string[] args)
        {
            this.menuItems = args.ToList();
        }

        public void Move()
        {
            HmacS256 hmac = new HmacS256(16);
            var idPC = indexPC.Next(0, menuItems.Count);
            var wordPC = menuItems[idPC];
            hmac.PrintHash(wordPC);
            PrintMenu();
            string choose = Console.ReadLine();
            var id = GetFigureId(choose);
            if (id >= 0)
            {
                Console.WriteLine("Your move:" + menuItems[id]);
                Console.WriteLine("Computer move:" + menuItems[idPC]);
                ChooseWinner(id, idPC);
                hmac.PrintKey();
            }
            else
            {
                Console.WriteLine("Invalid input");
                Move();
            }
        }

        private void ChooseWinner(int idPlayer, int idPC)
        {
            int hight = idPC + (menuItems.Count - 1) / 2 >= menuItems.Count ? menuItems.Count - 1 : idPC + (menuItems.Count - 1) / 2;
            int down = idPC + (menuItems.Count - 1) / 2 >= menuItems.Count ? (idPC + (menuItems.Count - 1) / 2) % menuItems.Count : -1;
            if (idPlayer == idPC) Console.WriteLine("No one win :(");
            else if ((idPlayer > idPC && idPlayer <= hight) || (idPlayer <= down)) Console.WriteLine("You win!");
            else Console.WriteLine("You lose!");
        }
         
        private int GetFigureId(string id)
        {
            if (id == exitString) { Environment.Exit(0);}
            int idWord = 0;
            if (int.TryParse(id, out idWord) && idWord <= menuItems.Count)
            {
                return idWord - 1;
            }

            return invalidId;
        }

        private void PrintMenu()
        {
            Console.WriteLine("Available moves:");
            foreach (var i in menuItems)
            {
                Console.WriteLine(menuItems.IndexOf(i) + 1 + " - " + i);
            }
            Console.WriteLine("0 - exit");
            Console.WriteLine("Enter your move:");
        }
    }

    class HmacS256
    {
        private readonly HMACSHA256 _hmac;
        public HmacS256(int keySize)
        {
            _hmac = new HMACSHA256(GenerateKey(new byte[keySize]));
        }

        public void PrintHash(string message)
        {
            Console.WriteLine("HMAC: \n " + BitConverter.ToString(this._hmac.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", string.Empty));
        }

        public void PrintKey()
        {
            Console.WriteLine("HMAC key: " + BitConverter.ToString(this._hmac.Key).Replace("-", string.Empty));
        }

        private byte[] GenerateKey(byte[] bytes)
        {
            RandomNumberGenerator.Create().GetBytes(bytes);
            return bytes;
        }
    }
    class ValidatorGame
    {
        private bool isValid = true;
        private readonly List<string> words;

        public ValidatorGame(string[] args)
        {
            words = args.ToList();
        }

        private void CheckUnique()
        {
            if (words.Count != words.Distinct().ToList().Count)
            {
                isValid = false;
                Console.WriteLine("All words needs to be unique! example: rock paper scissors lizard Spock");
            }
        }

        private void CheckOdd()
        {
            if (words.Count % 2 == 0)
            {
                isValid = false;
                Console.WriteLine("Count of words need to be odd! (>=3 mod 2 == 1)");
            }
        }

        public bool Validate()
        {
            this.CheckUnique();
            this.CheckOdd();
            return isValid;
        }
    }
}
