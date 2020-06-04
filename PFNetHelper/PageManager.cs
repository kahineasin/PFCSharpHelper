using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Web.Hosting;

namespace DirectSeller.classes
{
	/// <summary>
	/// PageManager 的摘要说明。
	/// </summary>
	public class PageManager:baseclass
	{
		private Page _page=new Page();
		public SessionManager Session;
		public RequestManager Request;
		public ViewManager ViewState;
		public PageManager(Page page)
		{
			_page=page;
            //Literal li=new Literal();
            //li.Text=@"<meta http-equiv=""X-UA-Compatible"" content=""IE=EmulateIE7""/> ";
            //_page.Header.Controls.AddAt(0,li);
			Session=new SessionManager(_page);
			Request=new RequestManager(_page);
			ViewState=new ViewManager();
		}

		public string getClientIP(out string servername)
		{
			servername=_page.Server.MachineName;
			return _page.Request.ServerVariables["REMOTE_ADDR"];
		}

		/// <summary>
		/// 生成客户端错误提示
		/// </summary>
		/// <param name="control">服务器控件</param>
		/// <param name="errmsg">错误提示</param>
		public void RegisterErrorMessage(WebControl control,string errmsg)
		{
			const string SCRIPT_ID="{8637FAE7-6349-40f5-B0D7-C544B9974778}";
			string Script="<script>var objcon=document.all('"+control.ClientID+"');objcon.innerHtml=alert('"+errmsg+"');</script>";
			if(!_page.ClientScript.IsClientScriptBlockRegistered(SCRIPT_ID))
			{
				_page.ClientScript.RegisterStartupScript(_page.GetType(),SCRIPT_ID,Script);
			}
		}

		public void initClientScript(string scriptType, string key, string script)
		{
			string CSSCRIPT = scriptType.ToLower();
			if (CSSCRIPT != null)
			{
				if (!(CSSCRIPT == "css"))
				{
					if (!(CSSCRIPT == "javascript"))
					{
						return;
					}
				}
				else
				{
                    if (!this._page.ClientScript.IsClientScriptBlockRegistered(key))
					{
						script=string.Format("<LINK href=\"{0}\" type=\"text/css\" rel=\"stylesheet\">",script);
                        this._page.ClientScript.RegisterClientScriptBlock(_page.GetType(), key, script);
					}
					return;
				}
				if (!this._page.ClientScript.IsClientScriptBlockRegistered(key))
				{
					script=String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>",script);
                    this._page.ClientScript.RegisterClientScriptBlock(_page.GetType(), key, script);
				}
			}
		}

		public string toRelativePath(string specPath)
		{
			if ((specPath == null) || (specPath == string.Empty))
			{
				return string.Empty;
			}
			string pageVirtualPath = this._page.Server.MapPath(this._page.Request.Path);
			specPath = this._page.Server.MapPath(specPath);
			string[] path1 = pageVirtualPath.Split(new char[] { '\\' });
			string[] path2 = specPath.Split(new char[] { '\\' });
			string newpath = string.Empty;
			string newpath2 = string.Empty;
			int j = 0;
			int path1len = path1.Length;
			int path2len = path1len;
			if (path2.Length > path1len)
			{
				path1len = path2.Length;
			}
			else
			{
				path2len = path2.Length;
			}
			for (int i = 0; i < path1len; i++)
			{
				if ((i < path2len) && (path1[i].ToLower() == path2[i].ToLower()))
				{
					j++;
				}
				else if (i < path2.Length)
				{
					newpath2 = newpath2 + "/" + path2[i];
				}
			}
			if (!(newpath2 != string.Empty))
			{
				return string.Empty;
			}
			newpath2 = newpath2.Substring(1);
			for (int m = 0; m < ((path1.Length - j) - 1); m++)
			{
				newpath = newpath + "../";
			}
			return (newpath + newpath2);
		}

