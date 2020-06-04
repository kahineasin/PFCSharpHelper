using System;

namespace DirectSeller.classes
{
	/// <summary>
	/// Message 的摘要说明。
	/// </summary>
	public class Message
	{
		private int id;

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id=value;
			}
		}

		private string title;

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title=value;
			}
		}

		private string mess;

		public string Mess
		{
			get
			{
				return mess;
			}
			set
			{
				mess=value;
			}
		}

		public Message(int id,string title,string mess)
		{
			this.id=id;
			this.title=title;
			this.mess=mess;
		}


	}
}
