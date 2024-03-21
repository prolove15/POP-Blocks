using System;
using System.Collections.Generic;

namespace POPBlocks.Server.Network
{
	public interface IDataManagerEpsilon {
		
		void GetLevels(Action<ResultObject> Callback);
		
		void SetBoosterData (Dictionary<string, int> dic);

		void GetBoosterData (Action<ResultObject[]> Callback);
		void UpdateBoost(string name, int count);

		void Logout ();

		void SetLevels();
	}
}


