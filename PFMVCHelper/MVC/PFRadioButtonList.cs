using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfect.MVC
{
    public class PFRadioButtonList:PFCheckBoxList
    {
        protected override string InputType { get { return "radio"; } }
    }
}
