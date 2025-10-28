using System;

namespace Alurax
{
	static public class TimeHelper
	{
		readonly static private DateTime m_UtcTime;
		
		static TimeHelper()
		{
			m_UtcTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		static private bool m_Synced;
		static private DateTime m_UtcTimeAtSync;
		static private DateTime m_UtcRemoteTimeAtSync;
		
		static public DateTime UTC
		{
			get
			{
				DateTime now = DateTime.UtcNow;
				if (m_Synced) {
					var deltaTime = now - m_UtcTimeAtSync;
					now = m_UtcRemoteTimeAtSync + deltaTime;
				}
				return now;
			}
		}
		
		static public DateTime Now
		{
			get
			{
				return UTC.ToLocalTime();
			}
		}
		
		
		static public int timestamp
		{
			get
			{
				var totalSeconds=(UTC.ToLocalTime() - m_UtcTime).TotalSeconds;
				return Convert.ToInt32(Math.Floor(totalSeconds));
			}
		}

		static public int utcTimestamp
		{
			get
			{
				var totalSeconds=(UTC - m_UtcTime).TotalSeconds;
				return Convert.ToInt32(Math.Floor(totalSeconds));
			}
		}

		
		static public void Sync(double timestamp)
		{
			m_Synced = true;
			m_UtcTimeAtSync = DateTime.UtcNow;
			m_UtcRemoteTimeAtSync = m_UtcTime.AddSeconds(timestamp);
			
		}
		
		//将UTC时间戳转成本地时间戳
		static public int UtcToLocalTimeStamp(int timestamp)
		{
			return Convert.ToInt32((m_UtcTime.AddSeconds(timestamp).ToLocalTime() - m_UtcTime).TotalSeconds);
		}
		
		//将本地时间戳转成UTC时间戳
		static public int LocalToUtcTimeStamp(int timestamp)
		{
			return Convert.ToInt32((m_UtcTime.ToLocalTime().AddSeconds(timestamp).ToUniversalTime() - m_UtcTime.ToLocalTime()).TotalSeconds);
		}

		static public DateTime GetTimeData(int timestamp)
		{
			return m_UtcTime.AddSeconds(timestamp);
		}
		
	}
}
