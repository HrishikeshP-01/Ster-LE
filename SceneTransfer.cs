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

				// Add mesh type & position to file
				float lx = mp.position.x;
				float ly = mp.position.y;
				float lz = mp.position.z;
				data += meshType[i].ToString() + "," + lx.ToString() + "," + ly.ToString() + "," + lz.ToString() + ",";

				// Add orientation to file
				float qx = mp.orientation.x;
				float qy = mp.orientation.y;
				float qz = mp.orientation.z;
				float qw = mp.orientation.w;
				data += qx.ToString() + "," + qy.ToString() + "," + qz.ToString() + "," + qw.ToString() + ",";

				// Add dimension info to file
				float sx = m.Bounds.dimensions.x;
				float sy = m.Bounds.dimensions.y;
				float sz = m.Bounds.dimensions.z;
				data += sx.ToString() + "," + sy.ToString() + "," + sz.ToString() + "," ;

				// Add vertex count to file
				float vc = m.GetVerts().Length;
				data += vc.ToString() + ",";
				data += "\n";
			}
			return data;
		}
	}
}
