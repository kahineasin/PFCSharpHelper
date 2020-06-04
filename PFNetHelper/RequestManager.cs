using System;
using System.Collections;
using System.Web;
using System.Web.UI;

namespace DirectSeller.classes
{
	/// <summary>
	/// RequestManager ��ժҪ˵����
	/// </summary>
	public class RequestManager:baseclass
	{
		private Page _page;
		public RequestManager(Page page)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			_page=page;
		}


		public string this[string key]
		{
			get
			{
				if(_page.Request[key]!=null)
				{
					return _page.Request[key];
				}
				else
				{
					_page.Response.Redirect("~/Login.aspx");
					return string.Empty;
				}
			}
		}
		/// <summary>
		/// ��ȡ����ҳ��
		/// </summary>
		public string OldPage
		{
			get
			{
				return getOldPageString();
			}
		}

		private string getOldPageString()
		{
			string rawurl=_page.Request.ServerVariables["HTTP_REFERER"];
            if (string.IsNullOrEmpty(rawurl))
                return "";
			int pos1=rawurl.LastIndexOf('/');
			int pos2=rawurl.LastIndexOf('?');
			if(pos2>-1)
				return rawurl.Substring(pos1+1,pos2-pos1-1);
			else
				return rawurl.Substring(pos1+1);
		}
	}
}