		///// <summary>
		///// 显示系统提示信息
		///// </summary>
		///// <param name="mid">信息ID</param>
		///// <param name="width">宽度</param>
		///// <param name="height">高度</param>
		///// <param name="left">左边距</param>
		///// <param name="top">上边距</param>
		//public void ShowMessage(int mid,int width,int height,int left,int top)
		//{
		//	//获取消息
		//	classes.CommFun cf=new DirectSeller.classes.CommFun();
		//	string title,message;
		//	cf.GetMessage(mid,out title,out message);
		//	if (message!=string.Empty) 
		//	{
		//		string Message="<script language=\"javascript\">function showmessage(){var a = new xWin("+mid+","+width+","+height+","+left+","+top+",\""+title+"\",\""+message+"\");} window.onload = showmessage;</script>";
		//		this._page.RegisterStartupScript("ShowMessagekey",Message);
		//	}
		//}

		//public void ShowMessages2(int mid,int width,int height,int left,int top)
		//{
		//	//获取消息
		//	classes.CommFun cf=new DirectSeller.classes.CommFun();
		//	Message[] messages=cf.GetMessages(mid);
		//	if (messages!=null&&messages.Length>0) 
		//	{
		//		string Message="<script language=\"javascript\">function showmessage(){var a = new xWin("+mid+","+width+","+height+","+left+","+top+",\""+messages[0].Title+"\",\"";
		//		for (int i=0;i<messages.Length;i++)
		//		{
		//			Message+=string.Format(" &nbsp;&nbsp; {0}、{1} <BR> ",messages[i].Id,messages[i].Mess);
		//		}
		//		Message+="\");} window.onload = showmessage;</script>";
		//		this._page.RegisterStartupScript("ShowMessagekey",Message);
		//	}
		//}

        ///// <summary>
        ///// 获取订货消息列表
        ///// </summary>
        ///// <param name="companyid">公司编号</param>
        ///// <param name="width">宽度</param>
        ///// <param name="height">高度</param>
        ///// <param name="left">左边距</param>
        ///// <param name="top">上边距</param>
        //public void ShowMessages(string companyid, int width, int height, int left, int top)
        //{
        //    //获取消息
        //    classes.CommFun cf = new DirectSeller.classes.CommFun();
        //    Message[] messages = cf.GetMessages(companyid);
        //    if (messages != null && messages.Length > 0)
        //    {
        //        string Message = "<script language=\"javascript\">function showmessage(){var a = new xWin(" + messages[0].Id + "," + width + "," + height + "," + left + "," + top + ",\"" + messages[0].Title + "\",\"";
        //        for (int i = 0; i < messages.Length; i++)
        //        {
        //            Message += string.Format(" &nbsp;&nbsp; {0}、{1} <BR> ", messages[i].Id, messages[i].Mess);
        //        }
        //        Message += "\");} window.onload = showmessage;</script>";
        //        this._page.RegisterStartupScript("ShowMessagekey", Message);
        //    }
        //}

		public void genAlertWindow(string message,string location)
		{
			_page.Response.Write("<script>alert('"+message+"');"+location+"</script>");
		}

		public void genAlertWindow2(string message,string location)
		{
			_page.Response.Write("<script>alert('"+message+"');location.assign('"+location+"');</script>");
		}

