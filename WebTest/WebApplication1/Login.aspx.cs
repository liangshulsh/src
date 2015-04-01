using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (this.txtUserName.Text == string.Empty)
            {
                this.Session["UserName"] = "World!";
            }
            else
            {
                this.Session["UserName"] = this.txtUserName.Text.ToString();
            }

            this.Response.Redirect("Default.aspx");
        }
    }
}