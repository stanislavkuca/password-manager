using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    //Used to display dummy items in the designer
    public class AccountListData
    {
        public ObservableCollection<Account> Accounts { get; set; }

        public AccountListData()
        {
            Accounts = new ObservableCollection<Account>
            {
                new Account("Google", "user4558888", "password564sdasd6E", "SDHSJDjkasdjdsdjsJKKAsjd"),
                new Account("DesignerClothes", "SDssaaa5444444", "SDsds2s2dsDASD", "starý účet a 1544 46s65sss2s24 244"),
                new Account("DesignerGoldJeweleryMax.com.co", "AS5T2uya987Ybj;'998jh", "SDsds2s2dsDASD", "starý účet a 1544 46s65sss2s24 244 sadddhjytty  ytytytyjtyjt jtjtjt"),
                new Account("Lorem ipsum dolor sit amet,  consectetuer", "adipiscing elit. Nullam feugiat, turpis at pulvinar vulputate", "SDsds2s2dsDASD", "erat libero tristique tellus, nec bibendum odio risus sit amet ante. Fusce aliquam vestibulum ipsum. Mauris tincidunt sem sed arcu.")
            };
        }
    }
}