		public void genAlertWindow(string message)
		{
			_page.Response.Write("<script>alert('"+message+"')</script>");
		}
		public void genAlertWindow(string message,int nflag)
		{
			_page.Response.Write("<script>alert('"+message+"');history.go(-"+nflag.ToString()+");</script>");
		}
		public void genAlertWindow(object control,string message)
		{
			if(control.GetType()==typeof(Label))
			{
				((Label)control).Text="<script>alert('"+message+"')</script>";
			}
		}
		/// <summary>
		/// 注册确认对话框
		/// </summary>
		/// <param name="control">要绑定的控件</param>
		/// <param name="str">注册信息</param>
		public void RegisterConfirmWindow(WebControl control,string str)
		{
			((WebControl)control).Attributes.Add("onclick","return(confirm('"+str+"'))");
		}
		public void RegisterCancelConfirm(object control)
		{
			((Button)control).Attributes.Add("onclick","return(confirm('你确认取消该笔订单吗？'))");
		}
		public void SetControlFocus(string conID1)
		{
//			const string SCRIPT_ID="{975C9E00-6524-423b-A5A0-6D474680391B}";
//			string Script="<script language=\"JavaScript\">document.all('"+conID1.Trim()+"').focus();</script>";
//			if(!_page.IsClientScriptBlockRegistered(SCRIPT_ID))
//			{
//				_page.RegisterStartupScript(SCRIPT_ID,Script);
//			}
			_page.Response.Write("<script language=\"JavaScript\">document.all('"+conID1.Trim()+"').focus();</script>");
			
		}

        public void SetControlFocus2(string conID1)
        {
            const string SCRIPT_ID = "{975C9E00-6524-423b-A5A0-6D474680391B}";
            string Script = "<script language=\"JavaScript\">document.all('" + conID1.Trim() + "').focus();</script>";
            if (!_page.ClientScript.IsClientScriptBlockRegistered(SCRIPT_ID))
            {
                _page.ClientScript.RegisterStartupScript(_page.GetType(), SCRIPT_ID, Script);
            }
            //_page.Response.Write("<script language=\"JavaScript\">document.all('" + conID1.Trim() + "').focus();</script>");

        }

		/// <summary>
		/// 获取查询字符串指定值
		/// </summary>
		/// <param name="key">字符串键值</param>
		/// <returns>返回字符串的值</returns>
		public string GetQueryValue(string key)
		{
			if(_page.Request.QueryString[key]!=null)
				return _page.Request.QueryString[key].ToString();
			else
				return "";
		}

		private bool CheckCurrentWebUrl()
		{
			string url=_page.Request.ServerVariables["URL"];
			string prefix=_page.Request.ServerVariables["SERVER_NAME"];
			string[] prefixes=new string[]{"send.perfect99.com","send01.perfect99.com","send02.perfect99.com","send03.perfect99.com","send04.perfect99.com","send05.perfect99.com"};
			string[] urls=new string[]{
"/webapp/directseller/Shop.aspx","/webapp/directseller/Shop_Quick.aspx","/webapp/directseller/Shop_Retail.aspx",
"/webapp/directseller/Customer_Shop.aspx","/webapp/directseller/Customer_ShoppingCart.aspx","/webapp/directseller/addtocart.aspx",
"/webapp/directseller/Query_ShoppingCart.aspx","/webapp/directseller/Query_ShoppingCart1.aspx",
"/webapp/directseller/CheckOrder.aspx","/webapp/directseller/CancelOrder.aspx","/webapp/directseller/Shop_RetailOut.aspx",
"/webapp/directseller/Customer_Add.aspx","/webapp/directseller/Customer_Edit.aspx","/webapp/directseller/Customer_Input.aspx"
			};
			bool flag=false;
			for (int i=0;i<prefixes.Length;i++)
			{
				if(prefix.ToLower()==prefixes[i].ToLower())
				{
					flag=true;
					break;
				}
			}
			if (flag)
			{
				if (url!=string.Empty)
				{
					if(url.IndexOf("?")>=0)
					{
						url=url.Split(new char[]{'?'})[0];
					}
				}
				for (int i=0;i<urls.Length;i++)
				{
					if (url.ToLower()==urls[i].ToLower())
					{
						flag=true;
						return flag;
					}
				}
				return false;
			}
			else
			{
				return false;
			}
		}

		public bool CheckValidUrl()
		{
			return true;
//			if (!CheckCurrentWebUrl())
//			{
//				genAlertWindow("请输入正确的网址！","location.assign('Login.aspx')");
//				_page.Session.Abandon();
//				
//				return false;
//			}
//			else
//			{
//				return true;
//			}
		}
	}
}
