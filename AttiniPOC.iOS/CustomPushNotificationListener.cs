using System;
using Newtonsoft.Json.Linq;
using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;

namespace AttiniPOC.iOS
{
	public class CustomPushNotificationListener : IPushNotificationListener
	{
		public void OnError(string message, DeviceType deviceType)
		{
			throw new NotImplementedException();
		}

		public void OnMessage(JObject values, DeviceType deviceType)
		{
			throw new NotImplementedException();
		}

		public void OnRegistered(string token, DeviceType deviceType)
		{
			if(deviceType == DeviceType.iOS)
			{
			}
		}

		public void OnUnregistered(DeviceType deviceType)
		{
			throw new NotImplementedException();
		}

		public bool ShouldShowNotification()
		{
			throw new NotImplementedException();
		}
	}
}