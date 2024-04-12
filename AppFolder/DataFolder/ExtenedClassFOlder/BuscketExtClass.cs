using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Linq;
using System.Windows;

namespace GoncharovVympelSale.AppFolder.DataFolder
{
    public partial class Busket
    {
        public int MaxAmount => DBEntities.GetContext().Storage
            .Where(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID && u.ProductID == ProductID)
            .Select(u => u.Amount)
            .FirstOrDefault();

        public decimal Price => DBEntities.GetContext().Product
            .Where(u => u.ProductID == ProductID)
            .Select(u => u.Price)
            .FirstOrDefault() * Amount;

        public string MessageUnder
        {
            get
            {
                var productAmount = DBEntities.GetContext().Storage
                    .Where(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID && u.ProductID == ProductID)
                    .Select(u => u.Amount)
                    .FirstOrDefault();

                return productAmount == 0 ? "Товар закончился!" : Amount > productAmount ? "Количество товаров превышает допустимое на складе! Измените количество." : "";
            }
        }

        public Visibility MessageVisibility
        {
            get
            {
                var productAmount = DBEntities.GetContext().Storage
                    .Where(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID && u.ProductID == ProductID)
                    .Select(u => u.Amount)
                    .FirstOrDefault();

                return Amount > productAmount || productAmount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool AmoutProductEnable
        {
            get
            {
                if(MaxAmount == 0)
                    return false;

                return true;
            }
        }

        public bool incrementEnable
        {
            get
            {



                if (Amount >= MaxAmount || Amount == 999 || !(AmoutProductEnable))
                    return false;

                return true;
            }
        }

        public bool decrementEnable
        {
            get
            {


                if (Amount <= 1 || MaxAmount == 0 || !(AmoutProductEnable))
                    return false;

                return true;
            }
        }

    }
}
