using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonNet
{
	public enum LOG_TYPE
	{
		NONE,
		INFO,
		ERROR
	}

	class CUnitUtil
	{
		static public string TimeStamp()
		{
			DateTime Now	= DateTime.Now;			
			
			string strTime = String.Format("[{0:D4}-{1:D2}-{2:D2} {3:D2}.{4:D2}.{5:D2}.{6:D3}]",
			                               Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, Now.Second, Now.Millisecond);
			return strTime;
		}
	}


	//간단하게 로그파일 남기기위해 작성
	public class CEzLog
	{
		//파일타입, 현재는 파일이름만...
		public enum LogType
		{
			NONE,
			FILE_NAME_ADD_TIME,
		}

		//생성자, strFileName 파일이름
		public CEzLog(string strFileName)
		{
			Create(strFileName, LogType.FILE_NAME_ADD_TIME);
		}

		//생성자, strFileName 파일이름, Type 파일타입
		public CEzLog(string strFileName, LogType Type )
		{
			Create(strFileName, Type);
		}

		//파일쓰기
		public void WriteLine( string str )
		{
			string strLog = CUnitUtil.TimeStamp() + str;
			m_Sw.WriteLine(strLog);
			//m_Sw.Flush();
		}

		//파일쓰기
		public void Log( string str, LOG_TYPE LogType = LOG_TYPE.NONE )
		{
			//CNetLog.AssertX(null != str, "null != str");
			
			string strMessage;
			if (LOG_TYPE.NONE == LogType)
			{
				WriteLine(str);
				return;
			}
			else if( LOG_TYPE.INFO == LogType )
			{
				strMessage = String.Format("[INFO]{0}", str);
			}
			else
			{//타입이 안정해지면 다 ERROR 타입으로
				strMessage = String.Format("[ERROR]{0}", str);
			}
			WriteLine(strMessage);
		}


        /// <summary>
        /// 20130924 chamto
        /// 유니티에서 파일을 안닫으면 중간에 열어볼수가 없어서, 파일을 한번쓰고 닫게 처리함
        /// 다시 파일을 열기위해 함수 추가함
        /// </summary>
        public void ReCreate(string strFileName)
        {
            Close();
            Create(strFileName, LogType.NONE);
        }


		//파일닫기
		public void Close()
		{
            if (null != m_Sw)
            {
                m_Sw.Flush(); //파일을 닫기전에 버퍼에 있는 내용을 모두 출력한다.
                m_Sw.Close();
                m_Sw = null;
            }
		}

		//기본생성 할수없음
		CEzLog(){}

		//실제 파일생성
		void Create(string strFileName, LogType Type)
		{
			string strDir = System.IO.Directory.GetCurrentDirectory() + "\\Log\\";  // ㅋ클릭하지 마이소~ 
			System.IO.Directory.CreateDirectory( strDir );
			
			if (LogType.FILE_NAME_ADD_TIME == Type)
			{
				m_strFileName = strDir + CUnitUtil.TimeStamp() + strFileName;		//시간 + 이름으로 파일명을 만듬
				m_Sw = new StreamWriter(new FileStream(m_strFileName, FileMode.OpenOrCreate, FileAccess.Write));
			}
			else
			{
				//m_Sw = new StreamWriter(new FileStream(m_strFileName, FileMode.OpenOrCreate, FileAccess.Write));
                m_Sw = new StreamWriter(new FileStream(m_strFileName, FileMode.Append, FileAccess.Write));
			}
			m_Sw.AutoFlush = true;		//자동플러쉬되어 WriteLine이 호출되면 바로 파일에 기록
		}

        
		//파일쓰기 핸들
		StreamWriter m_Sw			= null;

		//파일패쓰이름
		string m_strFileName { get; set; }
	}

}
