using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Repositories.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Booths
{
    public class Booth : IBooth
    {
        public Booth(int boothId, int capacity)
        {
            BoothId = boothId;
            Capacity = capacity;

            this.delicacyMenu = new DelicacyRepository();
            this.cocktailMenu = new CocktailRepository();

            CurrentBill = 0;
            Turnover = 0;
            IsReserved = false;

        }

        private int boothId;

        public int BoothId
        {
            get { return boothId; }
            private set { boothId = value; }
        }

        private int capacity;

        public int Capacity
        {
            get { return capacity; }
            private set 
            { 
                if (capacity <= 0)
                {
                    throw new ArgumentException(ExceptionMessages.CapacityLessThanOne);
                }
                capacity = value;
            }
        }

        private readonly IRepository<IDelicacy> delicacyMenu;
        public IRepository<IDelicacy> DelicacyMenu => this.delicacyMenu;

        private readonly IRepository<ICocktail> cocktailMenu;
        public IRepository<ICocktail> CocktailMenu => this.cocktailMenu;

        public double CurrentBill { get; private set; }

        public double Turnover { get; private set; }
        public bool IsReserved { get; private set; }
        public void ChangeStatus()
        {
            if (IsReserved == false)
            {
                this.IsReserved = true;
            }
            else if (IsReserved == true)
            {
                this.IsReserved = false;
            }
        }

        public void Charge()
        {
            Turnover += CurrentBill;
            CurrentBill = 0;
        }

        public void UpdateCurrentBill(double amount)
        {
            CurrentBill += amount;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Booth: {BoothId}");
            sb.AppendLine($"Capacity: {Capacity}");
            sb.AppendLine($"Turnover: {Turnover:f2} lv");
            sb.AppendLine($"-Cocktail menu:");
            foreach (var item in CocktailMenu.Models)
            {
                sb.AppendLine($"--{item.ToString()}");
            }
            sb.AppendLine($"-Delicacy menu:");
            foreach (var item in DelicacyMenu.Models)
            {
                sb.AppendLine($"--{item.ToString()}");
            }
            return sb.ToString().Trim();
        }
    }
}
