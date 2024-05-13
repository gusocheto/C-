using ChristmasPastryShop.Core.Contracts;
using ChristmasPastryShop.Models.Booths;
using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Repositories.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

namespace ChristmasPastryShop.Core
{
    public class Controller : IController
    {
        private IRepository<IBooth> booths;

        public Controller()
        {
            booths = new BoothRepository();
        }
        public string AddBooth(int capacity)
        {
            int bhootId = booths.Models.Count + 1;

            Booth booth = new Booth(bhootId ,capacity);
            booths.AddModel(booth);

            return String.Format(OutputMessages.NewBoothAdded, bhootId, booth.Capacity);
        }

        public string AddCocktail(int boothId, string cocktailTypeName, string cocktailName, string size)
        {
            if (cocktailTypeName != "Hibernation" && cocktailTypeName != "MulledWine")
            {
                return String.Format(OutputMessages.InvalidCocktailType, cocktailTypeName);
            }
            if (size != "Small" && size != "Middle" && size != "Large")
            {
                return String.Format(OutputMessages.InvalidCocktailSize, size);
            }
            if (booths.Models.Any(c => c.CocktailMenu.Models.Any(x => x.Name == cocktailName)))
            {
                return String.Format(OutputMessages.CocktailAlreadyAdded, size, cocktailName);
            }
            ICocktail cocktail = null;
            if (cocktailTypeName == "Hibernation")
            {
                cocktail = new Hibernation(cocktailName, size);
            }
            else if (cocktailTypeName == "MulledWine")
            {
                cocktail = new MulledWine(cocktailName, size);
            }
            IBooth booth = booths.Models.FirstOrDefault(x => x.BoothId == boothId);
            booth.CocktailMenu.AddModel(cocktail);
            return String.Format(OutputMessages.NewCocktailAdded, size, cocktailName, cocktailTypeName);
        }

        public string AddDelicacy(int boothId, string delicacyTypeName, string delicacyName)
        {
            if (delicacyTypeName != "Gingerbread" && delicacyTypeName != "Stolen")
            {
                return String.Format(OutputMessages.InvalidDelicacyType, delicacyTypeName);
            }
            if (this.booths.Models.Any(b => b.DelicacyMenu.Models.Any(dm => dm.Name == delicacyName)))
            {
                return string
                    .Format(OutputMessages.DelicacyAlreadyAdded, delicacyName);
            }
            IDelicacy delicacy = null;
            if (delicacyTypeName == "Gingerbread")
            {
                delicacy = new Gingerbread(delicacyName);
            }
            else if (delicacyTypeName == "Stolen")
            {
                 delicacy = new Stolen(delicacyName);
            }
            IBooth booth = this.booths.Models.FirstOrDefault(x => x.BoothId == boothId);
            booth.DelicacyMenu.AddModel(delicacy);
            return String.Format(OutputMessages.NewDelicacyAdded,delicacyTypeName, delicacyName);


        }

        public string BoothReport(int boothId)
        {
            return booths.Models.FirstOrDefault(b => b.BoothId == boothId).ToString();
        }

        public string LeaveBooth(int boothId)
        {
            IBooth booth = this.booths.Models.FirstOrDefault(b => b.BoothId == boothId);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Bill {booth.CurrentBill:f2} lv");
            booth.Charge();
            booth.ChangeStatus();
            sb.AppendLine($"Booth {boothId} is now available!");
            return sb.ToString().Trim();

        }

        public string ReserveBooth(int countOfPeople)
        {
            IBooth booth = booths.Models.Where(x => x.IsReserved == false && x.Capacity >= countOfPeople).OrderBy(b => b.Capacity)
                .ThenByDescending(x => x.BoothId).FirstOrDefault();

            if (booth == null)
            {
                return String.Format(OutputMessages.NoAvailableBooth, countOfPeople);
            }
            else
            {
                booth.ChangeStatus();
                return String.Format(OutputMessages.BoothReservedSuccessfully, booth.BoothId, countOfPeople);
            }
        }

        public string TryOrder(int boothId, string order)
        {
            IBooth booth = this.booths.Models.FirstOrDefault(b => b.BoothId == boothId);

            bool isCocktail = false;

            string[] orderArray = order.Split('/');

            if (orderArray[0] != "Stolen" && orderArray[0] != "Gingerbread" && orderArray[0] != "MulledWine" && orderArray[0] != "Hibernation")
            {
                return String.Format(OutputMessages.NotRecognizedType, orderArray[0]);
            }
            if (!booth.CocktailMenu.Models.Any(x =>x.Name == orderArray[1]) && !booth.DelicacyMenu.Models.Any(x => x.Name == orderArray[1]))
            {
                return String.Format(OutputMessages.CocktailStillNotAdded, orderArray[0], orderArray[1]);
            }

            if (orderArray[0] == "MulledWine" || orderArray[0] == "Hibernation")
            {
                isCocktail = true;
            }

            string size = orderArray[3];

            if (isCocktail)
            {
                ICocktail desiredCoctail = booth
                .CocktailMenu.Models
                .FirstOrDefault(m => m.GetType().Name == orderArray[0] && m.Name == orderArray[1] && m.Size == size);
                if (desiredCoctail == null)
                {
                    return String.Format(OutputMessages.CocktailStillNotAdded, size, orderArray[1]);
                }
                booth.UpdateCurrentBill(desiredCoctail.Price * int.Parse(orderArray[2]));
                return String.Format(OutputMessages.SuccessfullyOrdered, booth.BoothId, orderArray[2], orderArray[1]);
            }
            else
            {
                IDelicacy desiredCoctail = booth
                .DelicacyMenu.Models
                .FirstOrDefault(m => m.GetType().Name == orderArray[0] && m.Name == orderArray[1]);
                if (desiredCoctail == null)
                {
                    return String.Format(OutputMessages.DelicacyStillNotAdded, orderArray[0], orderArray[1]);
                }
                booth.UpdateCurrentBill(desiredCoctail.Price * int.Parse(orderArray[2]));
                return String.Format(OutputMessages.SuccessfullyOrdered, booth.BoothId, orderArray[2], orderArray[1]);
            }


        }
    }
}
