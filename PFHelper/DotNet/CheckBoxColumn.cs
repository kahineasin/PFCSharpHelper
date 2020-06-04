using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Perfect
{
    public class CheckBoxColumn : TemplateColumn
    {
        public CheckBoxColumn():base() {
            this.ItemTemplate = new CheckBoxColumnItemTemplate();
            this.HeaderTemplate = new CheckBoxColumnHeaderTemplate();

        }

        protected class CheckBoxColumnItemTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                CheckBox lb = new CheckBox();
                lb.CssClass = "grid_row_select";
                container.Controls.Add(lb);
            }
        }
        protected class CheckBoxColumnHeaderTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                CheckBox lb = new CheckBox();
                lb.CssClass = "grid_row_selectall";
                container.Controls.Add(lb);
            }
        }
    }
}
