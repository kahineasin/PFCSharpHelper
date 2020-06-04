using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;

namespace DirectSeller.classes
{
	/// <summary>
	/// SessionManager ��ժҪ˵����
	/// </summary>
	public class SessionManager:baseclass
	{
		public Page _page;
		public object this[string key]
		{
			get
			{
				if(_page.Session[key]==null)
				{
					_page.Response.Redirect("~/Login.aspx");
					return null;
				}
				else
					return _page.Session[key];
			}
			set
			{
				_page.Session[key]=value;
			}
		}
		public void ExitSession(string key)
		{
			_page.Session.Remove(key);
		}
		public void SessionSet(string key,object _value)
		{
			_page.Session[key]=_value;
		}
		public object GetSession(string key)
		{
			if(_page.Session[key]==null)
			{
                _page.Response.Redirect("~/Login.aspx");
				return null;
			}
			else
				return _page.Session[key];
		}
		public object GetSession(string key,string Url)
		{
			if(_page.Session[key]==null)
			{
				_page.Response.Redirect(Url);
				return null;
			}
			else
				return _page.Session[key];
		}
		public SessionManager(Page page)
		{
			_page=page;
		}
	}

	/// <summary>
	/// ״̬������
	/// </summary>
	public class ViewManager:System.Web.UI.Control
	{
		public object this[string key]
		{
			get
			{
				return ViewState[key];
			}
			set
			{
				ViewState[key]=value;
			}
		}
	}
}
