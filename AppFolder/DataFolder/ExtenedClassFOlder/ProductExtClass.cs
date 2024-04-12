using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Linq;
using System.Windows;

namespace GoncharovVympelSale.AppFolder.DataFolder
{
    public partial class Product
    {

        public int? AmoutOfDep
        {
            get
            {
                int? amount = 0;

                var sas = DBEntities.GetContext().Storage.Where(u => u.ProductID == ProductID && u.Amount > 0).ToList();

                foreach (var item in sas)
                {
                    amount += item.Amount;
                }

                return amount;
            }
        }

        public Visibility ButtonEnable
        {
            get
            {
                return GlobalVarriabels.isReadOnly ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public override string ToString()
        {
            return NameProduct.ToString();
        }
    }
}
