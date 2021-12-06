using StereoKit;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace StereoKitApp
{
    class SceneTranfer
    {
		public string ExportMeshData(List<Mesh> meshList, List<Pose> meshPoses, List<int> meshType)
		{
			string data = "";
			for (int i = 0; i < meshList.Count; i++)
			{
				Mesh m = meshList[i];
				Pose mp = meshPoses[i];
				float lx = mp.position.x;
				float ly = mp.position.y;
				float lz = mp.position.z;
				data += meshType[i].ToString() + "," + lx.ToString() + "," + ly.ToString() + "," + lz.ToString() + ",";
				float qx = mp.orientation.x;
				float qy = mp.orientation.y;
				float qz = mp.orientation.z;
				float qw = mp.orientation.w;
				data += qx.ToString() + "," + qy.ToString() + "," + qz.ToString() + "," + qw.ToString() + ",";
				data += "\n";
			}
			return data;
		}
	}
}
